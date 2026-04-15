using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class MaintancePlanComgasService : BaseCrudService<MaintancePlanComgasModel>
{
    private readonly DbSet<NoteCongasModel> _noteCongasModels;
    private readonly DbSet<MaintancePlanComgasModel> _maintancePlanComgasModels;
    
    public MaintancePlanComgasService(AppDbContext context) : base(context)
    {
        _maintancePlanComgasModels = context.Set<MaintancePlanComgasModel>();
        _noteCongasModels= context.Set<NoteCongasModel>();
    }
    
    public async Task CreatePlanosEmMassaAsync(List<MaintancePlanComgasModel> planos)
    {
        if (planos.Any())
        {
            await _context.PlanosManutencao.AddRangeAsync(planos);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<object?> AtivoGerald(string ativo)
    {
        // seleciona todas as notas associadas ao ativo
        List<NoteCongasModel> notas = await _noteCongasModels
            .Where(n => n.LocInstalacao == ativo || n.Equipamento == ativo || n.Item == ativo)
            .ToListAsync();

        // encontra os planos de manutenção associados a essas notas
        List<long?> planoId = notas
            .Select(n => n.PlanoManut)
            .ToList();
        
        List<MaintancePlanComgasModel> planosManut = await _maintancePlanComgasModels
            .Where(p => planoId.Contains(p.PlanoManut))
            .ToListAsync(); //alterar dps para busca melhorada, sem puxar todos os planos
        
        // encontra o último plano criado 
        MaintancePlanComgasModel? ultimoPlano = planosManut
            .OrderByDescending(p => p.DataPM)
            .FirstOrDefault();

        if(ultimoPlano == null) return null;
        
        // encontra a última nota do plano
        NoteCongasModel? ultimaNota = notas
            .Where(n=> n.PlanoManut == ultimoPlano.PlanoManut)
            .OrderByDescending(n => n.DataNota)
            .FirstOrDefault();

        return new
        {
            Tipo = ultimaNota?.TipoNota,
            DataCriacao = ultimoPlano.DataPM,
            Estrategia = ultimoPlano.Estrategia,
            Ciclo = ultimoPlano.Ciclo,
            Unidade = ultimoPlano.Unidade
        };
    }
}