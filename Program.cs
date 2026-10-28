// v0.2 - se corrigen los cierres inesperados del programa

// guarda los datos de un dueño
class Propietario
{
    public string Nombre;
    public string Dni;
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
    // listas en memoria, aqui se guardan los registros mientras el programa esta abierto
    static List<Propietario> propietarios = new List<Propietario>();
    static List<Mascota> mascotas = new List<Mascota>();

    static void Main()
    {
        int opcion = -1;

        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.2 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Ver registros");
            Console.WriteLine("0. Salir");

            // ahora se usa LeerEntero, ya no se cierra si escriben una letra
            opcion = LeerEntero("Elige una opcion: ", 0, 3);

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) VerRegistros();
        }

        // ERROR 1: al salir se pierden todos los datos, todavia no se guardan en archivo
        Console.WriteLine("Hasta luego");
    }

    // pide un numero y repite la pregunta hasta que sea valido y este dentro del rango
    // (idea tomada de la validacion de edad de clinica_service.py: 0 <= int(edad) <= 100)
    static int LeerEntero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            Console.Write(mensaje);

            // TryParse devuelve false en vez de cerrar el programa cuando no es numero
            if (int.TryParse(Console.ReadLine(), out int numero) && numero >= minimo && numero <= maximo)
            {
                return numero;
            }

            Console.WriteLine("Valor no valido, debe ser un numero entre " + minimo + " y " + maximo);
        }
    }

    // pide los datos del dueño y lo agrega a la lista
    static void RegistrarPropietario()
    {
        Propietario p = new Propietario();

        // ERROR 2: se puede dejar el nombre vacio
        Console.Write("Nombre: ");
        p.Nombre = Console.ReadLine();

        // ERROR 3: no se valida el DNI, acepta letras, vacio o cualquier largo
        Console.Write("DNI: ");
        p.Dni = Console.ReadLine();

        propietarios.Add(p);
        Console.WriteLine("Propietario registrado");
    }

    // pide los datos de la mascota y la une a un dueño de la lista
    static void RegistrarMascota()
    {
        // si no hay dueños no se puede registrar mascota (antes esto cerraba el programa)
        if (propietarios.Count == 0)
        {
            Console.WriteLine("Primero registra un propietario");
            return;
        }

        Mascota m = new Mascota();

        Console.Write("Nombre de la mascota: ");
        m.Nombre = Console.ReadLine();

        Console.Write("Especie: ");
        m.Especie = Console.ReadLine();

        // la edad ahora debe ser un numero entre 0 y 100
        m.Edad = LeerEntero("Edad: ", 0, 100);

        for (int i = 0; i < propietarios.Count; i++)
        {
            Console.WriteLine(i + ". " + propietarios[i].Nombre);
        }

        // el numero del dueño solo se acepta si existe en la lista
        int indice = LeerEntero("Numero del propietario: ", 0, propietarios.Count - 1);
        m.Dueno = propietarios[indice];

        // ERROR 4: se puede repetir la misma mascota para el mismo dueño
        mascotas.Add(m);
        Console.WriteLine("Mascota registrada");
    }

    // muestra todo lo que hay en las listas
    static void VerRegistros()
    {
        Console.WriteLine("--- Propietarios ---");
        foreach (Propietario p in propietarios)
        {
            // ERROR 5: el DNI se muestra completo, deberia estar oculto
            Console.WriteLine(p.Nombre + " | DNI: " + p.Dni);
        }

        Console.WriteLine("--- Mascotas ---");
        foreach (Mascota m in mascotas)
        {
            Console.WriteLine(m.Nombre + " | " + m.Especie + " | " + m.Edad + " años | Dueño: " + m.Dueno.Nombre);
        }
    }
}
