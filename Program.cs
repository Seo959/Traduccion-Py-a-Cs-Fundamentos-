// v0.12 - Cancelar citas, eliminar mascotas y fechas bien comparadas

using Biblioteca;

class Program
{
    static void Main()
    {
        Console.WriteLine("Los datos se guardan en: " + ModuloArchivo.Carpeta);
        int opcion = -1;

        while (opcion != 0)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLINICA DR. LEO v0.12 ===");
            Console.WriteLine("1. Registrar propietario");
            Console.WriteLine("2. Registrar mascota");
            Console.WriteLine("3. Agendar cita");
            Console.WriteLine("4. Buscar");
            Console.WriteLine("5. Ver reporte ordenado");
            Console.WriteLine("6. Exportar reporte (txt y csv)");
            Console.WriteLine("7. Cancelar cita");
            Console.WriteLine("8. Eliminar mascota");
            Console.WriteLine("0. Salir");

            int.TryParse(ModuloConsola.LeerTexto("Elige una opcion: "), out opcion);

            if (opcion == 1) RegistrarPropietario();
            else if (opcion == 2) RegistrarMascota();
            else if (opcion == 3) AgendarCita();
            else if (opcion == 4) Buscar();
            else if (opcion == 5) VerReporte();
            else if (opcion == 6) ExportarReporte();
            else if (opcion == 7) CancelarCita();
            else if (opcion == 8) EliminarMascota();
            else if (opcion != 0) Console.WriteLine("Opcion no valida");
        }

        Console.WriteLine("Hasta luego");
    }

    // esto pide los datos del dueño y lo guarda en propietarios.txt
    static void RegistrarPropietario()
    {
        string dni = ModuloConsola.LeerTexto("DNI (8 numeros): ");

        if (!ModuloRegistro.DniValido(dni))
        {
            Console.WriteLine("El DNI debe tener 8 numeros");
            return;
        }

        if (ModuloBusqueda.BuscarPropietario(dni) != null)
        {
            Console.WriteLine("Ya existe un propietario con ese DNI");
            return;
        }

        string nombre = ModuloConsola.LeerTexto("Nombre: ");

        string telefono = ModuloConsola.LeerTexto("Telefono (9 numeros): ");

        if (!ModuloRegistro.TelefonoValido(telefono))
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
        string dni = ModuloConsola.LeerTexto("DNI del dueño: ");

        if (ModuloBusqueda.BuscarPropietario(dni) == null)
        {
            Console.WriteLine("No existe un propietario con ese DNI");
            return;
        }

        string nombre = ModuloConsola.LeerTexto("Nombre de la mascota: ");

        if (ModuloBusqueda.BuscarMascota(dni, nombre) != null)
        {
            Console.WriteLine("Ese dueño ya tiene una mascota con ese nombre");
            return;
        }

        string especie = ModuloConsola.LeerTexto("Especie: ");

        if (!ModuloRegistro.EdadValida(ModuloConsola.LeerTexto("Edad (0 a 100): "), out int edad))
        {
            Console.WriteLine("Edad invalida");
            return;
        }

        string codigo = ModuloRegistro.GenerarCodigo("EXP-", ModuloArchivo.Mascotas, 4);
        ModuloArchivo.Guardar(ModuloArchivo.Mascotas, dni + "|" + nombre + "|" + especie + "|" + edad + "|" + codigo);
        Console.WriteLine("Mascota registrada con codigo " + codigo);
    }

    // esto agenda una cita para una mascota, valida fecha, hora y turno libre
    static void AgendarCita()
    {
        string dni = ModuloConsola.LeerTexto("DNI del dueño: ");
        string nombre = ModuloConsola.LeerTexto("Nombre de la mascota: ");

        if (ModuloBusqueda.BuscarMascota(dni, nombre) == null)
        {
            Console.WriteLine("No se encontro esa mascota");
            return;
        }

        string fecha = ModuloConsola.LeerTexto("Fecha (DD/MM/AAAA): ");
        string hora = ModuloConsola.LeerTexto("Hora (HH:MM): ");

        if (!ModuloRegistro.FechaValida(fecha) || !ModuloRegistro.HoraValida(hora))
        {
            Console.WriteLine("Fecha u hora invalida");
            return;
        }

        fecha = ModuloRegistro.NormalizarFecha(fecha);

        if (ModuloRegistro.TurnoOcupado(fecha, hora))
        {
            Console.WriteLine("Ya existe una cita en ese dia y horario");
            return;
        }

        string motivo = ModuloConsola.LeerTexto("Motivo: ");

        for (int i = 0; i < ModuloRegistro.Tipos.Length; i++)
        {
            Console.WriteLine(i + ". " + ModuloRegistro.Tipos[i]);
        }

        if (!int.TryParse(ModuloConsola.LeerTexto("Tipo de tramite: "), out int numeroTipo) || numeroTipo < 0 || numeroTipo >= ModuloRegistro.Tipos.Length)
        {
            Console.WriteLine("Tipo invalido");
            return;
        }

        string codigo = ModuloRegistro.GenerarCodigo("CIT-" + DateTime.Now.Year + "-", ModuloArchivo.Citas, 6);
        ModuloArchivo.Guardar(ModuloArchivo.Citas, dni + "|" + nombre + "|" + fecha + "|" + hora + "|" + motivo + "|Pendiente|" + codigo + "|" + ModuloRegistro.Tipos[numeroTipo]);
        Console.WriteLine("Cita agendada con codigo " + codigo);
    }

    // esto busca un texto y muestra lo que encuentre en propietarios, mascotas y citas
    static void Buscar()
    {
        List<string> encontrados = ModuloBusqueda.Buscar(ModuloConsola.LeerTexto("Buscar: "));

        foreach (string linea in encontrados)
        {
            Console.WriteLine(linea);
        }

        if (encontrados.Count == 0) Console.WriteLine("No se encontraron resultados");
    }

    // esto cancela una cita pidiendo su codigo
    static void CancelarCita()
    {
        // ERROR 10: se cancela sin pedir confirmacion, un codigo mal escrito cancela la cita equivocada
        Console.WriteLine(ModuloEdicion.CancelarCita(ModuloConsola.LeerTexto("Codigo de la cita: ")));
    }

    // esto elimina una mascota pidiendo el DNI del dueño y su nombre
    static void EliminarMascota()
    {
        string dni = ModuloConsola.LeerTexto("DNI del dueño: ");
        string nombre = ModuloConsola.LeerTexto("Nombre de la mascota: ");

        // ERROR 10: tampoco pide confirmacion antes de eliminar
        Console.WriteLine(ModuloEdicion.EliminarMascota(dni, nombre));
    }

    // esto muestra el reporte en pantalla con las mascotas ordenadas
    static void VerReporte()
    {
        Console.WriteLine("1. Ordenar por codigo");
        Console.WriteLine("2. Ordenar por nombre");

        // ERROR 8: si se escribe otra cosa, se ordena por codigo sin avisar
        int.TryParse(ModuloConsola.LeerTexto("Criterio: "), out int criterio);
        int columna = criterio == 2 ? 1 : 4;

        foreach (string linea in ModuloReporte.GenerarReporte(columna))
        {
            Console.WriteLine(linea);
        }
    }

    // esto guarda el reporte en txt y csv dentro de la carpeta datos
    static void ExportarReporte()
    {
        Console.WriteLine("Guardado en " + ModuloReporte.ExportarTexto(4));
        Console.WriteLine("Guardado en " + ModuloReporte.ExportarCsv());
    }
}
