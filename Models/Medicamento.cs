using SQLite;

namespace MauiAppVivaSenior.Models;

public class Medicamento
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; }
}