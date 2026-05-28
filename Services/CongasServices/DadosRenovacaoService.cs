using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace app_pm_back.Services.CongasServices;


public class DadosRenovacaoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DadosRenovacaoService> _logger;

    public DadosRenovacaoService(AppDbContext context, ILogger<DadosRenovacaoService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
//Faz tratamento em caso de notas +itens
public async Task<int> ImportarDadosNotaAsync(
    List<long> idsNotas,
    List<long> idsItensNota)
{
    _logger.LogInformation("Iniciando ImportarDadosNotaAsync com {QtdNotas} notas e {QtdItens} itens", 
        idsNotas?.Count ?? 0, idsItensNota?.Count ?? 0);
    
    if (idsNotas == null || idsNotas.Count == 0)
    {
        _logger.LogWarning("Lista de ids notas vazia ou nula");
        return 0;
    }

    if (idsItensNota == null || idsItensNota.Count == 0)
    {
        _logger.LogWarning("Lista de ids itens vazia ou nula");
        return 0;
    }

    try
    {
        //Busca notas e itens no banco
        var notas = await _context.PM_Notas
            .Where(n => idsNotas.Contains(n.IdNota))
            .ToListAsync();
        
        _logger.LogInformation("Carregadas {QtdNotas} notas do banco", notas.Count);

        var itens = await _context.PM_ItensNota
            .Where(i => idsItensNota.Contains(i.IdItemNota))
            .ToListAsync();

        _logger.LogInformation("Carregados {QtdItens} itens do banco", itens.Count);

        //Identifica as notas que possuem itens com palavras-chave de renovação
        var idsNotasRenovacao = itens
            .Where(i => ContemPalavraChaveItem(i.Causa, i.Sintoma))
            .Select(i => i.IdNota)
            .Distinct()
            .ToList();
        
        _logger.LogInformation("Identificadas {QtdNotas} notas com itens de renovação", idsNotasRenovacao.Count);

        if (idsNotasRenovacao.Count == 0)
        {
            _logger.LogWarning("Nenhuma nota com palavras-chave de renovação encontrada");
            return 0;
        }

        //Filtra as notas que devem ser marcadas com base nos ids retornados anteriormente
        var notasParaAtualizar = notas
            .Where(n => idsNotasRenovacao.Contains(n.IdNota))
            .ToList();

        //para cada nota indentificada, verifica se ela renova ativo ou LI
        foreach (var nota in notasParaAtualizar)
        {
            if (nota.IdAtivo.HasValue && nota.IdAtivo.Value > 0)
            {
                // Se IdAtivo está preenchido → RestauraAtivo
                nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                _logger.LogDebug("Nota {IdNota} marcada como RestauraAtivo (IdAtivo={IdAtivo})", 
                    nota.IdNota, nota.IdAtivo);
            }
            else if (nota.IdLi.HasValue && nota.IdLi.Value > 0)
            {
                // Se IdAtivo vazio mas IdLI preenchido → RestauraLI
                nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                _logger.LogDebug("Nota {IdNota} marcada como RestauraLI (IdLI={IdLI})", 
                    nota.IdNota, nota.IdLi);
            }
            // Se ambos vazios, não faz nada
        }

        //Agora faz uma nova busca em todas as notas que recebeu, não apenas as que tem item
        var palavrasRenovacao = new[] { "START", "RENOVAÇÃO", "RENOVACAO" };
        var palavrasTroca = new[] { "TROCA", "SUBSTITUIÇÃO", "SUBSTITUICAO" };

        foreach (var nota in notas)
        {
            var textosNota =
                $"{nota.DescricaoBreve ?? string.Empty} {nota.AnomaliaObservada ?? string.Empty} {nota.ObservacoesComplementares ?? string.Empty}"
                    .ToUpperInvariant();

            bool temRenovacao = palavrasRenovacao.Any(p => textosNota.Contains(p));
            bool temTroca = palavrasTroca.Any(p => textosNota.Contains(p));

            //para cada nota indentificada, verifica se ela renova ativo ou LI
            if (temRenovacao)
            {
                nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                _logger.LogDebug("Nota {IdNota} marcada como RestauraLI por texto (renovação)", nota.IdNota);

            }
            else if (temTroca && nota.IdAtivo.HasValue && nota.IdAtivo.Value > 0)
            {
                nota.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                _logger.LogDebug("Nota {IdNota} marcada como RestauraAtivo por texto (troca)", nota.IdNota);
            }
        }


        //Salva e Cadastra dados de renovação
        await _context.SaveChangesAsync();
        _logger.LogInformation("Alterações salvas no banco de dados");

        var resultado = await CadastraDadosRenovacaoAsync(idsNotas, ehNota: true);
        _logger.LogInformation("ImportarDadosNotaAsync concluído com {QtdRegistros} registros criados/atualizados", resultado);
            
        return resultado;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao processar notas em ImportarDadosNotaAsync");
        throw;
    }

}

// Verifica se Causa ou Sintoma contém palavras-chave
private static bool ContemPalavraChaveItem(string? causa, string? sintoma)
{
    var texto = $"{causa ?? string.Empty} {sintoma ?? string.Empty}".ToUpperInvariant();
    
    return texto.Contains("START") 
           || texto.Contains("RENOVAÇÃO") 
           || texto.Contains("RENOVACAO");
}

//Faz tratamento em caso de ordem
public async Task<int> ImportarDadosOrdemAsync(
    List<long> idsOrdens)
    {
        _logger.LogInformation("Iniciando ImportarDadosOrdemAsync com {QtdOrdens} ordens", idsOrdens?.Count ?? 0);

        if (idsOrdens == null || idsOrdens.Count == 0)
        {
            _logger.LogWarning("Lista de ordens vazia ou nula");
            return 0;
        }
        try
        {
        //Busca ordens no banco
        var ordens = await _context.PM_Ordens
            .Where(n => idsOrdens.Contains(n.IdOrdem))
            .ToListAsync();
        
        _logger.LogInformation("Carregadas {QtdOrdens} ordens do banco", ordens.Count);
            
        //Classifica as ordens baseado em DescricaoBreve
        var palavrasRenovacao = new[] { "TROCA", "START", "RENOVAÇÃO", "RENOVACAO" };
        var palavrasTroca = new[] {"SUBSTITUIÇÃO", "SUBSTITUICAO" };
        
        foreach (var ordem in ordens)
        {
            var textoOrdem = (ordem.DescricaoBreve ?? string.Empty).ToUpperInvariant();

            bool temRenovacao = palavrasRenovacao.Any(p => textoOrdem.Contains(p));
            bool temTroca = palavrasTroca.Any(p => textoOrdem.Contains(p));

            if (temRenovacao)
            {
                ordem.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraLI;
                _logger.LogDebug("Ordem {IdOrdem} marcada como RestauraLI", ordem.IdOrdem);
            }
            else if (temTroca && ordem.IdAtivo.HasValue && ordem.IdAtivo.Value > 0)
            {
                ordem.ConfiabilidadeRestaurada = ConfiabilidadeRestaurada.RestauraAtivo;
                _logger.LogDebug("Ordem {IdOrdem} marcada como RestauraAtivo", ordem.IdOrdem);
            }
        }

        //Salva e Cadastra dados de renovação
        await _context.SaveChangesAsync();
        _logger.LogInformation("Alterações salvas no banco de dados");

        var resultado = await CadastraDadosRenovacaoAsync(idsOrdens, ehNota: false);
        _logger.LogInformation("ImportarDadosOrdemAsync concluído com {QtdRegistros} registros criados/atualizados", resultado);
            
        return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar ordens em ImportarDadosOrdemAsync");
            throw;
        }

    }
    
    public async Task<int> CadastraDadosRenovacaoAsync(
    List<long> ids,
    bool ehNota) // true = nota, false = ordem
{
    _logger.LogInformation("Iniciando CadastraDadosRenovacaoAsync para {Tipo} com {QtdIds} IDs", ehNota, ids?.Count ?? 0);

    if (ids == null || ids.Count == 0)
    {
        _logger.LogWarning("Lista de IDs vazia ou nula para {Tipo}", ehNota);
        return 0;
    }

    var registrosCriados = 0;
    try
    {
    if (ehNota)
    {
        //se for nota, busca o id no banco
        var notas = await _context.PM_Notas
            .Where(n => ids.Contains(n.IdNota))
            .ToListAsync();
        
        _logger.LogInformation("Carregadas {QtdNotas} notas para cadastro de dados de renovação", notas.Count);

        //Cadastra cada nota 
        foreach (var nota in notas)
        {
            _logger.LogDebug("Processando nota {IdNota} com ConfiabilidadeRestaurada={Conf}", 
                nota.IdNota, nota.ConfiabilidadeRestaurada);
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
        
        //Cadastra cada ordem
        _logger.LogInformation("Carregadas {QtdOrdens} ordens para cadastro de dados de renovação", ordens.Count);

        foreach (var ordem in ordens)
        {
            _logger.LogDebug("Processando ordem {IdOrdem} com ConfiabilidadeRestaurada={Conf}", 
                ordem.IdOrdem, ordem.ConfiabilidadeRestaurada);

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
        _logger.LogInformation("Salvos {QtdRegistros} registros de dados de renovação", registrosCriados);
    }
    else
    {
        _logger.LogWarning("Nenhum registro de dados de renovação foi criado ou atualizado");
    }

    return registrosCriados;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao cadastrar dados de renovação para {Tipo}", tipo);
        throw;
    }
}

    //Trata dois casos, se a nota restaura apenas um ativo especifico ou toda a LI
private async Task<int> ProcessarPorConfiabilidadeAsync(
    ConfiabilidadeRestaurada? confiabilidade,
    int? idAtivo,
    long? idNota,
    long? idOrdem,
    DateTime dataSource,        // data da nota ou ordem
    OrigemUR origem)            // Nota ou Ordem
{
    if (confiabilidade == null)
    {
        _logger.LogDebug("ConfiabilidadeRestaurada é nula, nenhum processamento realizado");
        return 0;
    }

    var inseridos = 0;

    if (confiabilidade == ConfiabilidadeRestaurada.RestauraAtivo)
    {
        if (idAtivo.HasValue && idAtivo.Value > 0)
        {
            _logger.LogDebug("Processando RestauraAtivo para IdAtivo={IdAtivo}", idAtivo.Value);
            inseridos = await InserirOuAtualizarPorAtivoAsync(
                idAtivo.Value,
                idNota,
                idOrdem,
                dataSource,
                origem);
        }
        else
        {
            _logger.LogWarning("RestauraAtivo mas IdAtivo inválido ou nulo");
        }
    }
    else if (confiabilidade == ConfiabilidadeRestaurada.RestauraLI)
    {
        _logger.LogDebug("RestauraLI identificado - TODO: buscar ativos da LI");
        // TODO
        //Busca todos os Ativos daquele LI
        //For each ativo await InserirOuAtualizarPorAtivoAsync
    }

    return inseridos;
}

//Dado o ativo que será restaurado, verifica se ele já existe ou não na tabela DadosRenovacao
private async Task<int> InserirOuAtualizarPorAtivoAsync(
    int idAtivo,
    long? idNota,
    long? idOrdem,
    DateTime dataSource,
    OrigemUR origem)
{
    try
    {
    var existente = await _context.PM_DadosRenovacao
        .FirstOrDefaultAsync(d => d.IdAtivo == idAtivo);
    
    //se não existir adiciona
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
        _logger.LogInformation("Criado novo registro de DadosRenovacao: IdAtivo={IdAtivo}, Origem={Origem}, Data={Data}", 
            idAtivo, origem, dataSource);
        return 1;
    }
    else
    //se existir, verifica se a data nova é mais recente
    {
        if (dataSource > existente.UltimaRenovacao)
        {
            existente.UltimaRenovacao = dataSource;
            existente.OrigemUR = origem;
            if (idNota.HasValue) existente.IdNota = idNota.Value;
            if (idOrdem.HasValue) existente.IdOrdem = idOrdem.Value;
            _context.PM_DadosRenovacao.Update(existente);
            _logger.LogInformation("Atualizado registro de DadosRenovacao: IdAtivo={IdAtivo}, NovaData={NovaData}", 
                idAtivo, dataSource);
            return 1;
        }
        else
        {
            _logger.LogDebug("Registro de DadosRenovacao não atualizado: data anterior ou igual (IdAtivo={IdAtivo})", idAtivo);
        }
    }
    return 0;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao inserir/atualizar DadosRenovacao para IdAtivo={IdAtivo}", idAtivo);
        throw;
    }
}

}