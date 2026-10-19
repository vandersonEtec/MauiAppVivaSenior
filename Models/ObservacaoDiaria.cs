using SQLite;

namespace MauiAppVivaSenior.Models;

public class ObservacaoDiaria
{
    [PrimaryKey, AutoIncrement]
    public int IdObservacaoDiaria { get; set; }

    public int IdosoId { get; set; }

    public DateTime Data { get; set; }

    public string Alimentacao { get; set; }

    public string Hidratacao { get; set; }

    public string Sono { get; set; }

    public string MedicacaoDiaria { get; set; }

    public string Observacoes { get; set; }
}