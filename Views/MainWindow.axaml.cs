using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using GestorTareas.Data;
using GestorTareas.Models;
using GestorTareas.Services;

namespace GestorTareas.Views;


public partial class MainWindow : Window
{
    private readonly TareaServicio? _servicio;
    private Tarea? _seleccionada;

    public MainWindow()
    {
        InitializeComponent();

        CmbPrioridad.ItemsSource = TareaServicio.Prioridades;
        CmbEstado.ItemsSource = TareaServicio.Estados;

        try
        {
            
            string carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GestorTareas");
            Directory.CreateDirectory(carpeta);

            string rutaBd = Path.Combine(carpeta, "tareas.db");
            _servicio = new TareaServicio(new TareaDAO(rutaBd));

            LimpiarFormulario();
            CargarTareas();
        }
        catch (Exception ex)
        {
            Mostrar("No se pudo abrir la base de datos: " + ex.Message, esError: true);
            BtnGuardar.IsEnabled = false;
        }
    }

  

    private void BtnGuardar_Click(object? sender, RoutedEventArgs e)
    {
        if (_servicio is null) return;

        var tarea = LeerFormulario();
        if (tarea is null) return;

        try
        {
            _servicio.Crear(tarea);
            CargarTareas();
            LimpiarFormulario();
            Mostrar($"Tarea \"{tarea.Titulo}\" guardada correctamente.", esError: false);
        }
        catch (ArgumentException ex)
        {
            Mostrar(ex.Message, esError: true);
        }
        catch (Exception ex)
        {
            Mostrar("Error al guardar: " + ex.Message, esError: true);
        }
    }

    private void BtnActualizar_Click(object? sender, RoutedEventArgs e)
    {
        if (_servicio is null || _seleccionada is null) return;

        var datos = LeerFormulario();
        if (datos is null) return;

        
        var tarea = new Tarea(_seleccionada.Id, datos.Titulo, datos.Descripcion,
                              datos.Prioridad, datos.Estado, datos.FechaLimite);

        try
        {
            _servicio.Actualizar(tarea);
            CargarTareas();
            LimpiarFormulario();
            Mostrar($"Tarea \"{tarea.Titulo}\" actualizada correctamente.", esError: false);
        }
        catch (ArgumentException ex)
        {
            Mostrar(ex.Message, esError: true);
        }
        catch (Exception ex)
        {
            Mostrar("Error al actualizar: " + ex.Message, esError: true);
        }
    }

    private void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_servicio is null || _seleccionada is null) return;

        string titulo = _seleccionada.Titulo;

        try
        {
            _servicio.Eliminar(_seleccionada.Id);
            CargarTareas();
            LimpiarFormulario();
            Mostrar($"Tarea \"{titulo}\" eliminada.", esError: false);
        }
        catch (Exception ex)
        {
            Mostrar("Error al eliminar: " + ex.Message, esError: true);
        }
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e)
    {
        LimpiarFormulario();
        Mostrar(string.Empty, esError: false);
    }

    private void LstTareas_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _seleccionada = LstTareas.SelectedItem as Tarea;

        bool hayTarea = _seleccionada is not null;
        BtnActualizar.IsEnabled = hayTarea;
        BtnEliminar.IsEnabled = hayTarea;

        if (_seleccionada is not null)
        {
            TxtTitulo.Text = _seleccionada.Titulo;
            TxtDescripcion.Text = _seleccionada.Descripcion;
            CmbPrioridad.SelectedItem = _seleccionada.Prioridad;
            CmbEstado.SelectedItem = _seleccionada.Estado;
            DpFecha.SelectedDate = new DateTimeOffset(_seleccionada.FechaLimite);
        }
    }

        private Tarea? LeerFormulario()
    {
        if (DpFecha.SelectedDate is null)
        {
            Mostrar("Seleccione una fecha límite.", esError: true);
            return null;
        }

        return new Tarea(
            TxtTitulo.Text ?? string.Empty,
            TxtDescripcion.Text ?? string.Empty,
            CmbPrioridad.SelectedItem as string ?? string.Empty,
            CmbEstado.SelectedItem as string ?? string.Empty,
            DpFecha.SelectedDate.Value.DateTime);
    }

    private void CargarTareas()
    {
        if (_servicio is null) return;

        var tareas = _servicio.ObtenerTodas();
        LstTareas.ItemsSource = tareas;
        TxtContador.Text = $"Total: {tareas.Count} tarea(s)";
    }

    private void LimpiarFormulario()
    {
        LstTareas.SelectedItem = null;
        _seleccionada = null;

        TxtTitulo.Text = string.Empty;
        TxtDescripcion.Text = string.Empty;
        CmbPrioridad.SelectedItem = "Media";
        CmbEstado.SelectedItem = "Pendiente";
        DpFecha.SelectedDate = new DateTimeOffset(DateTime.Today);

        BtnActualizar.IsEnabled = false;
        BtnEliminar.IsEnabled = false;
    }

    private void Mostrar(string texto, bool esError)
    {
        TxtMensaje.Text = texto;
        TxtMensaje.Foreground = esError ? Brushes.Firebrick : Brushes.SeaGreen;
    }
}
