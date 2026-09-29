using SQLite;

namespace MauiAppVivaSenior.Models;

public class Consulta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int IdosoId { get; set; }

    public string Especialidade { get; set; }

    public string Medico { get; set; }

    public DateTime Data { get; set; }

    public string Horario { get; set; }

    public string Observacao { get; set; }
}
