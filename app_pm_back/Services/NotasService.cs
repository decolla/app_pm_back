using app_pm_back.Models;
using MongoDB.Driver;

namespace app_pm_back.Services;

public class NotasService
{
    private readonly IMongoCollection<NotaModel> _notas;

    public NotasService(IMongoCollection<NotaModel> notas)
    {
        _notas = notas;
    }
    
    public async Task<NotaModel> CreateNotaAsync(NotaModel nota){
        try
        {
            await _notas.InsertOneAsync(nota);
            return nota;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao criar nota: {e.Message}");
            return null;
        }
    }
}