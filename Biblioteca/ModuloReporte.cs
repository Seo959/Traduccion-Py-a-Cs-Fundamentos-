namespace Biblioteca;

// modulo de reportes: arma el listado general de la clinica
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
            // ERROR 2: el DNI se muestra completo
            lineas.Add(p[1] + " | DNI: " + p[0] + " | Telefono: " + p[2]);
        }

        lineas.Add("--- Mascotas ---");
        foreach (string[] m in ModuloOrdenamiento.Ordenar(ModuloArchivo.Leer(ModuloArchivo.Mascotas), columna))
        {
            lineas.Add(m[4] + " | " + m[1] + " | " + m[2] + " | " + m[3] + " años | DNI dueño: " + m[0]);
        }

        lineas.Add("--- Citas ---");
        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            lineas.Add(c[6] + " | " + c[7] + " | " + c[2] + " " + c[3] + " | " + c[1] + " | " + c[4] + " | " + c[5]);
        }

        return lineas;
    }
}
