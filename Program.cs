// v0.9 - Tipo de tramite, telefono validado y busqueda por DNI o telefono

using Biblioteca;

class Program
{
    static void Main()
    {
        int opcion = -1;

        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.9 ===");
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

    // esto pide un texto, no deja que quede vacio ni que lleve el caracter |
    static string LeerTexto(string mensaje)
    {
        string texto = "";

        while (texto == "")
        {
            Console.Write(mensaje);
            texto = (Console.ReadLine() ?? "").Trim();

            if (texto.Contains('|'))
            {
                Console.WriteLine("No se puede usar el caracter |");
                texto = "";
            }
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

        string telefono = LeerTexto("Telefono (9 numeros): ");

        if (!ModuloClinica.TelefonoValido(telefono))
        {
            Console.WriteLine("El telefono debe tener 9 numeros");
            return;
        }

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

        string codigo = ModuloClinica.GenerarCodigo("EXP-", ModuloArchivo.Mascotas);
        ModuloArchivo.Guardar(ModuloArchivo.Mascotas, dni + "|" + nombre + "|" + especie + "|" + edad + "|" + codigo);
        Console.WriteLine("Mascota registrada con codigo " + codigo);
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

        for (int i = 0; i < ModuloClinica.Tipos.Length; i++)
        {
            Console.WriteLine(i + ". " + ModuloClinica.Tipos[i]);
        }

        if (!int.TryParse(LeerTexto("Tipo de tramite: "), out int numeroTipo) || numeroTipo < 0 || numeroTipo >= ModuloClinica.Tipos.Length)
        {
            Console.WriteLine("Tipo invalido");
            return;
        }

        // ERROR 5: la cita queda siempre Pendiente, todavia no se puede cancelar, editar ni eliminar nada
        string codigo = ModuloClinica.GenerarCodigo("CIT-" + DateTime.Now.Year + "-", ModuloArchivo.Citas);
        ModuloArchivo.Guardar(ModuloArchivo.Citas, dni + "|" + nombre + "|" + fecha + "|" + hora + "|" + motivo + "|Pendiente|" + codigo + "|" + ModuloClinica.Tipos[numeroTipo]);
        Console.WriteLine("Cita agendada con codigo " + codigo);
    }

    // esto busca un texto en propietarios (nombre, DNI, telefono) y mascotas (nombre, especie, codigo)
    // ERROR 7: la busqueda no muestra las citas del cliente, solo propietarios y mascotas
    static void Buscar()
    {
        string texto = LeerTexto("Buscar: ").ToLower();
        int encontrados = 0;

        foreach (string[] p in ModuloArchivo.Leer(ModuloArchivo.Propietarios))
        {
            if (p[0].Contains(texto) || p[1].ToLower().Contains(texto) || p[2].Contains(texto))
            {
                Console.WriteLine("Propietario: " + p[1] + " | DNI: " + p[0] + " | Telefono: " + p[2]);
                encontrados++;
            }
        }

        foreach (string[] m in ModuloArchivo.Leer(ModuloArchivo.Mascotas))
        {
            if (m[1].ToLower().Contains(texto) || m[2].ToLower().Contains(texto) || m[4].ToLower().Contains(texto))
            {
                Console.WriteLine("Mascota: " + m[1] + " | " + m[2] + " | Codigo: " + m[4] + " | DNI dueño: " + m[0]);
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
