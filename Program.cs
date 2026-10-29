// v0.3 - validaciones de propietario y mascota, DNI protegido

// guarda los datos de un dueño
class Propietario
{
    public string Nombre;
    public string Dni;

    // devuelve el DNI con asteriscos, solo se ven los ultimos 2 digitos
    // (misma idea que obtener_dni_protegido de models/entidades.py)
    public string ObtenerDniProtegido()
    {
        if (Dni == "")
        {
            return "No registrado";
        }

        return new string('*', Dni.Length - 2) + Dni.Substring(Dni.Length - 2);
    }
}

// guarda los datos de una mascota y quien es su dueño
class Mascota
{
    public string Nombre;
    public string Especie;
    public int Edad;
    public Propietario Dueno;
}

class Program
{
    static List<Propietario> propietarios = new List<Propietario>();
    static List<Mascota> mascotas = new List<Mascota>();

    static void Main()
    {
        int opcion = -1;

        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.3 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Ver registros");
            Console.WriteLine("0. Salir");

            opcion = LeerEntero("Elige una opcion: ", 0, 3);

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) VerRegistros();
        }

        // ERROR 1: al salir se pierden todos los datos, todavia no se guardan en archivo
        Console.WriteLine("Hasta luego");
    }

    // pide un numero y repite la pregunta hasta que sea valido y este dentro del rango
    static int LeerEntero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (int.TryParse(Console.ReadLine(), out int numero) && numero >= minimo && numero <= maximo)
            {
                return numero;
            }

            Console.WriteLine("Valor no valido, debe ser un numero entre " + minimo + " y " + maximo);
        }
    }

    // pide un texto y repite la pregunta mientras se deje vacio
    static string LeerTexto(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine().Trim();

            if (texto != "")
            {
                return texto;
            }

            Console.WriteLine("Este dato no puede quedar vacio");
        }
    }

    // revisa que el DNI tenga 8 numeros (como validar_propietario en clinica_service.py:
    // "El DNI debe tener 8 números.")
    static bool DniValido(string dni)
    {
        if (dni.Length != 8)
        {
            return false;
        }

        foreach (char c in dni)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    // busca si ya existe un dueño con ese DNI (en Python: "Ya existe un propietario registrado con ese DNI.")
    static bool DniRepetido(string dni)
    {
        foreach (Propietario p in propietarios)
        {
            if (p.Dni == dni)
            {
                return true;
            }
        }

        return false;
    }

    // pide los datos del dueño, valida y lo agrega a la lista
    static void RegistrarPropietario()
    {
        Propietario p = new Propietario();

        // el nombre ya no puede quedar vacio
        p.Nombre = LeerTexto("Nombre: ");

        // el DNI se pide hasta que tenga 8 numeros y no este repetido
        while (true)
        {
            string dni = LeerTexto("DNI (8 numeros): ");

            if (!DniValido(dni))
            {
                Console.WriteLine("El DNI debe tener 8 numeros");
            }
            else if (DniRepetido(dni))
            {
                Console.WriteLine("Ya existe un propietario con ese DNI");
            }
            else
            {
                p.Dni = dni;
                break;
            }
        }

        // ERROR 2: todavia no se pide el telefono, en Python tambien es obligatorio

        propietarios.Add(p);
        Console.WriteLine("Propietario registrado");
    }

    // revisa si el dueño ya tiene una mascota con ese nombre
    // (en Python: "Ese propietario ya tiene una mascota con ese nombre.")
    static bool MascotaRepetida(string nombre, Propietario dueno)
    {
        foreach (Mascota m in mascotas)
        {
            if (m.Dueno == dueno && m.Nombre.ToLower() == nombre.ToLower())
            {
                return true;
            }
        }

        return false;
    }

    // pide los datos de la mascota y la une a un dueño de la lista
    static void RegistrarMascota()
    {
        if (propietarios.Count == 0)
        {
            Console.WriteLine("Primero registra un propietario");
            return;
        }

        Mascota m = new Mascota();

        m.Nombre = LeerTexto("Nombre de la mascota: ");
        m.Especie = LeerTexto("Especie: ");
        m.Edad = LeerEntero("Edad: ", 0, 100);

        for (int i = 0; i < propietarios.Count; i++)
        {
            Console.WriteLine(i + ". " + propietarios[i].Nombre);
        }

        int indice = LeerEntero("Numero del propietario: ", 0, propietarios.Count - 1);
        m.Dueno = propietarios[indice];

        // ya no se permite repetir la misma mascota para el mismo dueño
        if (MascotaRepetida(m.Nombre, m.Dueno))
        {
            Console.WriteLine("Ese propietario ya tiene una mascota con ese nombre");
            return;
        }

        mascotas.Add(m);
        Console.WriteLine("Mascota registrada");
    }

    // muestra todo lo que hay en las listas
    static void VerRegistros()
    {
        Console.WriteLine("--- Propietarios ---");
        foreach (Propietario p in propietarios)
        {
            // ahora el DNI sale protegido
            Console.WriteLine(p.Nombre + " | DNI: " + p.ObtenerDniProtegido());
        }

        Console.WriteLine("--- Mascotas ---");
        foreach (Mascota m in mascotas)
        {
            Console.WriteLine(m.Nombre + " | " + m.Especie + " | " + m.Edad + " años | Dueño: " + m.Dueno.Nombre);
        }

        // ERROR 3: no se puede editar ni eliminar nada, si te equivocas hay que cerrar y empezar de nuevo
    }
}
