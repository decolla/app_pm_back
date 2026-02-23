using app_pm_back.Models;
using MongoDB.Driver;

namespace app_pm_back.Services;

public class OrdemService
{
    private readonly IMongoCollection<NotaModel> _notas;
    private readonly IMongoCollection<OrdemModel> _ordens;

    // injeção no banco
    public OrdemService(IMongoCollection<OrdemModel> ordens, IMongoCollection<NotaModel> notas)
    {
        _notas = notas;
        _ordens = ordens;
    }

    // ================================= ORDENS ====================================

    // cria uma ordem e insere
    public async Task<OrdemModel> CreateOrdemAsync(OrdemModel ordem)
    {
        // procura a nota de origem
        var nota = await _notas.Find(n => n.Id == ordem.IdNota).FirstOrDefaultAsync();

        if (nota == null)
        {
            throw new FileNotFoundException("Não foi possível encontrar a nota com este id para a ordem");
        }

        // passa os parâmetros da nota para a ordem
        ordem.IdNota = nota.Id;
        ordem.StatusNota = nota.DadosGerais.Status;
        ordem.ObjetoReferencia.LocalInstalacao = nota.DadosGerais.LocalInstalacao;
        ordem.ObjetoReferencia.TAG = nota.DadosGerais.Tag;
        ordem.Responsabilidades.DataAvaria = nota.DataAvaria;
        ordem.Database.InicioDesejado = nota.CaracteristicasGerais.DataNota;
        ordem.Database.ConclDesejado = nota.CaracteristicasGerais.FimDesejado;

        // insere a ordem
        await _ordens.InsertOneAsync(ordem);

        nota.IdOrdem = ordem.Id;
        nota.DadosGerais.Status = "ORDEM_GERADA";
        nota.NroOrdem = ordem.Id;

        await _notas.ReplaceOneAsync(n => n.Id == nota.Id, nota);

        return ordem;
    }

    // atualiza uma ordem
    public async Task<OrdemModel> UpdateOrdemAsync(OrdemModel ordem)
    {
        // procura a ordem de origem
        var ordemOriginal = await _ordens.Find(n => n.Id == ordem.Id).FirstOrDefaultAsync();
        if (ordemOriginal == null)
        {
            throw new FileNotFoundException("Não foi possível encontrar a ordem com este id");
        }

        // atualiza a ordem
        await _ordens.ReplaceOneAsync(n => n.Id == ordem.Id, ordem);
        return ordem;
    }

    // retorna todas as ordens 
    public async Task<List<OrdemModel>> GetAllOrdensAsync()
    {
        return await _ordens.Find(n => true).ToListAsync();
    }
    
    // retorna uma página de ordens
    public async Task<List<OrdemModel>> GetOrdensPaginadasAsync(int pagina, int tamanho)
    {
        return await _ordens.Find(n => true)
            .Skip((pagina - 1) * tamanho)
            .Limit(tamanho)
            .ToListAsync();
    }

    // retorna uma ordem pelo id
    public async Task<OrdemModel> GetOrdemByIdAsync(string id)
    {
        return await _ordens.Find(n => n.Id == id).FirstOrDefaultAsync();
    }

    // deleta uma ordem
    public async Task<DeleteResult> DeleteOrdemAsync(string id)
    {
        return await _ordens.DeleteOneAsync(n => n.Id == id);
    }

    // retorna a quantidade de ordens no banco
    public async Task<long> CountOrdensAsync()
    {
        return await _ordens.CountDocumentsAsync(n => true);
    }
}