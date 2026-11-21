namespace GestorTareas.Models;


public class Tarea
{
    private string _titulo = string.Empty;
    private string _descripcion = string.Empty;

    public int Id { get; private set; }

    public string Titulo
    {
        get => _titulo;
        set => _titulo = (value ?? string.Empty).Trim();
    }

    public string Descripcion
    {
        get => _descripcion;
        set => _descripcion = (value ?? string.Empty).Trim();
    }

    public string Prioridad { get; set; }

    public string Estado { get; set; }

    public DateTime FechaLimite { get; set; }

    
    public Tarea(string titulo, string descripcion, string prioridad, string estado, DateTime fechaLimite)
        : this(0, titulo, descripcion, prioridad, estado, fechaLimite)
    {
    }

    
    public Tarea(int id, string titulo, string descripcion, string prioridad, string estado, DateTime fechaLimite)
    {
        Id = id;
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad;
        Estado = estado;
        FechaLimite = fechaLimite.Date;
    }

    
    public void AsignarId(int id)
    {
        Id = id;
    }

   
    public bool EstaVencida()
    {
        return Estado != "Completada" && FechaLimite.Date < DateTime.Today;
    }

    
    public string FechaTexto =>
        FechaLimite.ToString("dd/MM/yyyy") + (EstaVencida() ? " (vencida)" : string.Empty);

    public override string ToString()
    {
        return $"[{Id}] {Titulo} - {Prioridad} - {Estado} - {FechaLimite:dd/MM/yyyy}";
    }
}
