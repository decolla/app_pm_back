using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace app_pm_back.Services.CongasServices;


public class DadosRenovacaoService
{
    private readonly AppDbContext _context;

    public DadosRenovacaoService(AppDbContext context)
    {
        _context = context;
    }

public async Task<int> ImportarDadosNotaAsync(
    List<long> idsNotas,
    List<long> idsItensNota)
{
    if (idsNotas == null || idsNotas.Count == 0)
        return 0;

    if (idsItensNota == null || idsItensNota.Count == 0)
        return 0;

    // 1) Busca notas e itens no banco
    var notas = await _context.PM_Notas
        .Where(n => idsNotas.Contains(n.IdNota))
        .ToListAsync();

    var itens = await _context.PM_ItensNota
        .Where(i => idsItensNota.Contains(i.IdItemNota))
        .ToListAsync();
    
        // 2) Identifica quais notas têm itens com palavras-chave de renovação
    var idsNotasRenovacao = itens
        .Where(i => ContemPalavraChaveItem(i.Causa, i.Sintoma))
        .Select(i => i.IdNota)
        .Distinct()
        .ToList();

    if (idsNotasRenovacao.Count == 0)
        return 0;

    // 3) Filtra as notas que devem ser marcadas
    var notasParaAtualizar = notas
        .Where(n => idsNotasRenovacao.Contains(n.IdNota))
        .ToList();
    
    foreach (var nota in notasParaAtualizar)
    {
        if (nota.IdAtivo.HasValue && nota.IdAtivo.Value > 0)
        {
            // Se IdAtivo está preenchido → RestauraAtivo
            nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
        }
        else if (nota.IdLi.HasValue && nota.IdLi.Value > 0)
        {
            // Se IdAtivo vazio mas IdLI preenchido → RestauraLI
            nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
        }
        // Se ambos vazios, não faz nada (deixa como estava)
    }
    
    foreach (var nota in notas)
    {
        var textosNota = $"{nota.DescricaoBreve ?? string.Empty} {nota.AnomaliaObservada ?? string.Empty} {nota.ObservacoesComplementares ?? string.Empty}".ToUpperInvariant();

        var palavrasRenovacao = new[] { "START", "RENOVAÇÃO", "RENOVACAO" };
        var palavrasTroca = new[] { "TROCA", "SUBSTITUIÇÃO", "SUBSTITUICAO" };

        bool temRenovacao = palavrasRenovacao.Any(p => textosNota.Contains(p));
        bool temTroca = palavrasTroca.Any(p => textosNota.Contains(p));

        if (temRenovacao)
        {
            nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
        }
        else if (temTroca && nota.IdAtivo.HasValue && nota.IdAtivo.Value > 0)
        {
            nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
        }
    }


    // 5) Salva
    await _context.SaveChangesAsync();

    // 6) Cadastra dados de renovação
    return await CadastraDadosRenovacaoAsync(idsNotas, ehNota: true);

}

// Helper: verifica se Causa ou Sintoma contém palavras-chave
private static bool ContemPalavraChaveItem(string? causa, string? sintoma)
{
    var texto = $"{causa ?? string.Empty} {sintoma ?? string.Empty}".ToUpperInvariant();
    
    return texto.Contains("START") 
           || texto.Contains("RENOVAÇÃO") 
           || texto.Contains("RENOVACAO");
}

    public async Task<int> ImportarDadosOrdemAsync(
    List<long> idsOrdens)
    {
        if (idsOrdens == null || idsOrdens.Count == 0)
                return 0;

        // 1) Busca ordens no banco
        var ordens = await _context.PM_Ordens
            .Where(n => idsOrdens.Contains(n.IdOrdem))
            .ToListAsync();
            
        // 2) Classifica TODAS as ordens baseado em DescricaoBreve
        foreach (var ordem in ordens)
        {
            var textoOrdem = (ordem.DescricaoBreve ?? string.Empty).ToUpperInvariant();

            var palavrasRenovacao = new[] { "TROCA", "START", "RENOVAÇÃO", "RENOVACAO" };
            var palavrasTroca = new[] {"SUBSTITUIÇÃO", "SUBSTITUICAO" };

            bool temRenovacao = palavrasRenovacao.Any(p => textoOrdem.Contains(p));
            bool temTroca = palavrasTroca.Any(p => textoOrdem.Contains(p));

            if (temRenovacao)
            {
                ordem.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
            }
            else if (temTroca && ordem.IdAtivo.HasValue && ordem.IdAtivo.Value > 0)
            {
                ordem.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
            }
        }

        // 3) Salva
        await _context.SaveChangesAsync();

        // 4) Cadastra dados de renovação
        return await CadastraDadosRenovacaoAsync(idsOrdens, ehNota: false);

    }
    
