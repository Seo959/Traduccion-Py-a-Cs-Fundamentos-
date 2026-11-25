// v0.7 - Reinicio: biblioteca en consola con archivos de texto

using Biblioteca;

class Program
{
    static void Main()
    {
        int opcion = -1;

        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.7 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Agendar cita");
            Console.WriteLine("4. Buscar");
            Console.WriteLine("5. Ver reporte");
            Console.WriteLine("6. Exportar reporte a texto");
            Console.WriteLine("0. Salir");

            int.TryParse(LeerTexto("Elige una opcion: "), out opcion);

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) AgendarCita();
            else if (opcion == 4) Buscar();
            else if (opcion == 5) VerReporte();
            else if (opcion == 6) ExportarReporte();
            else if (opcion != 0) Console.WriteLine("Opcion no valida");
        }

        Console.WriteLine("Hasta luego");
    }

    // esto pide un texto y no deja que quede vacio
    static string LeerTexto(string mensaje)
    {
        string texto = "";

        while (texto == "")
        {
            Console.Write(mensaje);
            texto = (Console.ReadLine() ?? "").Trim();
        }

        return texto;
    }

    // esto pide los datos del dueño y lo guarda en propietarios.txt
    static void RegistrarPropietario()
    {
        string dni = LeerTexto("DNI (8 numeros): ");

        if (!ModuloClinica.DniValido(dni))
        {
            Console.WriteLine("El DNI debe tener 8 numeros");
            return;
        }

        if (ModuloClinica.BuscarPropietario(dni) != null)
        {
            Console.WriteLine("Ya existe un propietario con ese DNI");
            return;
        }

        string nombre = LeerTexto("Nombre: ");

        // ERROR 4: el telefono no se valida, y si alguien escribe "|" en un texto se rompe el archivo
        string telefono = LeerTexto("Telefono: ");

        ModuloArchivo.Guardar(ModuloArchivo.Propietarios, dni + "|" + nombre + "|" + telefono);
        Console.WriteLine("Propietario registrado");
    }

    // esto registra una mascota de un dueño que ya existe
    static void RegistrarMascota()
    {
        string dni = LeerTexto("DNI del dueño: ");

        if (ModuloClinica.BuscarPropietario(dni) == null)
        {
            Console.WriteLine("No existe un propietario con ese DNI");
            return;
        }

        string nombre = LeerTexto("Nombre de la mascota: ");

        if (ModuloClinica.BuscarMascota(dni, nombre) != null)
        {
            Console.WriteLine("Ese dueño ya tiene una mascota con ese nombre");
            return;
        }

        string especie = LeerTexto("Especie: ");

        if (!ModuloClinica.EdadValida(LeerTexto("Edad (0 a 100): "), out int edad))
        {
            Console.WriteLine("Edad invalida");
            return;
        }

        ModuloArchivo.Guardar(ModuloArchivo.Mascotas, dni + "|" + nombre + "|" + especie + "|" + edad);
        Console.WriteLine("Mascota registrada");
    }

    // esto agenda una cita para una mascota, valida fecha, hora y turno libre
    static void AgendarCita()
    {
        string dni = LeerTexto("DNI del dueño: ");
        string nombre = LeerTexto("Nombre de la mascota: ");

        if (ModuloClinica.BuscarMascota(dni, nombre) == null)
        {
            Console.WriteLine("No se encontro esa mascota");
            return;
        }

        string fecha = LeerTexto("Fecha (DD/MM/AAAA): ");
        string hora = LeerTexto("Hora (HH:MM): ");

        if (!ModuloClinica.FechaValida(fecha) || !ModuloClinica.HoraValida(hora))
        {
            Console.WriteLine("Fecha u hora invalida");
            return;
        }

        if (ModuloClinica.TurnoOcupado(fecha, hora))
        {
            Console.WriteLine("Ya existe una cita en ese dia y horario");
            return;
        }

        string motivo = LeerTexto("Motivo: ");

        // ERROR 5: la cita queda siempre Pendiente, todavia no se puede cancelar, editar ni eliminar nada
        ModuloArchivo.Guardar(ModuloArchivo.Citas, dni + "|" + nombre + "|" + fecha + "|" + hora + "|" + motivo + "|Pendiente");
        Console.WriteLine("Cita agendada");
    }

    // esto busca un texto en propietarios (nombre, DNI) y mascotas (nombre, especie)
    static void Buscar()
    {
        string texto = LeerTexto("Buscar: ").ToLower();
        int encontrados = 0;

        foreach (string[] p in ModuloArchivo.Leer(ModuloArchivo.Propietarios))
        {
            if (p[0].Contains(texto) || p[1].ToLower().Contains(texto))
            {
                Console.WriteLine("Propietario: " + p[1] + " | DNI: " + p[0]);
                encontrados++;
            }
        }

        foreach (string[] m in ModuloArchivo.Leer(ModuloArchivo.Mascotas))
        {
            if (m[1].ToLower().Contains(texto) || m[2].ToLower().Contains(texto))
            {
                Console.WriteLine("Mascota: " + m[1] + " | " + m[2] + " | DNI dueño: " + m[0]);
                encontrados++;
            }
        }

        if (encontrados == 0) Console.WriteLine("No se encontraron resultados");
    }

    // esto muestra el reporte en pantalla
    static void VerReporte()
    {
        foreach (string linea in ModuloClinica.GenerarReporte())
        {
            Console.WriteLine(linea);
        }
    }

    // esto guarda el mismo reporte en reporte.txt
    static void ExportarReporte()
    {
        File.WriteAllLines("reporte.txt", ModuloClinica.GenerarReporte());
        Console.WriteLine("Reporte guardado en reporte.txt");
    }
}
