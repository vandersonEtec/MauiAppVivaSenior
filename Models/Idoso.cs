using SQLite;

namespace MauiAppVivaSenior.Models;

public class Idoso
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; }

    public DateTime DataNascimento { get; set; }

    public string CPF { get; set; }

    public string Telefone { get; set; }

    public string Endereco { get; set; }

    public string Observacoes { get; set; }

    [Ignore]
    public int Idade
    {
        get
        {
            var hoje = DateTime.Today;

            var idade = hoje.Year - DataNascimento.Year;

            if (DataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            return idade;
        }
    }
}