using app_pm_back.Data;
using app_pm_back.Models.Domain;
using app_pm_back.Services;
using app_pm_back.Services.GenericServices;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Jobs;

public class ManutencaoGeneratorJob
{
    private readonly AppDbContext _context;
    private readonly ConfiguracaoGlobalService _configuracaoGlobalService;
    private readonly ILogger<ManutencaoGeneratorJob> _logger;

    public ManutencaoGeneratorJob(AppDbContext context, ILogger<ManutencaoGeneratorJob> logger,
        ConfiguracaoGlobalService configuracaoGlobalService)
    {
        _context = context;
        _logger = logger;
        _configuracaoGlobalService = configuracaoGlobalService;
    }

    public async Task RunDailyRoutine()
    {
        var config = await _configuracaoGlobalService.ObterConfiguracaoAsync();
        if (config.CriarFichasPreventivas == CriarFichasPreventivas.NaoCriar)
        {
            _logger.LogInformation(
                "Criar fichas preventivas desativada nas configurações globais, interrompendo processo...");
            return;
        }

        _logger.LogInformation("Iniciando varredura de planos de manutenção vencidos...");

        try
        {
            var dataAtual = DateTime.UtcNow;

            var planos = await _context.PM_Planos.AsNoTracking()
                .Where(p => p.DataConclusao > dataAtual && p.DataInicios <= dataAtual)
                .Include(p => p.TiposPlano)
                .ToListAsync();

            if (!planos.Any())
            {
                _logger.LogInformation("Nenhum plano ativo encontrado.");
                return;
            }

            // pega todos os Ids das ordens 
            var idsOrdensEsperadas = planos.SelectMany(p => p.TiposPlano)
                .Where(t => t.IdOrdem != null)
                .Select(t => t.IdOrdem!.Value).ToList();

            var ordensExistentes = await _context.PM_Ordens
                .Where(o => idsOrdensEsperadas.Contains(o.IdOrdem))
                .Select(o => o.IdOrdem)
                .ToListAsync();

            // pega todos os Ids das notas 
            var idsNotasEsperadas = planos.SelectMany(p => p.TiposPlano)
                .Where(t => t.IdNota != null)
                .Select(t => t.IdNota!.Value).ToList();

            var notasExistentes = await _context.PM_Notas
                .Where(n => idsNotasEsperadas.Contains(n.IdNota))
                .Select(n => n.IdNota)
                .ToListAsync();

            int novasOrdens = 0;
            int novasNotas = 0;

            foreach (var plano in planos)
            {
                var tiposVencidos = plano.TiposPlano
                    .Where(tp =>
                        plano.DataInicios!.Value.AddDays((tp.Dias ?? 0) - (tp.ToleranciaNegativa ?? 0)) < dataAtual)
                    .ToList();

                foreach (var tipo in tiposVencidos)
                {
                    if (tipo.IdOrdem.HasValue && !ordensExistentes.Contains(tipo.IdOrdem.Value))
                    {
                        var ordem = new OrdemModel()
                        {
                            IdOrdem = tipo.IdOrdem.Value,
                            IdPlano = plano.IdPlano,
                            DataCriacao = dataAtual,
                            StatusOrdem = StatusFichas.Aberta,
                            CriadaPorUsuario = "System",
                        };
                        _context.PM_Ordens.Add(ordem);
                        ordensExistentes.Add(tipo.IdOrdem.Value);
                        novasOrdens++;
                    }

                    else if (tipo.IdNota.HasValue && !notasExistentes.Contains(tipo.IdNota.Value))
                    {
                        var nota = new NotaModel()
                        {
                            IdNota = tipo.IdNota.Value,
                            IdPlano = plano.IdPlano,
                            DataNota = dataAtual,
                            StatusNota = StatusFichas.Aberta,
                            CriadaPorUsuario = "System",
                        };
                        _context.PM_Notas.Add(nota);
                        notasExistentes.Add(tipo.IdNota.Value);
                        novasNotas++;
                    }
                }
            }

            // salvar tudo num único lote transacionado
            if (novasOrdens > 0 || novasNotas > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Varredura concluída: {novasOrdens} ordens e {novasNotas} notas criadas.");
            }
            else
            {
                _logger.LogInformation("Varredura concluída: Nenhuma nova ordem ou nota precisou ser criada.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a varredura de planos de manutenção vencidos.");
        }
    }
}