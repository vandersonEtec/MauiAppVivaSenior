using SQLite;

namespace MauiAppVivaSenior.Models;

public class Administrador
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Permissao { get; set; }
}