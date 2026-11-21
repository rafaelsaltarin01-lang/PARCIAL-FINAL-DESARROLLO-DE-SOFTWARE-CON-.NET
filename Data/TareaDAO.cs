using System.Globalization;
using GestorTareas.Models;
using Microsoft.Data.Sqlite;

namespace GestorTareas.Data;


public class TareaDAO
{
    private const string FormatoFecha = "yyyy-MM-dd";

    private readonly string _cadenaConexion;

    public TareaDAO(string rutaBaseDatos)
    {
        _cadenaConexion = new SqliteConnectionStringBuilder
        {
            DataSource = rutaBaseDatos
        }.ToString();

        InicializarBaseDatos();
    }

   
    private void InicializarBaseDatos()
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            CREATE TABLE IF NOT EXISTS Tareas (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                Titulo      TEXT NOT NULL,
                Descripcion TEXT NOT NULL DEFAULT '',
                Prioridad   TEXT NOT NULL,
                Estado      TEXT NOT NULL,
                FechaLimite TEXT NOT NULL
            );";
        comando.ExecuteNonQuery();
    }

    
    public void Insertar(Tarea tarea)
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            INSERT INTO Tareas (Titulo, Descripcion, Prioridad, Estado, FechaLimite)
            VALUES ($titulo, $descripcion, $prioridad, $estado, $fecha);
            SELECT last_insert_rowid();";
        AgregarParametros(comando, tarea);

        long idGenerado = (long)comando.ExecuteScalar()!;
        tarea.AsignarId((int)idGenerado);
    }

  
    public List<Tarea> ObtenerTodas()
    {
        var tareas = new List<Tarea>();

        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            SELECT Id, Titulo, Descripcion, Prioridad, Estado, FechaLimite
            FROM Tareas
            ORDER BY FechaLimite ASC, Id ASC;";

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            tareas.Add(new Tarea(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetString(2),
                lector.GetString(3),
                lector.GetString(4),
                DateTime.ParseExact(lector.GetString(5), FormatoFecha, CultureInfo.InvariantCulture)));
        }

        return tareas;
    }

   
    public int Actualizar(Tarea tarea)
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = @"
            UPDATE Tareas
            SET Titulo = $titulo,
                Descripcion = $descripcion,
                Prioridad = $prioridad,
                Estado = $estado,
                FechaLimite = $fecha
            WHERE Id = $id;";
        AgregarParametros(comando, tarea);
        comando.Parameters.AddWithValue("$id", tarea.Id);

        return comando.ExecuteNonQuery();
    }

  
    public int Eliminar(int id)
    {
        using var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();

        using var comando = conexion.CreateCommand();
        comando.CommandText = "DELETE FROM Tareas WHERE Id = $id;";
        comando.Parameters.AddWithValue("$id", id);

        return comando.ExecuteNonQuery();
    }

    private static void AgregarParametros(SqliteCommand comando, Tarea tarea)
    {
        comando.Parameters.AddWithValue("$titulo", tarea.Titulo);
        comando.Parameters.AddWithValue("$descripcion", tarea.Descripcion);
        comando.Parameters.AddWithValue("$prioridad", tarea.Prioridad);
        comando.Parameters.AddWithValue("$estado", tarea.Estado);
        comando.Parameters.AddWithValue("$fecha", tarea.FechaLimite.ToString(FormatoFecha, CultureInfo.InvariantCulture));
    }
}
