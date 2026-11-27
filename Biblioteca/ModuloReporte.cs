using System.Text;

namespace Biblioteca;

// modulo de reportes: arma el listado general y lo exporta
static class ModuloReporte
{
    // esto arma las lineas del reporte, las mascotas salen ordenadas por la columna elegida
    // columna 4 = codigo, columna 1 = nombre
    public static List<string> GenerarReporte(int columna)
    {
        List<string> lineas = new List<string>();
        lineas.Add("REPORTE - CLINICA DR. LEO");

        lineas.Add("--- Propietarios ---");
        foreach (string[] p in ModuloArchivo.Leer(ModuloArchivo.Propietarios))
        {
            lineas.Add(p[1] + " | DNI: " + ModuloFormato.ProtegerDni(p[0]) + " | Telefono: " + p[2]);
        }

        lineas.Add("--- Mascotas ---");
        foreach (string[] m in ModuloOrdenamiento.Ordenar(ModuloArchivo.Leer(ModuloArchivo.Mascotas), columna))
        {
            lineas.Add(m[4] + " | " + m[1] + " | " + m[2] + " | " + m[3] + " años | DNI dueño: " + ModuloFormato.ProtegerDni(m[0]));
        }

        lineas.Add("--- Citas ---");
        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            lineas.Add(c[6] + " | " + c[7] + " | " + c[2] + " " + c[3] + " | " + c[1] + " | " + c[4] + " | " + c[5]);
        }

        return lineas;
    }

    // esto guarda el reporte en datos/reporte.txt y devuelve la ruta
    public static string ExportarTexto(int columna)
    {
        Directory.CreateDirectory(ModuloArchivo.Carpeta);
        string ruta = Path.Combine(ModuloArchivo.Carpeta, "reporte.txt");
        File.WriteAllLines(ruta, GenerarReporte(columna));
        return ruta;
    }

    // esto guarda las mascotas con su dueño en datos/expedientes.csv y devuelve la ruta
    public static string ExportarCsv()
    {
        List<string> lineas = new List<string>();
        lineas.Add("codigo,mascota,especie,edad,propietario,dni,telefono");

        foreach (string[] m in ModuloOrdenamiento.Ordenar(ModuloArchivo.Leer(ModuloArchivo.Mascotas), 4))
        {
            string[] p = ModuloBusqueda.BuscarPropietario(m[0]);

            // ERROR 9: si un nombre lleva coma, las columnas del CSV se desordenan
            lineas.Add(string.Join(",", m[4], m[1], m[2], m[3], p[1], ModuloFormato.ProtegerDni(m[0]), p[2]));
        }

        Directory.CreateDirectory(ModuloArchivo.Carpeta);
        string ruta = Path.Combine(ModuloArchivo.Carpeta, "expedientes.csv");
        File.WriteAllLines(ruta, lineas, new UTF8Encoding(true));
        return ruta;
    }
}
