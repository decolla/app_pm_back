using app_pm_back.Data;
using app_pm_back.DTO;
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
    public virtual async Task<object> UploadAnexo(IFormFile file, long? notaId, long? ordemId)
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
                TipoAnexo = file.ContentDisposition
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

    /// <summary>
    /// Busca anexos por critérios específicos.
    /// </summary>
    /// <param name="notaId">ID da nota associada.</param>
    /// <param name="ordemId">ID da ordem associada.</param>
    /// <param name="categoria">Categoria do anexo.</param>
    /// <returns>Lista de anexos encontrados.</returns>
    public virtual async Task<List<AnexoDto>> BuscarAnexo(long? notaId, long? ordemId, CategoriaAnexo categoria)
    {
        ValidateAssociacao(notaId, ordemId);

        if (categoria == CategoriaAnexo.Nota)
        {
            if (notaId.HasValue)
            {
                return await _context.PM_AnexosNota
                    .Where(a => a.IdNota == notaId.Value)
                    .Select(a => new AnexoDto
                    {
                        IdAnexo = a.IdAnexoNota,
                        Nome = a.Nome
                    })
                    .ToListAsync();
            }
            throw new ArgumentException("O ID da nota é obrigatório para o tipo de anexo Nota.");
        }
        else if(categoria == CategoriaAnexo.Ordem)
        {
            if (ordemId.HasValue)
            {
                return await _context.PM_AnexosOrdem
                    .Where(a => a.IdOrdem == ordemId.Value)
                    .Select(a => new AnexoDto
                    {
                        IdAnexo = a.IdAnexoOrdem,
                        Nome = a.Nome
                    })
                    .ToListAsync();
            }
            throw new ArgumentException("O ID da ordem é obrigatório para o tipo de anexo Ordem.");
        }
        throw new ArgumentException("Categoria de anexo inválida.");
    }

    private static void ValidateAssociacao(long? notaId, long? ordemId)
    {
        if ((notaId.HasValue && ordemId.HasValue) || (!notaId.HasValue && !ordemId.HasValue))
        {
            throw new ArgumentException("Informe apenas NotaId ou OrdemId para o anexo.");
        }
    }

    /// <summary>
    /// Deleta um anexo pelo seu identificador.
    /// </summary>
    public virtual async Task<bool> DeleteAnexo(long id)
    {
        var anexoNota = await _context.PM_AnexosNota.FindAsync(id);
        if (anexoNota != null)
        {
            _context.PM_AnexosNota.Remove(anexoNota);
            await _context.SaveChangesAsync();
            return true;
        }

        var anexoOrdem = await _context.PM_AnexosOrdem.FindAsync(id);
        if (anexoOrdem != null)
        {
            _context.PM_AnexosOrdem.Remove(anexoOrdem);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Atualiza um anexo pelo seu identificador.
    /// </summary>
    public virtual async Task<bool> UpdateAnexo(long id, string? nome, CategoriaAnexo? categoria)
    {
        var anexoNota = await _context.PM_AnexosNota.FindAsync(id);
        if (anexoNota != null)
        {
            if (nome != null && nome.Length > 0)
            {
                await using var stream = new MemoryStream();
                anexoNota.Nome = nome;
                anexoNota.Dados = stream.ToArray();
            }

            _context.PM_AnexosNota.Update(anexoNota);
            await _context.SaveChangesAsync();
            return true;
        }

        var anexoOrdem = await _context.PM_AnexosOrdem.FindAsync(id);
        if (anexoOrdem != null)
        {
            if (nome != null && nome.Length > 0)
            {
                await using var stream = new MemoryStream();
                anexoOrdem.Nome = nome;
                anexoOrdem.Dados = stream.ToArray();
            }

            if (categoria.HasValue)
            {
                anexoOrdem.TipoAnexo = categoria.Value.ToString();
            }

            _context.PM_AnexosOrdem.Update(anexoOrdem);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}