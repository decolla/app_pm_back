using app_pm_back.Data;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using app_pm_back.Models.Domain.Diretorios;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe OrdemService.
/// </summary>
public class OrdemService : BaseCrudService<OrdemModel>
{
    DiretorioService _diretorioService;
    
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="OrdemService"/>.
    /// </summary>
    public OrdemService(AppDbContext context, DiretorioService diretorioService) : base(context)
    {
        _diretorioService = diretorioService;
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="ordens">Parâmetro do método.</param>
    public virtual async Task ImportarOrdensEmLoteSeguroAsync(List<OrdemModel> ordens, bool sobrescrever,
        bool adicionarNovas, CancellationToken cancellationToken = default)
    {
        if (ordens == null || !ordens.Any()) return;

        var notaIds = ordens
            .Where(o => o.IdNota.HasValue)
            .Select(o => o.IdNota!.Value)
            .Distinct()
            .ToList();

        await ImportGhostHelper.EnsureNotasAsync(_context, notaIds.Select(id => (long?)id));

        var numerosOrdens = ordens
            .Where(o => o.NumeroOrdem.HasValue)
            .Select(o => o.NumeroOrdem!.Value)
            .Distinct()
            .ToList();

        var idsOrdens = ordens
            .Where(o => !o.NumeroOrdem.HasValue)
            .Select(o => o.IdOrdem)
            .Distinct()
            .ToList();

        var existentesPorNumero = numerosOrdens.Any()
            ? await _context.PM_Ordens
                .Where(o => o.NumeroOrdem.HasValue && numerosOrdens.Contains(o.NumeroOrdem.Value))
                .ToDictionaryAsync(o => o.NumeroOrdem!.Value)
            : new Dictionary<long, OrdemModel>();

        var existentesPorId = idsOrdens.Any()
            ? await _context.PM_Ordens
                .Where(o => idsOrdens.Contains(o.IdOrdem))
                .ToDictionaryAsync(o => o.IdOrdem)
            : new Dictionary<long, OrdemModel>();

        var novasOrdens = new List<OrdemModel>();
        foreach (var ordem in ordens)
        {
            cancellationToken.ThrowIfCancellationRequested();

            OrdemModel? existente = null;

            if (ordem.NumeroOrdem.HasValue)
            {
                existentesPorNumero.TryGetValue(ordem.NumeroOrdem.Value, out existente);
            }
            else
            {
                existentesPorId.TryGetValue(ordem.IdOrdem, out existente);
            }

            if (existente != null)
            {
                if (sobrescrever)
                {
                    _context.Entry(existente).CurrentValues.SetValues(ordem);
                }
            }
            else if (adicionarNovas)
            {
                novasOrdens.Add(ordem);
            }
        }

        if (novasOrdens.Any())
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _context.PM_Ordens.AddRangeAsync(novasOrdens);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Obtém um registro específico.
    /// </summary>
    /// <param name="ativo">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<object> GetFichasPorAtivoAsync(ComponentType ativo, string? tipoOrdem)
    {
        var query = _dbSet.AsQueryable();

        query = query.Where(n => n.Nota != null && n.Nota.IdAtivo == ativo);

        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.FimReal ?? n.FimPlanejado ?? DateTime.UtcNow,
            Tipo = n.Classificacao
        }).ToListAsync();

        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }

    /// <summary>
    /// Obtém os usuários e grupos pertencentes a uma ordem.
    /// </summary>
    /// <param name="idOrdem">Id da ordem a ser realizada a busca</param>
    /// <returns>GPM, CTM, e usuários dos respetivos grupos</returns>
    public async Task<Object> GetGruposResponsaveis(long idOrdem)
    {
        OrdemModel? ordem = await _context.PM_Ordens
            .Include(o => o.GPM)
            .Include(o => o.CTM)
            .FirstOrDefaultAsync(o => o.IdOrdem == idOrdem);

        if (ordem == null) return new object();

        System.Collections.IEnumerable usuariosGpm = new List<object>();
        if (ordem.IdGpm.HasValue)
        {
            usuariosGpm = await _context.PM_Usuarios
                .Where(u => u.Gpms.Any(g => g.IdGPM == ordem.IdGpm.Value))
                .Select(u => new { u.IdUsuario, u.Nome })
                .ToListAsync();
        }

        System.Collections.IEnumerable usuariosCtm = new List<object>();
        if (ordem.IdCtm.HasValue)
        {
            usuariosCtm = await _context.PM_Usuarios
                .Where(u => u.Ctms.Any(c => c.IdCTM == ordem.IdCtm.Value))
                .Select(u => new { u.IdUsuario, u.Nome })
                .ToListAsync();
        }

        return new
        {
            GPM = ordem.GPM,
            UsuariosGPM = usuariosGpm,
            CTM = ordem.CTM,
            UsuariosCTM = usuariosCtm
        };
    }

