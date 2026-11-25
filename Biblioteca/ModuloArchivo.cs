namespace Biblioteca;

// esta clase guarda y lee los registros en archivos de texto
// cada registro es una linea y los campos van separados por |
static class ModuloArchivo
{
    // ERROR 1: la ruta es relativa, los .txt se crean donde se ejecute el programa
    public const string Propietarios = "propietarios.txt";
    public const string Mascotas = "mascotas.txt";
    public const string Citas = "citas.txt";

    // esto agrega una linea al final del archivo
    public static void Guardar(string archivo, string linea)
    {
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
