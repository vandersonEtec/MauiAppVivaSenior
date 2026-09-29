using SQLite;

namespace MauiAppVivaSenior.Models;

public class IdosoMedicamento
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int IdosoId { get; set; }

    public int MedicamentoId { get; set; }

    public string Dosagem { get; set; } = string.Empty;

    public string Horario { get; set; } = string.Empty;

    public string Observacao { get; set; } = string.Empty;
}