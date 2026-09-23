using SQLite;

namespace MauiAppVivaSenior.Models;


//Classe representa os dados de um idoso cadastrado no sistema

public class Idoso
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; }

    public DateTime DataNascimento { get; set; }

    public string Telefone { get; set; }

    public string Endereco { get; set; }
}