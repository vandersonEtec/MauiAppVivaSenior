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
    }

    // IDOSOS

    public async Task<int> SalvarIdosoAsync(Idoso idoso)
    {
        return await _database.InsertAsync(idoso);
    }

    public async Task<List<Idoso>> ListarIdososAsync()
    {
        return await _database.Table<Idoso>().ToListAsync();
    }

    // MEDICAMENTOS

    public async Task<int> SalvarMedicamentoAsync(Medicamento medicamento)
    {
        return await _database.InsertAsync(medicamento);
    }

    public async Task<List<Medicamento>> ListarMedicamentosAsync(int idosoId)
    {
        return await _database
            .Table<Medicamento>()
            .Where(m => m.IdosoId == idosoId)
            .ToListAsync();
    }
}