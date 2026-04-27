using app_pm_back.Data;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class NotaManutencaoService : BaseCrudService<NotaManutencaoModel>
{
    public NotaManutencaoService(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Importa notas em lote, validando e ignorando Chaves Estrangeiras que não existem no banco.
    /// </summary>
    /// <param name="notas">Notas a serem cadastradas</param>
    public async Task ImportarNotasEmLoteSeguroAsync(List<NotaManutencaoModel> notas)
    {
        if (notas == null || !notas.Any()) return;

        // extrai todos os códigos únicos que vieram do Excel
        var codigosLocaisExcel = notas
            .Where(n => n.LocalInstalacaoCodigo != null) // filtra para evitar nulls
            .Select(n => n.LocalInstalacaoCodigo!)
            .Distinct()
            .ToList();

        var tagsEquipamentosExcel = notas
            .Where(n => !string.IsNullOrWhiteSpace(n.EquipamentoTag))
            .Select(n => n.EquipamentoTag!)
            .Distinct()
            .ToList();

        // consulta o Banco de Dados 
        var locaisExistentes = (await _context.LocaisInstalacao
                .Where(l => codigosLocaisExcel.Contains(l.CodigoLocalInstalacao)) 
                .Select(l => l.CodigoLocalInstalacao)
                .ToListAsync())
                .ToHashSet();

        var equipamentosExistentes = (await _context.EquipamentosAtivos
                .Where(e => tagsEquipamentosExcel.Contains(e.TagEquipamento))
                .Select(e => e.TagEquipamento)
                .ToListAsync())
                .ToHashSet();

        // limpeza
        foreach (var nota in notas)
        {
            // se a nota apontar para um Local que NÃO está no banco, anulamos o campo
            if (!string.IsNullOrWhiteSpace(nota.LocalInstalacaoCodigo) && !locaisExistentes.Contains(nota.LocalInstalacaoCodigo))
            {
                nota.LocalInstalacaoCodigo = null; 
            }

            // se a nota apontar para um Equipamento que NÃO está no banco, anulamos o campo
            if (!string.IsNullOrWhiteSpace(nota.EquipamentoTag) && !equipamentosExistentes.Contains(nota.EquipamentoTag))
            {
                nota.EquipamentoTag = null;
            }
        }

        // inserção Rápida em Lote
        await _dbSet.AddRangeAsync(notas);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Regra específica para a API, busca fichas por ativo e tipo de nota
    /// </summary>
    /// <param name="ativo">Nome do ativo a ser buscado como referência</param>
    /// <param name="tipoNota">Tipo da nota (corretiva ou preventiva)</param>
    /// <returns></returns>
    public async Task<object> GetFichasPorAtivoAsync(string ativo, string? tipoNota)
    {
        var query = _dbSet.AsQueryable(); 
        
        query = query.Where(n => n.LocalInstalacaoCodigo == ativo || n.EquipamentoTag == ativo);
        
        if (!string.IsNullOrWhiteSpace(tipoNota))
        {
            query = query.Where(n => n.TipoNota == tipoNota);
        }
        
        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.DataNota,
            Tipo = n.TipoNota
        }).ToListAsync();
        
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}