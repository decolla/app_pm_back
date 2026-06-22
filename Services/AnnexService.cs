using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;
/// <summary>
/// Classe AnnexService.
/// </summary>

public class AnnexService
{
    private readonly AppDbContext _context;
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="AnnexService"/>.
    /// </summary>

    public AnnexService(AppDbContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="file">Arquivo a ser armazen.</param>
    /// <param name="tipoReferencia">Parâmetro do método.</param>
    /// <param name="categoria">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>

    public virtual async Task<object> UploadAnexo(IFormFile file, long? notaId, long? ordemId, string tipoReferencia, CategoriaAnexo categoria)
    {
        ValidateAssociacao(notaId, ordemId);

        await using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        if (notaId.HasValue)
        {
            var anexo = new AnexoNotaModel
            {
                Nome = file.FileName,
                TipoArquivo = file.ContentType,
                Dados = stream.ToArray(),
                IdNota = notaId.Value
            };
            _context.PM_AnexosNota.Add(anexo);
            await _context.SaveChangesAsync();
            return new { IdAnexo = anexo.IdAnexoNota, Nome = anexo.Nome };
        }
        else
        {
            var anexo = new AnexoOrdemModel
            {
                Nome = file.FileName,
                TipoArquivo = file.ContentType,
                Dados = stream.ToArray(),
                IdOrdem = ordemId!.Value,
                TipoAnexo = categoria.ToString()
            };
            _context.PM_AnexosOrdem.Add(anexo);
            await _context.SaveChangesAsync();
            return new { IdAnexo = anexo.IdAnexoOrdem, Nome = anexo.Nome };
        }
    }

    public virtual async Task<(byte[] Dados, string MimeType, string Nome)?> DownloadAnexo(long id)
    {
        var anexoNota = await _context.PM_AnexosNota.FindAsync(id);
        if (anexoNota != null && anexoNota.Dados != null)
        {
            return (anexoNota.Dados, anexoNota.TipoArquivo, anexoNota.Nome);
        }

        var anexoOrdem = await _context.PM_AnexosOrdem.FindAsync(id);
        if (anexoOrdem != null && anexoOrdem.Dados != null)
        {
            return (anexoOrdem.Dados, anexoOrdem.TipoArquivo, anexoOrdem.Nome);
        }

        return null;
    }

    public virtual async Task<List<(long, string)>> BuscarAnexo(long? notaId, long? ordemId, string tipoReferencia)
    {
        ValidateAssociacao(notaId, ordemId);

        if (notaId.HasValue)
        {
            return await _context.PM_AnexosNota
                .Where(a => a.IdNota == notaId.Value)
                .Select(a => new ValueTuple<long, string>((long)a.IdAnexoNota, a.Nome))
                .ToListAsync();
        }
        else
        {
            return await _context.PM_AnexosOrdem
                .Where(a => a.IdOrdem == ordemId!.Value)
                .Select(a => new ValueTuple<long, string>((long)a.IdAnexoOrdem, a.Nome))
                .ToListAsync();
        }
    }

    private static void ValidateAssociacao(long? notaId, long? ordemId)
    {
        if ((notaId.HasValue && ordemId.HasValue) || (!notaId.HasValue && !ordemId.HasValue))
        {
            throw new ArgumentException("Informe apenas NotaId ou OrdemId para o anexo.");
        }
    }
}