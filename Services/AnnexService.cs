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
    /// <param name="notaId">Id da nota associada.</param>
    /// <param name="ordemId">Id da ordem associada.</param>
    /// <param name="tipoReferencia">Tipo da referencia.</param>
    /// <param name="categoria">Categoria do anexo.</param>
    /// <returns>Anexo salvo.</returns>
    public async Task<AnnexModel> UploadAnexo(IFormFile file, long? notaId, long? ordemId, string tipoReferencia, CategoriaAnexo categoria)
    {
        ValidateAssociacao(notaId, ordemId);

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        AnnexModel annexModel = new AnnexModel
        {
            NomeAnexo = file.FileName,
            MimeType = file.ContentType,
            Dados = stream.ToArray(),
            NotaId = notaId,
            OrdemId = ordemId,
            TipoReferencia = tipoReferencia,
            Categoria = categoria,
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
    /// <param name="notaId">Id da nota associada.</param>
    /// <param name="ordemId">Id da ordem associada.</param>
    /// <param name="tipoReferencia">Tipo da referencia.</param>
    /// <returns>Lista de pares id/nome do anexo.</returns>
    public async Task<List<(long, string)>> BuscarAnexo(long? notaId, long? ordemId, string tipoReferencia)
    {
        ValidateAssociacao(notaId, ordemId);

        var query = _annexModels.AsNoTracking()
            .Where(o => o.TipoReferencia == tipoReferencia && (o.NotaId == notaId || o.OrdemId == ordemId));

        return await query
            .Select(model => new ValueTuple<long, string>(model.IdAnexo, model.NomeAnexo))
            .ToListAsync();
    }

    private static void ValidateAssociacao(long? notaId, long? ordemId)
    {
        if ((notaId.HasValue && ordemId.HasValue) || (!notaId.HasValue && !ordemId.HasValue))
        {
            throw new ArgumentException("Informe apenas NotaId ou OrdemId para o anexo.");
        }
    }
}