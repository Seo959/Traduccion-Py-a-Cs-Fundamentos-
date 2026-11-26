using System.Globalization;

namespace Biblioteca;

// modulo de registros: validaciones, codigos y turnos
static class ModuloRegistro
{
    // tipos de tramite que se pueden registrar
    public static readonly string[] Tipos = { "Cita", "Venta", "Cirugia", "Otro" };

    // esto revisa que el DNI tenga 8 numeros
    public static bool DniValido(string dni)
    {
        return dni.Length == 8 && dni.All(char.IsDigit);
    }

    // esto revisa que el telefono tenga 9 numeros
    public static bool TelefonoValido(string telefono)
    {
        return telefono.Length == 9 && telefono.All(char.IsDigit);
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
}
