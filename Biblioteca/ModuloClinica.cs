using System.Globalization;

namespace Biblioteca;

// esta clase tiene las validaciones y busquedas de la clinica
static class ModuloClinica
{
    // esto revisa que el DNI tenga 8 numeros
    public static bool DniValido(string dni)
    {
        return dni.Length == 8 && dni.All(char.IsDigit);
    }

    // esto revisa que la edad sea un numero entre 0 y 100
    public static bool EdadValida(string texto, out int edad)
    {
        return int.TryParse(texto, out edad) && edad >= 0 && edad <= 100;
    }

    // esto revisa que la fecha tenga formato DD/MM/AAAA
    public static bool FechaValida(string fecha)
    {
        return DateTime.TryParseExact(fecha, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    // esto revisa que la hora tenga formato HH:MM de 24 horas
    public static bool HoraValida(string hora)
    {
        return DateTime.TryParseExact(hora, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    // esto busca un propietario por DNI, devuelve null si no existe
    public static string[] BuscarPropietario(string dni)
    {
        foreach (string[] p in ModuloArchivo.Leer(ModuloArchivo.Propietarios))
        {
            if (p[0] == dni) return p;
        }

        return null;
    }

    // esto busca una mascota por DNI del dueño y nombre, devuelve null si no existe
    public static string[] BuscarMascota(string dni, string nombre)
    {
        foreach (string[] m in ModuloArchivo.Leer(ModuloArchivo.Mascotas))
        {
            if (m[0] == dni && m[1].ToLower() == nombre.ToLower()) return m;
        }

        return null;
    }

    // esto crea el siguiente codigo contando las lineas del archivo, ej: EXP-001
    public static string GenerarCodigo(string prefijo, string archivo)
    {
        // ERROR 6: si algun dia se elimina un registro, el codigo se puede repetir
        return prefijo + (ModuloArchivo.Leer(archivo).Count + 1).ToString("000");
    }

    // esto dice si ya hay una cita pendiente en ese dia y hora
    public static bool TurnoOcupado(string fecha, string hora)
    {
        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            // ERROR 3: se comparan textos, "5/11/2026" y "05/11/2026" cuentan como fechas distintas
            if (c[2] == fecha && c[3] == hora && c[5] == "Pendiente") return true;
        }

        return false;
    }

    // esto arma las lineas del reporte general
    public static List<string> GenerarReporte()
    {
        List<string> lineas = new List<string>();
        lineas.Add("REPORTE - CLINICA DR. LEO");

        lineas.Add("--- Propietarios ---");
        foreach (string[] p in ModuloArchivo.Leer(ModuloArchivo.Propietarios))
        {
            // ERROR 2: el DNI se muestra completo, en Python se muestra con asteriscos
            lineas.Add(p[1] + " | DNI: " + p[0] + " | Telefono: " + p[2]);
        }

        lineas.Add("--- Mascotas ---");
        foreach (string[] m in ModuloArchivo.Leer(ModuloArchivo.Mascotas))
        {
            lineas.Add(m[4] + " | " + m[1] + " | " + m[2] + " | " + m[3] + " años | DNI dueño: " + m[0]);
        }

        lineas.Add("--- Citas ---");
        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            lineas.Add(c[6] + " | " + c[2] + " " + c[3] + " | " + c[1] + " | " + c[4] + " | " + c[5]);
        }

        return lineas;
    }
}
