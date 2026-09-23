using SQLite;

namespace MauiAppVivaSenior.Models;

//Classe representa os dados do medicamento cadastrado no sistema

public class Medicamento
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int IdosoId { get; set; }

    public string Nome { get; set; }

    public string Dosagem { get; set; }

    public string Horario { get; set; }

    public string Observacao { get; set; }
}