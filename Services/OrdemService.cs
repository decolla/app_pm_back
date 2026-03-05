using app_pm_back.Data;
using app_pm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class OrdemService
{
    private readonly AppDbContext _context;

    public OrdemService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria uma ordem e vincula automaticamente à nota de origem.
    /// </summary>
    public async Task<OrdemPetroModel> CreateOrdemAsync(OrdemPetroModel ordem)
    {
        // 1. Verifica se a Nota existe no banco antes de criar a Ordem
        var nota = await _context.NotasPetro.FindAsync(ordem.IdNota);
        if (nota == null)
        {
            throw new Exception($"Não foi possível encontrar a Nota com o ID {ordem.IdNota} para gerar a ordem.");
        }

        // 2. Adiciona a Ordem ao contexto
        _context.OrdensPetro.Add(ordem);
        
        // Salva para gerar o IdOrdem no banco (PostgreSQL gera o ID numérico)
        await _context.SaveChangesAsync();

        // 3. Atualiza a Nota com o ID da Ordem gerada
        nota.IdOrdem = ordem.IdOrdem;
        
        // Salva a alteração da Nota
        await _context.SaveChangesAsync();

        return ordem;
    }

    /// <summary>
    /// Retorna todas as ordens cadastradas.
    /// </summary>
    public async Task<List<OrdemPetroModel>> GetAllOrdensAsync()
    {
        return await _context.OrdensPetro
            .Include(o => o.NotaOrigem) // Traz os dados da Nota junto (opcional, remova se quiser só a Ordem)
            .ToListAsync();
    }

    /// <summary>
    /// Retorna ordens com paginação.
    /// </summary>
    public async Task<List<OrdemPetroModel>> GetOrdensPaginadasAsync(int pagina, int tamanho)
    {
        return await _context.OrdensPetro
            .OrderBy(o => o.IdOrdem)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();
    }

    /// <summary>
    /// Busca uma ordem específica pelo ID.
    /// </summary>
    public async Task<OrdemPetroModel?> GetOrdemByIdAsync(long id)
    {
        return await _context.OrdensPetro
            .Include(o => o.NotaOrigem) // Faz o JOIN automático com a tabela de Notas
            .FirstOrDefaultAsync(o => o.IdOrdem == id);
    }

    /// <summary>
    /// Atualiza os dados de uma ordem existente.
    /// </summary>
    public async Task<OrdemPetroModel> UpdateOrdemAsync(OrdemPetroModel ordem)
    {
        var existe = await _context.OrdensPetro.AnyAsync(o => o.IdOrdem == ordem.IdOrdem);
        if (!existe)
        {
            throw new Exception("Ordem não encontrada para atualização.");
        }

        _context.OrdensPetro.Update(ordem);
        await _context.SaveChangesAsync();
        
        return ordem;
    }

    /// <summary>
    /// Deleta uma ordem pelo ID.
    /// </summary>
    public async Task<bool> DeleteOrdemAsync(long id)
    {
        var ordem = await _context.OrdensPetro.FindAsync(id);
        if (ordem == null) return false;

        // Se deletar a ordem, precisamos desvincular da nota (opcional dependendo da sua regra de negócio)
        var nota = await _context.NotasPetro.FirstOrDefaultAsync(n => n.IdOrdem == id);
        if (nota != null)
        {
            nota.IdOrdem = null;
        }

        _context.OrdensPetro.Remove(ordem);
        var alteracoes = await _context.SaveChangesAsync();
        
        return alteracoes > 0;
    }

    /// <summary>
    /// Retorna a contagem total de ordens.
    /// </summary>
    public async Task<long> CountOrdensAsync()
    {
        return await _context.OrdensPetro.CountAsync();
    }
}