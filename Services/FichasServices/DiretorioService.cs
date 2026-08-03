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
                    { Id = dn.IdNota, Nome = dn.Nota?.DescricaoBreve, Tipo = "Nota" }).ToList(),
                Ordens = d.OrdensVinculadas.Select(do_ => new FolhaDto
                    { Id = do_.IdOrdem, Nome = do_.Ordem?.DescricaoBreve, Tipo = "Ordem" }).ToList()
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
    public async Task<DiretorioModel> VincularNotaAoDiretorioAsync(long idDiretorio, long idNota)
    {
        // verica se já existe este diretório 
        var existe = await _context.Set<DiretorioNotaModel>()
            .AnyAsync(dn => dn.IdDiretorio == idDiretorio && dn.IdNota == idNota);

        if (!existe)
        {
            try
            {
                // verifica se a nota e o diretorio existem
                var nota = await _context.PM_Notas.FindAsync(idNota);
                if (nota == null) throw new KeyNotFoundException($"Nota com ID {idNota} não encontrada no sistema.");
                
                var diretorio = await _context.PM_Diretorios.FindAsync(idDiretorio);
                if (diretorio == null) throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");
                
                // vincula uma nota 
                _context.Set<DiretorioNotaModel>().Add(new DiretorioNotaModel
                    { IdDiretorio = idDiretorio, IdNota = idNota });
                await _context.SaveChangesAsync();
                
                return diretorio;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao vincular nota ao diretório: {ex.Message}", ex);
            }
        }

        throw new Exception($"A nota com ID {idNota} já está vinculada ao diretório com ID {idDiretorio}.");
    }

    /// <summary>
    /// Adiciona uma ordem ao diretório 
    /// </summary>
    /// <param name="idDiretorio">Id do diretório</param>
    /// <param name="idOrdem">Id da ordem</param>
    public async Task<DiretorioModel> VincularOrdemAoDiretorioAsync(long idDiretorio, long idOrdem)
    {
        var existe = await _context.Set<DiretorioOrdemModel>()
            .AnyAsync(do_ => do_.IdDiretorio == idDiretorio && do_.IdOrdem == idOrdem);

        if (!existe)
        {
            try
            {
                // verifica se a ordem e o diretorio existem
                var ordem = await _context.PM_Ordens.FindAsync(idOrdem);
                if (ordem == null) throw new KeyNotFoundException($"Ordem com ID {idOrdem} não encontrada.");

                var diretorio = await _context.PM_Diretorios.FindAsync(idDiretorio);
                if (diretorio == null) throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");

                _context.Set<DiretorioOrdemModel>().Add(new DiretorioOrdemModel
                    { IdDiretorio = idDiretorio, IdOrdem = idOrdem });
                await _context.SaveChangesAsync();
                
                return diretorio;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao vincular ordem ao diretório: {ex.Message}", ex);
            }
        }
        
        throw new Exception($"A ordem com ID {idOrdem} já está vinculada ao diretório com ID {idDiretorio}.");
    }
    
    /// <summary>
    /// Método auxiliar para evitar loops infinitos ao mover diretórios. Verifica se o novo pai é descendente do diretório original.
    /// </summary>
    /// <param name="idDiretorioOriginal">ID do diretório original</param>
    /// <param name="idNovoPai">ID do novo pai</param>
    /// <returns></returns>
    private async Task<bool> VerificarSePaiEDescendente(long idDiretorioOriginal, long idNovoPai)
    {
        long? paiAtual = idNovoPai;

        // Sobe na árvore a partir do novo pai para ver se esbarra no diretório original
        while (paiAtual.HasValue)
        {
            if (paiAtual.Value == idDiretorioOriginal)
                return true; // Encontrou um ciclo!

            var diretorioAcima = await _context.PM_Diretorios
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.IdDiretorio == paiAtual.Value);
            
            paiAtual = diretorioAcima?.IdDiretorioPai;
        }

        return false;
    }

    public async Task<DiretorioModel> CriarDiretorioAsync(string nome, long? idDiretorioPai = null)
    {
        var novoDiretorio = new DiretorioModel
        {
            Nome = nome,
            IdDiretorioPai = idDiretorioPai
        };

        _context.PM_Diretorios.Add(novoDiretorio);
        await _context.SaveChangesAsync();

        return novoDiretorio;
    }

    public async Task<DiretorioModel?> ObterDiretorioAsync(long idDiretorio)
    {
        var diretorio = await _context.PM_Diretorios
            .Include(d => d.Subdiretorios)
            .Include(d => d.NotasVinculadas)
            .Include(d => d.OrdensVinculadas)
            .FirstOrDefaultAsync(d => d.IdDiretorio == idDiretorio);

        return diretorio;
    }

    public async Task<List<FolhaDto>> ObterFolhasDoDiretorioAsync(long idDiretorio)
    {
        var diretorio = await _context.PM_Diretorios
            .Include(d => d.NotasVinculadas).ThenInclude(dn => dn.Nota)
            .Include(d => d.OrdensVinculadas).ThenInclude(do_ => do_.Ordem)
            .FirstOrDefaultAsync(d => d.IdDiretorio == idDiretorio);

        if (diretorio == null)
        {
            throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");
        }

        var folhas = new List<FolhaDto>();

        folhas.AddRange(diretorio.NotasVinculadas.Select(dn => new FolhaDto
            { Id = dn.IdNota, Nome = dn.Nota?.DescricaoBreve, Tipo = "Nota" }));

        folhas.AddRange(diretorio.OrdensVinculadas.Select(do_ => new FolhaDto
            { Id = do_.IdOrdem, Nome = do_.Ordem?.DescricaoBreve, Tipo = "Ordem" }));

        return folhas;
    }

    public async Task RemoverNotaDoDiretorioAsync(long idDiretorio, long idNota)
    {
        var vinculo = await _context.Set<DiretorioNotaModel>()
            .FirstOrDefaultAsync(dn => dn.IdDiretorio == idDiretorio && dn.IdNota == idNota);

        if (vinculo != null)
        {
            _context.Set<DiretorioNotaModel>().Remove(vinculo);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoverOrdemDoDiretorioAsync(long idDiretorio, long idOrdem)
    {
        var vinculo = await _context.Set<DiretorioOrdemModel>()
            .FirstOrDefaultAsync(do_ => do_.IdDiretorio == idDiretorio && do_.IdOrdem == idOrdem);

        if (vinculo != null)
        {
            _context.Set<DiretorioOrdemModel>().Remove(vinculo);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Realiza a remoção de um diretório do banco
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser removido</param>
    public async Task RemoverDiretorioAsync(long idDiretorio)
    {
        await PrepararRemocaoRecursivaAsync(idDiretorio);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Função auxiliar de remoção recursiva
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser removido</param>
    private async Task PrepararRemocaoRecursivaAsync(long idDiretorio)
    {
        var diretorio = await _context.PM_Diretorios
            .Include(d => d.Subdiretorios)
            .Include(d => d.NotasVinculadas)
            .Include(d => d.OrdensVinculadas)
            .FirstOrDefaultAsync(d => d.IdDiretorio == idDiretorio);

        if (diretorio != null)
        {
            // remove os vínculos de notas e ordens
            _context.Set<DiretorioNotaModel>().RemoveRange(diretorio.NotasVinculadas);
            _context.Set<DiretorioOrdemModel>().RemoveRange(diretorio.OrdensVinculadas);

            // desce nas subpastas recursivamente
            foreach (var subdiretorio in diretorio.Subdiretorios.ToList())
            {
                await PrepararRemocaoRecursivaAsync(subdiretorio.IdDiretorio);
            }

            // marca a pasta atual para ser apagada
            _context.PM_Diretorios.Remove(diretorio);
        }
    }

    /// <summary>
    /// Atualiza o nome do diretório
    /// </summary>
    /// <param name="idDiretorio"></param>
    /// <param name="nome"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<DiretorioModel> AtualizarDiretorioAsync(long idDiretorio, string nome)
    {
        var diretorio = await ObterDiretorioAsync(idDiretorio);
        if (diretorio == null)
        {
            throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");
        }

        diretorio.Nome = nome;
        await _context.SaveChangesAsync();
        return diretorio;
    }

    /// <summary>
    /// Obtém todos os diretórios cadastrados em bancos
    /// </summary>
    /// <returns></returns>
    public async Task<List<DiretorioModel>> ObterTodosDiretoriosAsync()
    {
        return await _context.PM_Diretorios
            .Include(d => d.Subdiretorios)
            .Include(d => d.NotasVinculadas)
            .Include(d => d.OrdensVinculadas)
            .ToListAsync();
    }

    /// <summary>
    /// Move o diretório
    /// </summary>
    /// <param name="idDiretorio">ID do diretório a ser movido</param>
    /// <param name="idDiretorioPai">Diretório de destino</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<DiretorioModel> MoverDiretorioAsync(long idDiretorio, long? idDiretorioPai)
    {
        if (idDiretorio == idDiretorioPai)
            throw new InvalidOperationException("Um diretório não pode ser movido para dentro dele mesmo.");

        var diretorio = await ObterDiretorioAsync(idDiretorio);

        // bloqueio de referência circular
        if (idDiretorioPai.HasValue)
        {
            bool isDescendente = await VerificarSePaiEDescendente(idDiretorio, idDiretorioPai.Value);
            if (isDescendente)
                throw new InvalidOperationException("Não é possível mover um diretório para dentro de uma das suas próprias subpastas.");
        }

        diretorio.IdDiretorioPai = idDiretorioPai;
 
        await _context.SaveChangesAsync();
        
        return diretorio;
    }

    /// <summary>
    /// Retorna o caminho de um diretório 
    /// </summary>
    /// <param name="idDiretorio">Id do diretório que será gerado o caminho</param>
    /// <returns></returns>
    public async Task<string> GetCaminhoAsync(long idDiretorio)
    {
        var diretorio = await ObterDiretorioAsync(idDiretorio);
        string caminho = "";
        if (diretorio == null) return caminho;

        while (true)
        {
            if (diretorio == null) break;
            // preenche o caminho
            caminho = $"{diretorio.Nome}/{caminho}";
            if (diretorio.IdDiretorioPai == null) break;
            // procura o pai
            diretorio = await ObterDiretorioAsync(diretorio.IdDiretorioPai.Value);
        }

        return caminho;
    }
}