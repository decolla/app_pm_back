using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Models.Domain.Diretorios;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class DiretorioService
{
    private readonly AppDbContext _context;

    public DiretorioService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna a árvore completa 
    /// </summary>
    /// <returns>Lista de nós da árvore</returns>
    public async Task<List<DiretorioDto>> ObterArvoreAsync()
    {
        // todos os diretórios da árvore
        var todosDiretorios = await _context.PM_Diretorios
            .Include(d => d.NotasVinculadas).ThenInclude(dn => dn.Nota)
            .Include(d => d.OrdensVinculadas).ThenInclude(do_ => do_.Ordem)
            .ToListAsync();

        // gera um DTO de envio
        var dicionarioDtos = todosDiretorios.ToDictionary(
            d => d.IdDiretorio,
            d => new DiretorioDto
            {
                Id = d.IdDiretorio,
                Nome = d.Nome,
                IdPai = d.IdDiretorioPai,
                // mapeia folhas para o DTO
                Notas = d.NotasVinculadas.Select(dn => new FolhaDto
                    { Id = dn.IdNota, Nome = dn.Nota.DescricaoBreve, Tipo = "Nota" }).ToList(),
                Ordens = d.OrdensVinculadas.Select(do_ => new FolhaDto
                    { Id = do_.IdOrdem, Nome = do_.Ordem.DescricaoBreve, Tipo = "Ordem" }).ToList()
            });

        // monta a árvore completa
        var raizes = new List<DiretorioDto>();

        foreach (var dto in dicionarioDtos.Values)
        {
            if (dto.IdPai.HasValue && dicionarioDtos.ContainsKey(dto.IdPai.Value))
            {
                // se tiver um pai
                dicionarioDtos[dto.IdPai.Value].Subdiretorios.Add(dto);
            }
            else
            {
                // caso não tenha um pai
                raizes.Add(dto);
            }
        }

        return raizes;
    }

    /// <summary>
    /// Adiciona uma nota ao diretório
    /// </summary>
    /// <param name="idDiretorio">Id do diretório</param>
    /// <param name="idNota">Id da nota</param>
    public async Task VincularNotaAoDiretorioAsync(long idDiretorio, long idNota)
    {
        var existe = await _context.Set<DiretorioNotaModel>()
            .AnyAsync(dn => dn.IdDiretorio == idDiretorio && dn.IdNota == idNota);

        if (!existe)
        {
            try
            {
                await _context.PM_Notas.FindAsync(idNota);

                _context.Set<DiretorioNotaModel>().Add(new DiretorioNotaModel
                    { IdDiretorio = idDiretorio, IdNota = idNota });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao vincular nota ao diretório: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Adiciona uma ordem ao diretório 
    /// </summary>
    /// <param name="idDiretorio">Id do diretório</param>
    /// <param name="idOrdem">Id da ordem</param>
    public async Task VincularOrdemAoDiretorioAsync(long idDiretorio, long idOrdem)
    {
        var existe = await _context.Set<DiretorioOrdemModel>()
            .AnyAsync(do_ => do_.IdDiretorio == idDiretorio && do_.IdOrdem == idOrdem);

        if (!existe)
        {
            try
            {
                await _context.PM_Ordens.FindAsync(idOrdem);
                
                _context.Set<DiretorioOrdemModel>().Add(new DiretorioOrdemModel
                    { IdDiretorio = idDiretorio, IdOrdem = idOrdem });
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao vincular ordem ao diretório: {ex.Message}", ex);
            }
        }
    }
}