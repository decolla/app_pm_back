using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.DTO.Buscas;
using app_pm_back.Models.Domain;
using app_pm_back.Models.Domain.Diretorios;
using app_pm_back.Services.Extensions;
using AutoMapper.Internal;
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
                if (diretorio == null)
                    throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");

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
                if (diretorio == null)
                    throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");

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

    // public async Task<List<object>> ObterFichasDiretorio(long idDiretorio, Fichas tipoFicha, dynamic busca)
    // {
    //     var diretorio = await _context.PM_Diretorios
    //         .Include(d => d.NotasVinculadas).ThenInclude(dn => dn.Nota)
    //         .Include(d => d.OrdensVinculadas).ThenInclude(do_ => do_.Ordem)
    //         .ThenInclude(ordemModel => ordemModel.Nota)
    //         .FirstOrDefaultAsync(d => d.IdDiretorio == idDiretorio);
    //
    //     if (diretorio == null)
    //     {
    //         throw new KeyNotFoundException($"Diretório com ID {idDiretorio} não encontrado.");
    //     }
    //
    //     var notasQuery = diretorio.NotasVinculadas.Select(n => n.Nota).AsQueryable();
    //     var ordensQuery = diretorio.OrdensVinculadas.Select(o => o.Ordem).AsQueryable();
    //
    //     string? sapCode = buscaType.GetProperty("SapName")?.GetValue(busca) as string;
    //     if (!string.IsNullOrEmpty(sapCode))
    //     {
    //         notasQuery = notasQuery.Where(n => n.SapCode == sapCode);
    //         ordensQuery = ordensQuery.Where(o => o.Nota != null && o.Nota.SapCode == sapCode);
    //     }
    //
    //     string? dnsIot = buscaType.GetProperty("DnsIot")?.GetValue(busca) as string;
    //     if (!string.IsNullOrEmpty(dnsIot))
    //     {
    //         notasQuery = notasQuery.Where(n => n.DnsIot == dnsIot);
    //         ordensQuery = ordensQuery.Where(o => o.DnsIot == dnsIot);
    //     }
    //
    //     if (true)
    //     {
    //         // notasQuery = notasQuery.Where(n => n.IdNota == busca.NumeroRegistro.Value);
    //         // ordensQuery = ordensQuery.Where(o => o.NumeroRegistro == busca.NumeroRegistro.Value);
    //     }
    //
    //     if (buscaType.GetProperty("Componente")?.GetValue(busca) is ComponentType componente)
    //     {
    //         notasQuery = notasQuery.Where(n => n.IdsAtivo.Contains(componente));
    //         ordensQuery = ordensQuery.Where(o => o.IdsAtivo.Contains(componente));
    //     }
    //
    //     var folhas = new List<object>();
    //
    //     folhas.AddRange(diretorio.NotasVinculadas.Where(n =>
    //         n.Nota.SapCode.Contains(busca.SapName)).Select(dn => new
    //     {
    //         Id = dn.IdNota,
    //         NumeroFicha = dn.Nota?.NumeroNota,
    //         SapName = dn.Nota?.SapCode,
    //         DnsIot = dn.Nota?.DnsIot,
    //         Componente = dn.Nota?.IdsAtivo,
    //         Tipo = "Nota"
    //     }));
    //
    //     folhas.AddRange(diretorio.OrdensVinculadas.Select(do_ => new
    //     {
    //         Id = do_.IdOrdem,
    //         NumeroFicha = do_.Ordem?.NumeroOrdem,
    //         SapName = "N/A",
    //         DnsIot = do_.Ordem?.DnsIot,
    //         Componente = do_.Ordem?.IdsAtivo,
    //         Tipo = "Ordem"
    //     }));
    //
    //     return folhas;
    // }


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
                throw new InvalidOperationException(
                    "Não é possível mover um diretório para dentro de uma das suas próprias subpastas.");
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

    /// <summary>
    /// Filtra as notas e ordens de um diretório folha com base nos filtros passados
    /// </summary>
    /// <param name="idDiretorio">Id do diretório folha</param>
    /// <param name="pagina">Pagina a ser exibida</param>
    /// <param name="tamanho">Tamanho da página</param>
    /// <param name="busca"></param>
    /// <param name="tipo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<object> ObterFichasDiretorioFiltradasAsync(
        long idDiretorio,
        int? pagina,
        int? tamanho,
        dynamic? busca,
        Fichas tipo
    )
    {
        if (busca == null)
        {
            throw new ArgumentNullException(nameof(busca));
        }

        var dadosQuery = _context.PM_Diretorios
            .Include(d => d.Subdiretorios)
            .AsNoTracking()
            .AsQueryable();

        if (tipo == Fichas.Notas)
        {
            dadosQuery = dadosQuery
                .Include(d => d.NotasVinculadas)
                .AsNoTracking()
                .AsQueryable();
        }
        else if (tipo == Fichas.Ordens)
        {
            dadosQuery = dadosQuery
                .Include(d => d.OrdensVinculadas)
                .AsNoTracking()
                .AsQueryable();
        }
        else
        {
            dadosQuery = dadosQuery
                .Include(d => d.NotasVinculadas)
                .Include(d => d.OrdensVinculadas)
                .AsNoTracking()
                .AsQueryable();
        }

        // safe check
        var buscaType = (Type)busca.GetType();

        string? sapName = buscaType.GetProperty("SapName")?.GetValue(busca);
        if (!string.IsNullOrEmpty(sapName))
        {
            dadosQuery = dadosQuery.Where(n =>
                n.NotasVinculadas.Any(dn => dn.Nota.SapCode != null && dn.Nota.SapCode.Contains(sapName)));
        }

        string? dnsIot = buscaType.GetProperty("DnsIot")?.GetValue(busca);
        if (!string.IsNullOrEmpty(dnsIot))
        {
            dadosQuery =
                dadosQuery.Where(n => n.NotasVinculadas.Any(dn => dn.Nota.DnsIot == dnsIot));
            dadosQuery =
                dadosQuery.Where(n => n.OrdensVinculadas.Any(dn => dn.Ordem.DnsIot == dnsIot));
        }

        string? nroFicha = buscaType.GetProperty("NroFicha")?.GetValue(busca);
        if (!string.IsNullOrEmpty(nroFicha))
        {
            dadosQuery =
                dadosQuery.Where(n => n.NotasVinculadas.Any(dn => dn.Nota.NumeroNota.ToString() == nroFicha));
            dadosQuery =
                dadosQuery.Where(n => n.OrdensVinculadas.Any(dn => dn.Ordem.NumeroOrdem.ToString() == nroFicha));
        }

        ComponentType? componente = buscaType.GetProperty("Componente")?.GetValue(busca) as ComponentType?;
        if (componente.HasValue)
        {
            dadosQuery = dadosQuery.Where(n =>
                n.NotasVinculadas.Any(dn => dn.Nota.IdsAtivo != null && dn.Nota.IdsAtivo.Contains(componente.Value)));
            dadosQuery = dadosQuery.Where(n =>
                n.OrdensVinculadas.Any(dn =>
                    dn.Ordem.IdsAtivo != null && dn.Ordem.IdsAtivo.Contains(componente.Value)));
        }

        dadosQuery = dadosQuery.AplicarOrdenacao(_context);

        var notas = await dadosQuery.SelectMany(d => d.NotasVinculadas).Select(dn => dn.Nota).ToListAsync()
            .ConfigureAwait(false);
        var ordens = await dadosQuery.SelectMany(d => d.OrdensVinculadas).Select(do_ => do_.Ordem).ToListAsync()
            .ConfigureAwait(false);

        List<object> listaUnica = new([]);

        listaUnica.AddRange(notas);
        listaUnica.AddRange(ordens);

        // paginação
        int totalGeral = listaUnica.Count;
        int p = pagina ?? 1;
        int t = tamanho ?? 20;
        int totalPaginas = (int)Math.Ceiling((double)totalGeral / t);

        // pega os n primeiros itens
        var itensPaginados = listaUnica.Skip((p - 1) * t).Take(t).ToList();

        return new
        {
            Total = totalGeral,
            TotalPaginas = totalPaginas,
            pagina = p,
            Tamanho = t,
            Result = itensPaginados
        };
    }

    public async Task<List<OrdemModel>> ObterOrdensDiretorioFiltradasAsync(long id)
    {
        var diretorio = await ObterDiretorioAsync(id);
        return diretorio?.OrdensVinculadas.Select(do_ => do_.Ordem).ToList() ?? new List<OrdemModel>();
    }
}