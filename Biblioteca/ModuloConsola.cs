namespace Biblioteca;

// modulo de consola: lee lo que escribe el usuario
static class ModuloConsola
{
    // esto pide un texto, no deja que quede vacio ni que lleve el caracter |
    public static string LeerTexto(string mensaje)
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
}
