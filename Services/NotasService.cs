using app_pm_back.Models;
using MongoDB.Driver;

namespace app_pm_back.Services;

public class NotasService
{
    private readonly IMongoCollection<NotaModel> _notas;

    // injeção no banco
    public NotasService(IMongoCollection<NotaModel> notas)
    {
        _notas = notas;
    }

    // ================================= NOTAS =====================================

    // cria uma nota e insere
    public async Task<NotaModel> CreateNotaAsync(NotaModel nota)
    {
        await _notas.InsertOneAsync(nota);
        return nota;
    }

    // atualiza uma nota
    public async Task<NotaModel> UpdateNotaAsync(NotaModel nota)
    {
        await _notas.ReplaceOneAsync(n => n.Id == nota.Id, nota);
        return nota;
    }

    // retorna todas as notas
    public async Task<List<NotaModel>>? GetAllNotasAsync()
    {
        return await _notas.Find(n => true).ToListAsync();
    }
    
    // retorna uma página de notas
    public async Task<List<NotaModel>> GetNotasPaginadasAsync(int pagina, int tamanho)
    {
        return await _notas.Find(n => true)
            .Skip((pagina - 1) * tamanho)
            .Limit(tamanho)
            .ToListAsync();
    }

    // retorna uma nota pelo id
    public async Task<NotaModel> GetNotaByIdAsync(string id)
    {
        return await _notas.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    // deleta uma nota
    public async Task<DeleteResult> DeleteNotaAsync(string id)
    {
        return await _notas.DeleteOneAsync(n => n.Id == id);
    }

    // retorna a quantidade de notas no banco
    public async Task<long> CountNotasAsync()
    {
        return await _notas.CountDocumentsAsync(n => true);
    }
}