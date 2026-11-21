
CREATE TABLE IF NOT EXISTS Tareas (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Titulo      TEXT NOT NULL,
    Descripcion TEXT NOT NULL DEFAULT '',
    Prioridad   TEXT NOT NULL,   
    Estado      TEXT NOT NULL,   a
    FechaLimite TEXT NOT NULL    
);
