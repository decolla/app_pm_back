using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class ItemNoteCongasService : BaseCrudService<ItemNotaManutencaoModel>
{
    public ItemNoteCongasService(AppDbContext context) : base(context)
    {
        
    }
    
    /// <summary>
    /// Retorna os itens de uma nota pelo ID
    /// </summary>
    /// <param name="id">Id da nota a ser realizada a busca</param>
    /// <returns>Lista de Itens associadas a essa nota</returns>
    public async Task<List<ItemNotaManutencaoModel>> GetItemsByNotaIdAsync(long id) // USE long ou int AQUI!
    {
        return await _dbSet.Where(a => a.NotaId == id).ToListAsync();
    }
}