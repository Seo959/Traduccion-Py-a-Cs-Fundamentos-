namespace Biblioteca;

// modulo de ordenamiento: ordena los registros de menor a mayor
static class ModuloOrdenamiento
{
    // esto ordena con el metodo burbuja segun la columna elegida (sin importar mayusculas)
    public static List<string[]> Ordenar(List<string[]> registros, int columna)
    {
        int n = registros.Count;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (string.Compare(registros[j][columna], registros[j + 1][columna], StringComparison.OrdinalIgnoreCase) > 0)
                {
                    string[] auxiliar = registros[j];
                    registros[j] = registros[j + 1];
                    registros[j + 1] = auxiliar;
                }
            }
        }

        return registros;
    }
}
