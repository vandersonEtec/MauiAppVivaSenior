
using SQLite;
using MauiAppVivaSenior.Models;

namespace MauiAppVivaSenior.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

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
        await _database.CreateTableAsync<Usuario>();
        await _database.CreateTableAsync<Administrador>();
        await _database.CreateTableAsync<ObservacaoDiaria>();
        await _database.CreateTableAsync<UsuarioIdoso>();

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Idoso ADD COLUMN Genero TEXT");

            await _database.ExecuteAsync(
                "ALTER TABLE Idoso ADD COLUMN ContatoEmergencia TEXT");
        }
        catch
        {
            // As colunas já existem.
        }

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Medicamento ADD COLUMN ViaAdministracao TEXT");

            await _database.ExecuteAsync(
                "ALTER TABLE Medicamento ADD COLUMN Status TEXT");
        }
        catch
        {
            // As colunas já existem.
        }

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Consulta ADD COLUMN Local TEXT");

            await _database.ExecuteAsync(
                "ALTER TABLE Consulta ADD COLUMN Documentos TEXT");
        }
        catch
        {
            // As colunas já existem.
        }
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
        return await _database
            .Table<Idoso>()
            .ToListAsync();
    }

    public async Task<int> ExcluirIdosoAsync(Idoso idoso)
    {
        var medicamentos = await _database
            .Table<IdosoMedicamento>()
            .Where(im => im.IdosoId == idoso.Id)
            .ToListAsync();

        foreach (var associacao in medicamentos)
        {
            await _database.DeleteAsync(associacao);
        }

        var vinculos = await _database
            .Table<UsuarioIdoso>()
            .Where(v => v.IdosoId == idoso.Id)
            .ToListAsync();

        foreach (var vinculo in vinculos)
        {
            await _database.DeleteAsync(vinculo);
        }

        var consultas = await _database
            .Table<Consulta>()
            .Where(c => c.IdosoId == idoso.Id)
            .ToListAsync();

        foreach (var consulta in consultas)
        {
            await _database.DeleteAsync(consulta);
        }

        var observacoes = await _database
            .Table<ObservacaoDiaria>()
            .Where(o => o.IdosoId == idoso.Id)
            .ToListAsync();

        foreach (var observacao in observacoes)
        {
            await _database.DeleteAsync(observacao);
        }

        return await _database.DeleteAsync(idoso);
    }

    // MEDICAMENTOS

    public async Task<int> SalvarMedicamentoAsync(
        Medicamento medicamento)
    {
        return await _database.InsertAsync(medicamento);
    }

    public async Task<int> AtualizarMedicamentoAsync(
        Medicamento medicamento)
    {
        return await _database.UpdateAsync(medicamento);
    }

    public async Task<Medicamento> BuscarMedicamentoPorNomeAsync(
        string nome)
    {
        return await _database
            .Table<Medicamento>()
            .Where(m => m.Nome == nome)
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

    public async Task<List<Medicamento>> ListarMedicamentosAsync(
        int idosoId)
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

    public async Task<List<Medicamento>> ListarTodosMedicamentosAsync()
    {
        return await _database
            .Table<Medicamento>()
            .ToListAsync();
    }

    public async Task<int> ExcluirIdosoMedicamentoAsync(
        IdosoMedicamento idosoMedicamento)
    {
        return await _database.DeleteAsync(idosoMedicamento);
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

    public async Task<List<IdosoMedicamento>> ListarIdosoMedicamentosAsync(
        int idosoId)
    {
        return await _database
            .Table<IdosoMedicamento>()
            .Where(im => im.IdosoId == idosoId)
            .ToListAsync();
    }

    // CONSULTAS

    public async Task<int> SalvarConsultaAsync(Consulta consulta)
    {
        return await _database.InsertAsync(consulta);
    }

    public async Task<int> AtualizarConsultaAsync(Consulta consulta)
    {
        return await _database.UpdateAsync(consulta);
    }

    public async Task<List<Consulta>> ListarConsultasAsync(
        int idosoId)
    {
        return await _database
            .Table<Consulta>()
            .Where(c => c.IdosoId == idosoId)
            .ToListAsync();
    }

    public async Task<int> ExcluirConsultaAsync(Consulta consulta)
    {
        return await _database.DeleteAsync(consulta);
    }

    // OBSERVAÇÕES DIÁRIAS

    public async Task<int> SalvarObservacaoDiariaAsync(
        ObservacaoDiaria observacao)
    {
        return await _database.InsertAsync(observacao);
    }

    public async Task<int> AtualizarObservacaoDiariaAsync(
        ObservacaoDiaria observacao)
    {
        return await _database.UpdateAsync(observacao);
    }

    public async Task<List<ObservacaoDiaria>> ListarObservacoesDiariasAsync(
        int idosoId)
    {
        return await _database
            .Table<ObservacaoDiaria>()
            .Where(o => o.IdosoId == idosoId)
            .OrderByDescending(o => o.Data)
            .ToListAsync();
    }

    public async Task<int> ExcluirObservacaoDiariaAsync(
        ObservacaoDiaria observacao)
    {
        return await _database.DeleteAsync(observacao);
    }

    // USUÁRIOS

    public async Task<int> SalvarUsuarioAsync(Usuario usuario)
    {
        return await _database.InsertAsync(usuario);
    }

    public async Task<int> AtualizarUsuarioAsync(Usuario usuario)
    {
        return await _database.UpdateAsync(usuario);
    }

    public async Task<List<Usuario>> ListarUsuariosAsync()
    {
        return await _database
            .Table<Usuario>()
            .ToListAsync();
    }

    public async Task<Usuario> BuscarUsuarioPorEmailAsync(
        string email)
    {
        return await _database
            .Table<Usuario>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<Usuario> BuscarUsuarioPorIdAsync(
        int usuarioId)
    {
        return await _database
            .Table<Usuario>()
            .Where(u => u.Id == usuarioId)
            .FirstOrDefaultAsync();
    }

    // ADMINISTRADORES

    public async Task<int> SalvarAdministradorAsync(
        Administrador administrador)
    {
        return await _database.InsertAsync(administrador);
    }

    public async Task<int> AtualizarAdministradorAsync(
        Administrador administrador)
    {
        return await _database.UpdateAsync(administrador);
    }

    public async Task<List<Administrador>> ListarAdministradoresAsync()
    {
        return await _database
            .Table<Administrador>()
            .ToListAsync();
    }

    public async Task<Administrador> BuscarAdministradorPorUsuarioAsync(
        int usuarioId)
    {
        return await _database
            .Table<Administrador>()
            .Where(a => a.UsuarioId == usuarioId)
            .FirstOrDefaultAsync();
    }

    // VÍNCULO ENTRE USUÁRIOS E IDOSOS

    public async Task<int> SalvarUsuarioIdosoAsync(
        UsuarioIdoso vinculo)
    {
        var existente = await _database
            .Table<UsuarioIdoso>()
            .Where(v =>
                v.UsuarioId == vinculo.UsuarioId &&
                v.IdosoId == vinculo.IdosoId)
            .FirstOrDefaultAsync();

        if (existente != null)
            return 0;

        return await _database.InsertAsync(vinculo);
    }

    public async Task<List<Idoso>> ListarIdososPorUsuarioAsync(
        int usuarioId)
    {
        var vinculos = await _database
            .Table<UsuarioIdoso>()
            .Where(v => v.UsuarioId == usuarioId)
            .ToListAsync();

        var idosos = new List<Idoso>();

        foreach (var vinculo in vinculos)
        {
            var idoso = await _database
                .Table<Idoso>()
                .Where(i => i.Id == vinculo.IdosoId)
                .FirstOrDefaultAsync();

            if (idoso != null)
                idosos.Add(idoso);
        }

        return idosos;
    }

    public async Task<List<UsuarioIdoso>> ListarVinculosPorIdosoAsync(
        int idosoId)
    {
        return await _database
            .Table<UsuarioIdoso>()
            .Where(v => v.IdosoId == idosoId)
            .ToListAsync();
    }

    public async Task<int> ExcluirVinculoUsuarioIdosoAsync(
        UsuarioIdoso vinculo)
    {
        return await _database.DeleteAsync(vinculo);
    }
}
