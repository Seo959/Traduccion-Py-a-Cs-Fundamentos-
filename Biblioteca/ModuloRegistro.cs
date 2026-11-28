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

    // esto devuelve la fecha siempre como DD/MM/AAAA, ej: 5/3/2026 pasa a 05/03/2026
    public static string NormalizarFecha(string fecha)
    {
        DateTime resultado = DateTime.ParseExact(fecha, "d/M/yyyy", CultureInfo.InvariantCulture);
        return resultado.ToString("dd/MM/yyyy");
    }

    // esto crea el siguiente codigo: busca el numero mas alto que ya existe y le suma 1, ej: EXP-004
    // "columna" es el lugar donde esta el codigo en cada linea del archivo
    public static string GenerarCodigo(string prefijo, string archivo, int columna)
    {
        int mayor = 0;

        foreach (string[] registro in ModuloArchivo.Leer(archivo))
        {
            string codigo = registro[columna];

            if (int.TryParse(codigo.Substring(codigo.LastIndexOf('-') + 1), out int numero) && numero > mayor)
            {
                mayor = numero;
            }
        }

        return prefijo + (mayor + 1).ToString("000");
    }

    // esto dice si ya hay una cita pendiente en ese dia y hora
    public static bool TurnoOcupado(string fecha, string hora)
    {
        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            if (NormalizarFecha(c[2]) == NormalizarFecha(fecha) && c[3] == hora && c[5] == "Pendiente") return true;
        }

        return false;
    }
}
