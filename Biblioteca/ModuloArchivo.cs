namespace Biblioteca;

// esta clase guarda y lee los registros en archivos de texto
// cada registro es una linea y los campos van separados por |
static class ModuloArchivo
{
    // los archivos siempre se guardan en la carpeta "datos" junto al programa
    public static readonly string Carpeta = Path.Combine(AppContext.BaseDirectory, "datos");

    public static readonly string Propietarios = Path.Combine(Carpeta, "propietarios.txt");
    public static readonly string Mascotas = Path.Combine(Carpeta, "mascotas.txt");
    public static readonly string Citas = Path.Combine(Carpeta, "citas.txt");

    // esto agrega una linea al final del archivo (crea la carpeta si no existe)
    public static void Guardar(string archivo, string linea)
    {
        Directory.CreateDirectory(Carpeta);
        File.AppendAllText(archivo, linea + "\n");
    }

    // esto lee el archivo y devuelve cada linea separada en campos
    public static List<string[]> Leer(string archivo)
    {
        List<string[]> registros = new List<string[]>();

        if (!File.Exists(archivo))
        {
            return registros;
        }

        foreach (string linea in File.ReadAllLines(archivo))
        {
            if (linea.Trim() != "")
            {
                registros.Add(linea.Split('|'));
            }
        }

        return registros;
    }
}
