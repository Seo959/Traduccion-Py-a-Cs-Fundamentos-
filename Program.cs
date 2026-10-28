// v0.1 - esqueleto basico de la clinica veterinaria (solo consola)

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

        // el menu se repite hasta que el usuario elija 0
        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.1 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Ver registros");
            Console.WriteLine("0. Salir");
            Console.Write("Elige una opcion: ");

            // ERROR 1: si escriben una letra, int.Parse falla y el programa se cierra
            opcion = int.Parse(Console.ReadLine());

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) VerRegistros();
        }

        // ERROR 2: al salir se pierden todos los datos, todavia no se guardan en archivo
        Console.WriteLine("Hasta luego");
    }

    // pide los datos del dueño y lo agrega a la lista
    static void RegistrarPropietario()
    {
        Propietario p = new Propietario();

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
        Mascota m = new Mascota();

        Console.Write("Nombre de la mascota: ");
        m.Nombre = Console.ReadLine();

        Console.Write("Especie: ");
        m.Especie = Console.ReadLine();

        // ERROR 4: si la edad no es un numero, el programa se cierra
        Console.Write("Edad: ");
        m.Edad = int.Parse(Console.ReadLine());

        // muestra los dueños con su numero para elegir uno
        for (int i = 0; i < propietarios.Count; i++)
        {
            Console.WriteLine(i + ". " + propietarios[i].Nombre);
        }

        // ERROR 5: si no hay dueños o el numero no existe, la lista falla y se cierra
        Console.Write("Numero del propietario: ");
        int indice = int.Parse(Console.ReadLine());
        m.Dueno = propietarios[indice];

        mascotas.Add(m);
        Console.WriteLine("Mascota registrada");
    }

    // muestra todo lo que hay en las listas
    static void VerRegistros()
    {
        Console.WriteLine("--- Propietarios ---");
        foreach (Propietario p in propietarios)
        {
            // ERROR 6: el DNI se muestra completo, deberia estar oculto
            Console.WriteLine(p.Nombre + " | DNI: " + p.Dni);
        }

        Console.WriteLine("--- Mascotas ---");
        foreach (Mascota m in mascotas)
        {
            Console.WriteLine(m.Nombre + " | " + m.Especie + " | " + m.Edad + " años | Dueño: " + m.Dueno.Nombre);
        }
    }
}
