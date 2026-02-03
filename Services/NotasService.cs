using app_pm_back.Models;
using MongoDB.Driver;

namespace app_pm_back.Services;

public class NotasService
{
    private readonly IMongoCollection<NotaModel> _notas;
    private readonly IMongoCollection<OrdemModel> _ordens;

    public NotasService(IMongoCollection<NotaModel> notas, IMongoCollection<OrdemModel> ordens)
    {
        _notas = notas;
        _ordens = ordens;
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

    public async Task<NotaModel> UpdateNotaAsync(NotaModel nota)
    {
        try
        {
            await _notas.ReplaceOneAsync( n => n.IdMongo == nota.IdMongo, nota);
            return nota;
        }catch(Exception e ){
            Console.WriteLine($"Erro ao atualizar nota: {e.Message}");
            return null;
        }
    }
    
    public async Task<List<NotaModel>> GetAllNotasAsync(){
        return await _notas.Find(n => true).ToListAsync();
    }
    
    public async Task<NotaModel> GetNotaByIdAsync(string id){
        return await _notas.Find(n => n.IdMongo == id).FirstOrDefaultAsync();
    }
    
    public async Task<DeleteResult> DeleteNotaAsync(string id){
        return await _notas.DeleteOneAsync(n => n.IdMongo == id);
    }
    
    public async Task<long> CountNotasAsync(){
        return await _notas.CountDocumentsAsync(n => true);
    }

    public async Task<OrdemModel> CreateOrdemAsync(OrdemModel ordem)
    {
        try
        {
            var nota = await _notas.Find(n => n.IdMongo == ordem.IdNota).FirstOrDefaultAsync();

            if (nota == null)
            {
                throw new FileNotFoundException("Não foi possível encontrar a nota com este id para a ordem");
            };
            
            ordem.IdNota = nota.IdMongo;
            ordem.ObjetoReferencia.LocalInstalacao = nota.DadosGerais.LocalInstalacao;

            await _ordens.InsertOneAsync(ordem);
            return ordem;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao criar ordem: {e.Message}");
            return null;
        }
        
        
    }
    
}