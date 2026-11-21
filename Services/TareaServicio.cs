using GestorTareas.Data;
using GestorTareas.Models;

namespace GestorTareas.Services;


public class TareaServicio
{
    public static readonly string[] Prioridades = { "Alta", "Media", "Baja" };
    public static readonly string[] Estados = { "Pendiente", "En progreso", "Completada" };

    private const int MaxTitulo = 80;
    private const int MaxDescripcion = 300;

    private readonly TareaDAO _dao;

    public TareaServicio(TareaDAO dao)
    {
        _dao = dao;
    }

    public List<Tarea> ObtenerTodas()
    {
        return _dao.ObtenerTodas();
    }

    public void Crear(Tarea tarea)
    {
        // Una tarea nueva no puede tener fecha límite en el pasado.
        Validar(tarea, validarFechaFutura: true);
        _dao.Insertar(tarea);
    }

    public void Actualizar(Tarea tarea)
    {
        Validar(tarea, validarFechaFutura: false);

        int filas = _dao.Actualizar(tarea);
        if (filas == 0)
        {
            throw new InvalidOperationException("La tarea ya no existe en la base de datos.");
        }
    }

    public void Eliminar(int id)
    {
        int filas = _dao.Eliminar(id);
        if (filas == 0)
        {
            throw new InvalidOperationException("La tarea ya no existe en la base de datos.");
        }
    }

    /// <summary>Lanza ArgumentException con un mensaje comprensible si hay datos incorrectos.</summary>
    private static void Validar(Tarea tarea, bool validarFechaFutura)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(tarea.Titulo))
            errores.Add("El título es obligatorio.");
        else if (tarea.Titulo.Length > MaxTitulo)
            errores.Add($"El título no puede superar {MaxTitulo} caracteres.");

        if (tarea.Descripcion.Length > MaxDescripcion)
            errores.Add($"La descripción no puede superar {MaxDescripcion} caracteres.");

        if (!Prioridades.Contains(tarea.Prioridad))
            errores.Add("Seleccione una prioridad válida.");

        if (!Estados.Contains(tarea.Estado))
            errores.Add("Seleccione un estado válido.");

        if (validarFechaFutura && tarea.FechaLimite.Date < DateTime.Today)
            errores.Add("La fecha límite no puede ser anterior a hoy.");

        if (errores.Count > 0)
        {
            throw new ArgumentException(string.Join(" ", errores));
        }
    }
}