    public async Task<int> CadastraDadosRenovacaoAsync(
    List<long> ids,
    bool ehNota) // true = nota, false = ordem
{
    if (ids == null || ids.Count == 0)
        return 0;

    var registrosCriados = 0;

    if (ehNota)
    {
        var notas = await _context.PM_Notas
            .Where(n => ids.Contains(n.IdNota))
            .ToListAsync();

        foreach (var nota in notas)
        {
            var inseridos = await ProcessarPorConfiabilidadeAsync(
                nota.ConfiabilidadeRestaurada,
                nota.IdAtivo,
                nota.IdNota,
                null,
                nota.DataNota,          // passa data da nota
                OrigemUR.Nota);         // origem é Nota
            registrosCriados += inseridos;
        }
    }
    else
    {
        var ordens = await _context.PM_Ordens
            .Where(o => ids.Contains(o.IdOrdem))
            .ToListAsync();

        foreach (var ordem in ordens)
        {
            var inseridos = await ProcessarPorConfiabilidadeAsync(
                ordem.ConfiabilidadeRestaurada,
                ordem.IdAtivo,
                null,
                ordem.IdOrdem,
                ordem.DataCriacao,      // passa data da ordem
                OrigemUR.Ordem);        // origem é Ordem
            registrosCriados += inseridos;
        }
    }

    if (registrosCriados > 0)
    {
        await _context.SaveChangesAsync();
    }

    return registrosCriados;
}

private async Task<int> ProcessarPorConfiabilidadeAsync(
    ConfiabilidadeRestaurada? confiabilidade,
    int? idAtivo,
    long? idNota,
    long? idOrdem,
    DateTime dataSource,        // data da nota ou ordem
    OrigemUR origem)            // Nota ou Ordem
{
    if (confiabilidade == null)
        return 0;

    var inseridos = 0;

    if (confiabilidade == ConfiabilidadeRestaurada.RestauraAtivo)
    {
        if (idAtivo.HasValue && idAtivo.Value > 0)
        {
            inseridos = await InserirOuAtualizarPorAtivoAsync(
                idAtivo.Value,
                idNota,
                idOrdem,
                dataSource,
                origem);
        }
    }
    else if (confiabilidade == ConfiabilidadeRestaurada.RestauraLI)
    {
        // TODO
        //Busca todos os Ativos daquele LI
        //For each ativo await InserirOuAtualizarPorAtivoAsync
    }

    return inseridos;
}
//
private async Task<int> InserirOuAtualizarPorAtivoAsync(
    int idAtivo,
    long? idNota,
    long? idOrdem,
    DateTime dataSource,
    OrigemUR origem)
{
    var existente = await _context.PM_DadosRenovacao
        .FirstOrDefaultAsync(d => d.IdAtivo == idAtivo);

    if (existente == null)
    {
        var novo = new DadosRenovacaoModel
        {
            UltimaRenovacao = dataSource,
            OrigemUR = origem,
            Confiabilidade = 1.0,
            IdAtivo = idAtivo,
            IdNota = idNota ?? 0,
            IdOrdem = idOrdem ?? 0,
            IdUsuario = 0
        };
        _context.PM_DadosRenovacao.Add(novo);
        return 1;
    }
    else
    {
        if (dataSource > existente.UltimaRenovacao)
        {
            existente.UltimaRenovacao = dataSource;
            existente.OrigemUR = origem;
            if (idNota.HasValue) existente.IdNota = idNota.Value;
            if (idOrdem.HasValue) existente.IdOrdem = idOrdem.Value;
            _context.PM_DadosRenovacao.Update(existente);
            return 1;
        }
    }

    return 0;
}
    
    

}