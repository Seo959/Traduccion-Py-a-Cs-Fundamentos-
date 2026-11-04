// v0.5 - se puede editar y eliminar propietarios y mascotas

using System.Text.Json;
using System.Text.Json.Serialization;

// guarda los datos de un dueño (ahora con propiedades para que JSON pueda leerlas)
class Propietario
{
    public string Nombre { get; set; }
    public string Dni { get; set; }
    public string Telefono { get; set; }

    // devuelve el DNI con asteriscos, solo se ven los ultimos 2 digitos
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
    public string Nombre { get; set; }
    public string Especie { get; set; }
    public int Edad { get; set; }

    // el dueño no se guarda completo en el JSON, solo su DNI (en Python se usa propietario_id)
    [JsonIgnore]
    public Propietario Dueno { get; set; }
    public string DniDueno { get; set; }
}

// agrupa las dos listas para guardarlas juntas en un solo archivo
// (en Python el JSON tiene las listas propietarios, mascotas, citas, pagos y atenciones)
class DatosClinica
{
    public List<Propietario> Propietarios { get; set; } = new List<Propietario>();
    public List<Mascota> Mascotas { get; set; } = new List<Mascota>();
}

class Program
{
    static List<Propietario> propietarios = new List<Propietario>();
    static List<Mascota> mascotas = new List<Mascota>();

    // ERROR 1: la ruta es relativa, el archivo se crea donde se ejecute el programa
    // (en Python se guarda siempre en la carpeta data del proyecto)
    static string rutaDatos = "datos_clinica.json";

