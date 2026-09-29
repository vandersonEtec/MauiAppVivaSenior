using SQLite;
using MauiAppVivaSenior.Models;

namespace MauiAppVivaSenior.Services;

// Esta classe é responsável pelo criação das tabelas e acesso ao banco de dados SQLite.

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        string caminhoBanco = Path.Combine(
            FileSystem.AppDataDirectory,
            "vivasenior.db3");

        _database = new SQLiteAsyncConnection(caminhoBanco);
    }

    public async Task InicializarBancoAsync()
    {
        await _database.CreateTableAsync<Idoso>();
        await _database.CreateTableAsync<Medicamento>();
        await _database.CreateTableAsync<Consulta>();
        await _database.CreateTableAsync<IdosoMedicamento>();
    }

    // IDOSOS
    public async Task<int> SalvarIdosoAsync(Idoso idoso)
    {
        return await _database.InsertAsync(idoso);
    }
    public async Task<int> AtualizarIdosoAsync(Idoso idoso)
    {
        return await _database.UpdateAsync(idoso);
    }

    public async Task<List<Idoso>> ListarIdososAsync()
    {
        return await _database.Table<Idoso>().ToListAsync();
    }

    //EXCLUI IDOSO E OS MEDICAMENTOS
    public async Task<int> ExcluirIdosoAsync(Idoso idoso)
    {
        var associacoes = await _database
            .Table<IdosoMedicamento>()
            .Where(im => im.IdosoId == idoso.Id)
            .ToListAsync();

        foreach (var associacao in associacoes)
        {
            await _database.DeleteAsync(associacao);
        }

        return await _database.DeleteAsync(idoso);
    }

    // MEDICAMENTOS
    public async Task<int> SalvarMedicamentoAsync(Medicamento medicamento)
    {
        return await _database.InsertAsync(medicamento);
    }

    public async Task<Medicamento> BuscarMedicamentoPorNomeAsync(
    string nome)
    {
        return await _database
            .Table<Medicamento>()
            .Where(m => m.Nome == nome)
            .FirstOrDefaultAsync();
    }

    public async Task<IdosoMedicamento> BuscarIdosoMedicamentoAsync(
    int idosoId,
    int medicamentoId)
    {
        return await _database
            .Table<IdosoMedicamento>()
            .Where(im =>
                im.IdosoId == idosoId &&
                im.MedicamentoId == medicamentoId)
            .FirstOrDefaultAsync();
    }
    public async Task<Medicamento> BuscarMedicamentoPorIdAsync(
    int medicamentoId)
    {
        return await _database
            .Table<Medicamento>()
            .Where(m => m.Id == medicamentoId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Medicamento>> ListarMedicamentosAsync(int idosoId)
    {
        var associacoes = await _database
            .Table<IdosoMedicamento>()
            .Where(im => im.IdosoId == idosoId)
            .ToListAsync();

        var medicamentos = new List<Medicamento>();

        foreach (var associacao in associacoes)
        {
            var medicamento = await _database
                .Table<Medicamento>()
                .Where(m => m.Id == associacao.MedicamentoId)
                .FirstOrDefaultAsync();

            if (medicamento != null)
            {
                medicamentos.Add(medicamento);
            }
        }

        return medicamentos;
    }

    //METODO QUE BUSCA TODOS OS MEDICAMENTOS DO CATÁLAGO
    public async Task<List<Medicamento>> ListarTodosMedicamentosAsync()
    {
        return await _database
            .Table<Medicamento>()
            .ToListAsync();
    }

    //EXCLUIR MEDICAMENTO DO IDOSO
    public async Task<int> ExcluirIdosoMedicamentoAsync(
    IdosoMedicamento idosoMedicamento)
    {
        return await _database.DeleteAsync(idosoMedicamento);
    }

    //CONSULTAS
    public async Task<int> SalvarConsultaAsync(Consulta consulta) 
    {
        return await _database.InsertAsync(consulta);
    }
    public async Task<int> AtualizarConsultaAsync(Consulta consulta)
    {
        return await _database.UpdateAsync(consulta);
    }
    public async Task<List<Consulta>> ListarConsultasAsync(int idosoId)
    { 
        return await _database
            .Table<Consulta>()
            .Where(c => c.IdosoId == idosoId)
            .ToListAsync(); 
    }
    public async Task<int> SalvarIdosoMedicamentoAsync(
    IdosoMedicamento idosoMedicamento)
    {
        return await _database.InsertAsync(idosoMedicamento);
    }

    public async Task<int> AtualizarIdosoMedicamentoAsync(
    IdosoMedicamento idosoMedicamento)
    {
        return await _database.UpdateAsync(idosoMedicamento);
    }
    public async Task<int> ExcluirConsultaAsync(Consulta consulta)
    {
        return await _database.DeleteAsync(consulta);
    }

    public async Task<List<IdosoMedicamento>> ListarIdosoMedicamentosAsync(
        int idosoId)
    {
        return await _database
            .Table<IdosoMedicamento>()
            .Where(im => im.IdosoId == idosoId)
            .ToListAsync();
    }

}