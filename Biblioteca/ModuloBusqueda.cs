namespace Biblioteca;

// modulo de busquedas: encuentra propietarios, mascotas y citas
static class ModuloBusqueda
{
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

    // esto busca un texto en propietarios, mascotas y citas y devuelve las lineas encontradas
    public static List<string> Buscar(string texto)
    {
        texto = texto.ToLower();
        List<string> encontrados = new List<string>();

        foreach (string[] p in ModuloArchivo.Leer(ModuloArchivo.Propietarios))
        {
            if (p[0].Contains(texto) || p[1].ToLower().Contains(texto) || p[2].Contains(texto))
            {
                encontrados.Add("Propietario: " + p[1] + " | DNI: " + p[0] + " | Telefono: " + p[2]);
            }
        }

        foreach (string[] m in ModuloArchivo.Leer(ModuloArchivo.Mascotas))
        {
            if (m[1].ToLower().Contains(texto) || m[2].ToLower().Contains(texto) || m[4].ToLower().Contains(texto))
            {
                encontrados.Add("Mascota: " + m[1] + " | " + m[2] + " | Codigo: " + m[4] + " | DNI dueño: " + m[0]);
            }
        }

        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            if (c[0].Contains(texto) || c[1].ToLower().Contains(texto) || c[6].ToLower().Contains(texto) || c[7].ToLower().Contains(texto))
            {
                encontrados.Add(c[7] + ": " + c[6] + " | " + c[2] + " " + c[3] + " | " + c[1] + " | " + c[5]);
            }
        }

        return encontrados;
    }
}
