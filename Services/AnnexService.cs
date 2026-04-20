using app_pm_back.Data;
using app_pm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Servico de persistencia e recuperacao de anexos.
/// </summary>
public class AnnexService
{
    /// <summary>
    /// Contexto de banco de dados.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Conjunto de anexos.
    /// </summary>
    private readonly DbSet<AnnexModel> _annexModels;

    /// <summary>
    /// Inicializa uma nova instancia de <see cref="AnnexService"/>.
    /// </summary>
    /// <param name="context">Contexto da aplicacao.</param>
    public AnnexService(AppDbContext context)
    {
        _context = context;
        _annexModels = context.Set<AnnexModel>();
    }

    /// <summary>
    /// Faz upload e salva um anexo no banco.
    /// </summary>
    /// <param name="file">Arquivo enviado.</param>
    /// <param name="referenciaId">Id da entidade referenciada.</param>
    /// <param name="tipoReferencia">Tipo da referencia.</param>
    /// <returns>Anexo salvo.</returns>
    public async Task<AnnexModel> UploadAnexo(IFormFile file, long referenciaId, string tipoReferencia)
    {
        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        AnnexModel annexModel = new AnnexModel
        {
            NomeAnexo = file.FileName,
            MimeType = file.ContentType,
            Dados = stream.ToArray(),
            ReferenciaId = referenciaId,
            TipoReferencia = tipoReferencia,
            DataUpload = DateTime.Now
        };

        _annexModels.Add(annexModel);
        await _context.SaveChangesAsync();

        return annexModel;
    }

    /// <summary>
    /// Busca um anexo por id.
    /// </summary>
    /// <param name="id">Id do anexo.</param>
    /// <returns>Anexo encontrado ou nulo.</returns>
    public async Task<AnnexModel?> DownloadAnexo(long id)
    {
        var annexModel = await _annexModels.FindAsync(id);
        return annexModel;
    }

    /// <summary>
    /// Lista anexos por referencia e tipo.
    /// </summary>
    /// <param name="referenciaId">Id da referencia.</param>
    /// <param name="tipoReferencia">Tipo da referencia.</param>
    /// <returns>Lista de pares id/nome do anexo.</returns>
    public async Task<List<(long, string)>> BuscarAnexo(long referenciaId, string tipoReferencia)
    {
        List<(long, string)> list = new List<(long, string)>();

        var query = await _annexModels.Where(o => o.ReferenciaId == referenciaId &&
                                                  o.TipoReferencia == tipoReferencia).ToListAsync();

        foreach (var model in query)
        {
            list.Add((model.IdAnexo, model.NomeAnexo));   
        }
        
        return list;
    }
}