namespace Biblioteca;

// modulo de formato: cambia como se muestran los datos
static class ModuloFormato
{
    // esto muestra el DNI con asteriscos, solo se ven los ultimos 2 numeros
    public static string ProtegerDni(string dni)
    {
        return new string('*', dni.Length - 2) + dni.Substring(dni.Length - 2);
    }
}
