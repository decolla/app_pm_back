using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe TipoPlanoService.
/// </summary>
public class TipoPlanoService : BaseCrudService<TipoPlanoModel>
{
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="TipoPlanoService"/>.
    /// </summary>
    public TipoPlanoService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="id">Identificador do registro.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<TipoPlanoModel?> GetCompletoByIdAsync(long id)
    {
        return await _context.PM_TiposPlano
            .Include(t => t.TipoEstrategia)
            .Include(t => t.TipoCep)
            .Include(t => t.TipoManutencao)
            .FirstOrDefaultAsync(t => t.IdTipoPlano == id);
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="categoria">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<List<TipoPlanoModel>> GetTodosByTipo(CategoriaTipoManutencao categoria)
    {
        return await _context.PM_TiposPlano
            .Where(t => t.Categoria == categoria)
            .ToListAsync();
    }

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    /// <param name="novo">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<TipoPlanoModel> CreateComCopiaAsync(TipoPlanoModel novo)
    {
        if (novo.IdTipoEstrategia != null)
        {

            var tipoEstrategia = await _context.PM_TiposEstrategia
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTipoEstrategia == novo.IdTipoEstrategia);
            
            if (tipoEstrategia == null)
            {
                throw new Exception("Tipo de estratégia informado não encontrado.");
            }
            
            AplicarDadosTipoEstrategia(novo, tipoEstrategia);

            return await CreateAsync(novo);
        }
        

        if (novo.IdTipoManutencao == null)
        {
            var tipoManutencao = await _context.PM_TiposManutencao
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTipoManutencao == novo.IdTipoManutencao);
            
            if (tipoManutencao == null)
            {
                throw new Exception("Tipo de manutenção informado não encontrado.");
            }
            
            AplicarDadosTipoManutencao(novo, tipoManutencao);
            
            return await CreateAsync(novo);
        }
        
        throw new Exception("É necessário informar o IdTipoEstrategia ou IdTipoManutencao para criar um TipoPlano.");
    }

    private static void AplicarDadosTipoEstrategia(TipoPlanoModel destino, TipoEstrategiaModel origem)
    {
        destino.Dias = origem.Dias;
        destino.ToleranciaPositiva = origem.ToleranciaPositiva;
        destino.ToleranciaNegativa = origem.ToleranciaNegativa;
        destino.Continua = origem.Continua;
        destino.Categoria = origem.Categoria;
        destino.IdTipoCep = origem.IdTipoCep;
        destino.IdTipoEstrategia = origem.IdTipoEstrategia;
        destino.IdTipoManutencao = origem.IdTipoManutencao;
    }
    
    private static void AplicarDadosTipoManutencao(TipoPlanoModel destino, TipoManutencaoModel origem)
    {
        destino.Categoria = origem.Categoria;
        
        destino.IdTipoManutencao = origem.IdTipoManutencao;
    }

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    /// <param name="atualizado">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    /// <exception cref="System.Exception">Lançada em caso de erro interno ou falha no processamento.</exception>
    public async Task<TipoPlanoModel> UpdateComListasAsync(TipoPlanoModel atualizado)
    {
        var existente = await GetCompletoByIdAsync(atualizado.IdTipoPlano);
        if (existente == null) throw new Exception("Registo não encontrado");

        _context.Entry(existente).CurrentValues.SetValues(atualizado);
        await _context.SaveChangesAsync();
        return existente;
    }
}