namespace Biblioteca;

// modulo de edicion: cancela citas y elimina mascotas
static class ModuloEdicion
{
    // esto cambia el estado de una cita a Cancelada usando su codigo
    public static string CancelarCita(string codigo)
    {
        List<string[]> citas = ModuloArchivo.Leer(ModuloArchivo.Citas);

        foreach (string[] c in citas)
        {
            if (c[6].ToLower() == codigo.ToLower())
            {
                if (c[5] == "Cancelada") return "Esa cita ya esta cancelada";

                c[5] = "Cancelada";
                ModuloArchivo.Reescribir(ModuloArchivo.Citas, citas);
                return "Cita cancelada";
            }
        }

        return "No existe una cita con ese codigo";
    }

    // esto elimina una mascota, solo si no tiene citas registradas
    public static string EliminarMascota(string dni, string nombre)
    {
        if (ModuloBusqueda.BuscarMascota(dni, nombre) == null)
        {
            return "No se encontro esa mascota";
        }

        foreach (string[] c in ModuloArchivo.Leer(ModuloArchivo.Citas))
        {
            if (c[0] == dni && c[1].ToLower() == nombre.ToLower())
            {
                return "No se puede eliminar: la mascota tiene citas registradas";
            }
        }

        List<string[]> mascotas = ModuloArchivo.Leer(ModuloArchivo.Mascotas);
        mascotas.RemoveAll(m => m[0] == dni && m[1].ToLower() == nombre.ToLower());
        ModuloArchivo.Reescribir(ModuloArchivo.Mascotas, mascotas);
        return "Mascota eliminada";
    }
}
