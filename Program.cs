// v0.4 - los datos se guardan y cargan desde un archivo JSON

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
            Console.WriteLine("=== CLINICA DR. LEO v0.4 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Ver registros");
            Console.WriteLine("0. Salir");

            opcion = LeerEntero("Elige una opcion: ", 0, 3);

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) VerRegistros();
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

        // ERROR 3: si el archivo esta danado o editado mal, aqui el programa se cierra
        // (en Python se usa try/except y se continua con las listas vacias)
        DatosClinica datos = JsonSerializer.Deserialize<DatosClinica>(File.ReadAllText(rutaDatos));

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

        // ERROR 4: no se puede editar ni eliminar nada, si te equivocas hay que borrar el JSON a mano
    }
}
