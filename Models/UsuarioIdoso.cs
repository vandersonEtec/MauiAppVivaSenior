
using SQLite;

namespace MauiAppVivaSenior.Models;

public class UsuarioIdoso
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int IdosoId { get; set; }
}
