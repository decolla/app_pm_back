using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Servico de CRUD e operacoes em lote para equipamentos.
/// </summary>
public class EquipmentService : BaseCrudService<EquipamentoAtivoModel>
{
    /// <summary>
    /// Construtor.
    /// </summary>
    public EquipmentService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Cria ou atualiza equipamentos em massa, criando referencias fantasma quando necessario.
    /// </summary>
    public async Task CreateEquipamentosEmMassaAsync(List<EquipamentoAtivoModel> equipamentos)
    {
        if (equipamentos == null || !equipamentos.Any()) return;

        var fabricanteIds = equipamentos
            .Where(e => e.FabricanteId.HasValue)
            .Select(e => e.FabricanteId!.Value)
            .Distinct()
            .ToList();

        var localizacaoIds = equipamentos
            .Where(e => e.LocalizacaoId.HasValue)
            .Select(e => e.LocalizacaoId!.Value)
            .Distinct()
            .ToList();

        var enderecoIds = equipamentos
            .Where(e => e.EnderecoId.HasValue)
            .Select(e => e.EnderecoId!.Value)
            .Distinct()
            .ToList();

        var imobilizadoTags = equipamentos
            .Where(e => !string.IsNullOrWhiteSpace(e.ImobilizadoTag))
            .Select(e => e.ImobilizadoTag!)
            .Distinct()
            .ToList();

        var gpmIds = equipamentos
            .Where(e => e.GpmId.HasValue)
            .Select(e => e.GpmId!.Value)
            .Distinct()
            .ToList();

        var centroTrabalhoIds = equipamentos
            .Where(e => e.CentroTrabalhoId.HasValue)
            .Select(e => e.CentroTrabalhoId!.Value)
            .Distinct()
            .ToList();

        var locaisInstalacao = equipamentos
            .Where(e => e.LocalInstalacaoCodigo.HasValue)
            .Select(e => e.LocalInstalacaoCodigo!.Value)
            .Distinct()
            .ToList();

        await ImportGhostHelper.EnsureFabricantesAsync(_context, fabricanteIds.Select(id => (int?)id));
        await ImportGhostHelper.EnsureLocalizacoesAsync(_context, localizacaoIds.Select(id => (int?)id));
        await ImportGhostHelper.EnsureEnderecosAsync(_context, enderecoIds.Select(id => (int?)id));
        await ImportGhostHelper.EnsureImobilizadosAsync(_context, imobilizadoTags);
        await ImportGhostHelper.EnsureGpmAsync(_context, gpmIds.Select(id => (int?)id));
        await ImportGhostHelper.EnsureCentrosTrabalhoAsync(_context, centroTrabalhoIds.Select(id => (int?)id));
        await ImportGhostHelper.EnsureLocaisInstalacaoAsync(_context, locaisInstalacao.Select(id => (int?)id));

        var tags = equipamentos
            .Where(e => !string.IsNullOrWhiteSpace(e.TagEquipamento))
            .Select(e => e.TagEquipamento)
            .Distinct()
            .ToList();

        var existentes = await _context.PM_EquipamentosAtivos
            .Where(e => tags.Contains(e.TagEquipamento))
            .ToDictionaryAsync(e => e.TagEquipamento);

        var novosEquipamentos = new List<EquipamentoAtivoModel>();
        foreach (var equipamento in equipamentos)
        {
            if (string.IsNullOrWhiteSpace(equipamento.TagEquipamento)) continue;

            if (existentes.TryGetValue(equipamento.TagEquipamento, out var existente))
            {
                ImportMergeHelper.MergeNonEmpty(existente, equipamento);

                if (equipamento.DetalhesTecnicos != null && equipamento.DetalhesTecnicos.Count > 0)
                {
                    foreach (var detalhe in equipamento.DetalhesTecnicos)
                    {
                        existente.DetalhesTecnicos[detalhe.Key] = detalhe.Value;
                    }
                }
            }
            else
            {
                novosEquipamentos.Add(equipamento);
            }
        }

        if (novosEquipamentos.Any())
        {
            await _context.AddRangeAsync(novosEquipamentos);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Remove um equipamento pela tag (chave primaria textual).
    /// </summary>
    public async Task<bool> DeleteByTagAsync(string tagEquipamento)
    {
        var entity = await _context.PM_EquipamentosAtivos.FindAsync(tagEquipamento);
        if (entity == null)
        {
            return false;
        }

        _context.PM_EquipamentosAtivos.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}