    /// <summary>
    /// Atualiza uma nota por completo com base no DTO da tela do frontend.
    /// </summary>
    /// <param name="ordemTela">Nota completa</param>
    /// <returns>Nota atualizada</returns>
    public async Task<OrdemModel> AtualizarOrdemCompletaAsync(OrdemModel ordemTela)
    {
        // limpa o change tracker
        _context.ChangeTracker.Clear();

        // busca a ordem no banco de dados
        var ordemDoBanco = await _dbSet
            .Include(n => n.Operacoes)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.IdOrdem == ordemTela.IdOrdem);

        if (ordemDoBanco == null)
            throw new Exception("Ordem não encontrada.");

        // atualiza o cabeçalho da ordem
        _context.Entry(ordemDoBanco).CurrentValues.SetValues(ordemTela);
        _context.Entry(ordemDoBanco).State = EntityState.Modified;

        // sincroniza operações
        var idOperacoes = ordemTela.Operacoes.Select(v => v.IdOperacao).ToList();

        // deleta
        foreach (var opBanco in ordemDoBanco.Operacoes.Where(v => !idOperacoes.Contains(v.IdOperacao)))
        {
            _context.Entry(opBanco).State = EntityState.Deleted;
        }

        foreach (var opTela in ordemTela.Operacoes)
        {
            if (opTela.IdOperacao == 0)
            {
                // caso de nova variável
                opTela.IdOrdem = ordemDoBanco.IdOrdem;
                _context.Entry(opTela).State = EntityState.Added;
            }
            else
            {
                // caso de update
                _context.Entry(opTela).State = EntityState.Modified;
            }
        }

        // salva bb
        await _context.SaveChangesAsync();

        return ordemDoBanco;
    }

    /// <summary>
    /// Gera um payload de ordem a partir de uma nota.
    /// </summary>
    /// <param name="idNota">Id da nota de referência</param>
    /// <returns>Ordem payload</returns>
    /// <exception cref="ArgumentNullException">Nota não existe</exception>
    public async Task<OrdemModel> ImportarOrdemAsync(long idNota)
    {
        // limpa o change tracker
        _context.ChangeTracker.Clear();

        NotaModel? nota = _context.PM_Notas.FindAsync(idNota).Result;

        if (nota == null) throw new ArgumentNullException($"Nota com id {idNota} não encontrada.");

        // gera um payload de ordem a partir da nota
        OrdemModel ordem = new OrdemModel
        {
            IdNota = nota.IdNota,
            IdLi = nota.IdLi,
            DnsIot = nota.DnsIot,
            Classificacao = nota.Classificacao,
            MetodoClassificacao = nota.MetodoClassificacao,
            ConfiabilidadeRestaurada = nota.ConfiabilidadeRestaurada,
            IdAtivo = nota.IdAtivo,
            IdGpm = nota.IdGpm,
            FimPlanejado = nota.FimDesejado,
        };

        return ordem;
    }
    
    /// <summary>
    /// Retorna todas as pastas onde uma Ordem específica está guardada
    /// </summary>
    public async Task<List<DiretorioDto>> ObterDiretoriosDaOrdemAsync(long idOrdem)
    {
        // puxa dados do diretorio
        var diretorios = await _context.Set<DiretorioOrdemModel>()
            .Where(dn => dn.IdOrdem == idOrdem)
            .Select(dn => dn.Diretorio) // puxa a pasta associada
            .ToListAsync();

        // mapeia o dto
        return diretorios.Select(d => new DiretorioDto
        {
            Id = d.IdDiretorio,
            Nome = d.Nome,
            IdPai = d.IdDiretorioPai
        }).ToList();
    }

    public async Task<object?> GetOrdemCompletaAsync(long idOrdem)
    {
        var ordem = await _dbSet
            .Include(n => n.Operacoes)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.IdOrdem == idOrdem);

        if (ordem == null) return null;
        
        // obter o diretório da ordem
        var diretorio = await ObterDiretoriosDaOrdemAsync(idOrdem);
        long? idDiretorio = diretorio.FirstOrDefault()?.Id;
        
        // obtém o caminho em string do diretório
        var caminhoDiretorio = await _diretorioService.GetCaminhoAsync(idDiretorio ?? 0);

        return new
        {
            Ordem = ordem,
            Diretorio = diretorio,
            CaminhoDiretorio = caminhoDiretorio
        };
    }
}