using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class ItemNoteCongasService : BaseCrudService<ItemNotaCongasModel>
{
    public ItemNoteCongasService(AppDbContext context) : base(context)
    {
        
    }
    
    /// <summary>
    /// Retorna os itens de uma nota pelo ID
    /// </summary>
    /// <param name="id">Id da nota a ser realizada a busca</param>
    /// <returns>Lista de Itens associadas a essa nota</returns>
    public async Task<List<ItemNotaCongasModel>> GetItemsByNotaIdAsync(long id) // USE long ou int AQUI!
    {
        return await _dbSet.Where(a => a.NoteCongasIdNota == id).ToListAsync();
    }
}