    static void Main()
    {
        // al iniciar se cargan los datos guardados
        CargarDatos();

        int opcion = -1;

        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.5 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Ver registros");
            Console.WriteLine("4. Editar propietario");
            Console.WriteLine("5. Editar mascota");
            Console.WriteLine("6. Eliminar propietario");
            Console.WriteLine("7. Eliminar mascota");
            Console.WriteLine("0. Salir");

            opcion = LeerEntero("Elige una opcion: ", 0, 7);

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) VerRegistros();
            else if (opcion == 4) EditarPropietario();
            else if (opcion == 5) EditarMascota();
            else if (opcion == 6) EliminarPropietario();
            else if (opcion == 7) EliminarMascota();
        }

        Console.WriteLine("Hasta luego");
    }

    // convierte las listas a JSON y las escribe en el archivo
    // (equivale a guardar_datos de clinica_service.py)
    static void GuardarDatos()
    {
        DatosClinica datos = new DatosClinica();
        datos.Propietarios = propietarios;
        datos.Mascotas = mascotas;

        // antes de guardar se copia el DNI del dueño a cada mascota
        foreach (Mascota m in mascotas)
        {
            m.DniDueno = m.Dueno.Dni;
        }

        JsonSerializerOptions opciones = new JsonSerializerOptions();
        opciones.WriteIndented = true;

        // ERROR 2: el DNI y el telefono se guardan en texto plano, sin proteger
        // (en Python se usa CifradorDatos y se guardan como "enc:...")
        File.WriteAllText(rutaDatos, JsonSerializer.Serialize(datos, opciones));
    }

    // lee el archivo y vuelve a llenar las listas
    // (equivale a cargar_datos de clinica_service.py)
    static void CargarDatos()
    {
        if (!File.Exists(rutaDatos))
        {
            return;
        }

        // si el archivo esta danado se avisa y se sigue con las listas vacias
        // (igual que el try/except de _cargar_desde_json en Python)
        DatosClinica datos;
        try
        {
            datos = JsonSerializer.Deserialize<DatosClinica>(File.ReadAllText(rutaDatos));
        }
        catch (JsonException)
        {
            Console.WriteLine("No se pudo leer " + rutaDatos + ", se inicia sin datos");
            return;
        }

        propietarios = datos.Propietarios;

        // se vuelve a unir cada mascota con su dueño usando el DNI guardado
        foreach (Mascota m in datos.Mascotas)
        {
            foreach (Propietario p in propietarios)
            {
                if (p.Dni == m.DniDueno)
                {
                    m.Dueno = p;
                }
            }

            if (m.Dueno != null)
            {
                mascotas.Add(m);
            }
        }
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

    // revisa que el DNI tenga 8 numeros
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

    // busca si ya existe un dueño con ese DNI
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

    // pide los datos del dueño, valida, lo agrega a la lista y guarda el archivo
    static void RegistrarPropietario()
    {
        Propietario p = new Propietario();

        p.Nombre = LeerTexto("Nombre: ");

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

        // ahora si se pide el telefono (en Python tambien es obligatorio)
        p.Telefono = LeerTexto("Telefono: ");

        propietarios.Add(p);
        GuardarDatos();
        Console.WriteLine("Propietario registrado");
    }

    // revisa si el dueño ya tiene una mascota con ese nombre
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

    // pide los datos de la mascota, la une a un dueño y guarda el archivo
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

        if (MascotaRepetida(m.Nombre, m.Dueno))
        {
            Console.WriteLine("Ese propietario ya tiene una mascota con ese nombre");
            return;
        }

        mascotas.Add(m);
        GuardarDatos();
        Console.WriteLine("Mascota registrada");
    }

    // muestra los dueños numerados y devuelve el elegido (-1 si no hay ninguno)
    static int ElegirPropietario()
    {
        if (propietarios.Count == 0)
        {
            Console.WriteLine("No hay propietarios registrados");
            return -1;
        }

        for (int i = 0; i < propietarios.Count; i++)
        {
            Console.WriteLine(i + ". " + propietarios[i].Nombre);
        }

        return LeerEntero("Numero del propietario: ", 0, propietarios.Count - 1);
    }

    // muestra las mascotas numeradas y devuelve la elegida (-1 si no hay ninguna)
    static int ElegirMascota()
    {
        if (mascotas.Count == 0)
        {
            Console.WriteLine("No hay mascotas registradas");
            return -1;
        }

        for (int i = 0; i < mascotas.Count; i++)
        {
            Console.WriteLine(i + ". " + mascotas[i].Nombre + " (" + mascotas[i].Dueno.Nombre + ")");
        }

        return LeerEntero("Numero de la mascota: ", 0, mascotas.Count - 1);
    }

    // vuelve a pedir los datos de un dueño ya registrado
    // (equivale a actualizar_propietario de clinica_service.py)
    static void EditarPropietario()
    {
        int indice = ElegirPropietario();
        if (indice == -1) return;

        Propietario p = propietarios[indice];

        p.Nombre = LeerTexto("Nuevo nombre: ");

        // ERROR 3: al editar solo se revisa que el DNI tenga 8 numeros, no se revisa si otro dueño ya lo tiene
        // (en Python: "Ya existe otro propietario registrado con ese DNI.")
        string dni = LeerTexto("Nuevo DNI (8 numeros): ");
        while (!DniValido(dni))
        {
            Console.WriteLine("El DNI debe tener 8 numeros");
            dni = LeerTexto("Nuevo DNI (8 numeros): ");
        }
        p.Dni = dni;

        p.Telefono = LeerTexto("Nuevo telefono: ");

        GuardarDatos();
        Console.WriteLine("Propietario actualizado");
    }

    // vuelve a pedir los datos de una mascota ya registrada
    // (equivale a actualizar_mascota de clinica_service.py)
    static void EditarMascota()
    {
        int indice = ElegirMascota();
        if (indice == -1) return;

        Mascota m = mascotas[indice];

        string nombre = LeerTexto("Nuevo nombre: ");

        // el nombre nuevo no puede repetirse en otra mascota del mismo dueño
        foreach (Mascota otra in mascotas)
        {
            if (otra != m && otra.Dueno == m.Dueno && otra.Nombre.ToLower() == nombre.ToLower())
            {
                Console.WriteLine("Ese propietario ya tiene una mascota con ese nombre");
                return;
            }
        }

        m.Nombre = nombre;
        m.Especie = LeerTexto("Nueva especie: ");
        m.Edad = LeerEntero("Nueva edad: ", 0, 100);

        GuardarDatos();
        Console.WriteLine("Mascota actualizada");
    }

    // elimina un dueño, solo si no tiene mascotas
    // (en Python: "No se puede eliminar: el propietario todavía tiene mascotas vinculadas.")
    static void EliminarPropietario()
    {
        int indice = ElegirPropietario();
        if (indice == -1) return;

        Propietario p = propietarios[indice];

        foreach (Mascota m in mascotas)
        {
            if (m.Dueno == p)
            {
                Console.WriteLine("No se puede eliminar: el propietario todavia tiene mascotas");
                return;
            }
        }

        // ERROR 4: se elimina sin pedir confirmacion, un numero mal escrito borra el registro
        propietarios.RemoveAt(indice);
        GuardarDatos();
        Console.WriteLine("Propietario eliminado");
    }

    // elimina una mascota de la lista (equivale a eliminar_mascota de clinica_service.py)
    static void EliminarMascota()
    {
        int indice = ElegirMascota();
        if (indice == -1) return;

        mascotas.RemoveAt(indice);
        GuardarDatos();
        Console.WriteLine("Mascota eliminada");
    }

    // muestra todo lo que hay en las listas
    static void VerRegistros()
    {
        Console.WriteLine("--- Propietarios ---");
        foreach (Propietario p in propietarios)
        {
            Console.WriteLine(p.Nombre + " | DNI: " + p.ObtenerDniProtegido() + " | Telefono: " + p.Telefono);
        }

        Console.WriteLine("--- Mascotas ---");
        foreach (Mascota m in mascotas)
        {
            Console.WriteLine(m.Nombre + " | " + m.Especie + " | " + m.Edad + " años | Dueño: " + m.Dueno.Nombre);
        }

        // ERROR 5: las listas no se pueden buscar ni ordenar, si hay muchos registros es dificil encontrar uno
    }
}
