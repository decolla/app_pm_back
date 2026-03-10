using app_pm_back.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.CongasServices;

public class NoteCongasService
{
    private readonly AppDbContext _context;

    public NoteCongasService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NoteCongasModel> CreateNotaAsync(NoteCongasModel nota)
    {
        _context.NotasCongas.Add(nota);
        await _context.SaveChangesAsync();
        return nota;
    }
    
    // atualiza uma nota
    public async Task<NoteCongasModel> UpdateNotaAsync(NoteCongasModel nota)
    {
        _context.NotasCongas.Update(nota);
        await _context.SaveChangesAsync();
        return nota;
    }

    // retorna todas as notas
    public async Task<List<NoteCongasModel>> GetAllNotasAsync()
    {
        return await _context.NotasCongas.ToListAsync();
    }
    
    // retorna uma página de notas
    public async Task<List<NoteCongasModel>> GetNotasPaginadasAsync(int pagina, int tamanho)
    {
        return await _context.NotasCongas.
            OrderBy(n => n.IdNota) // No banco relacional é sempre bom ordenar antes de paginar
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();
    }

    // retorna uma nota pelo id
    public async Task<NoteCongasModel?> GetNotaByIdAsync(int id)
    {
        return await _context.NotasCongas.FindAsync(id);
    }

    // deleta uma nota
    public async Task<bool> DeleteNotaAsync(int id)
    {
        var nota = await _context.NotasCongas.FindAsync(id);
        if (nota == null) return false;

        _context.NotasCongas.Remove(nota);
        var alteracoes = await _context.SaveChangesAsync();
        
        return alteracoes > 0;
    }

    // retorna a quantidade de notas no banco
    public async Task<long> CountNotasAsync()
    {
        return await _context.NotasCongas.CountAsync();
    }
    

    // retorna a quantidade e os dados resumidos das fichas de um ativo 
    // a busca é feita a partir de um local de instalação ou campo pesquisado
    public async Task<object> GetFichasPorAtivoAsync(string ativo, string? tipoNota)
    {
        // inicia a query na tabela de notas 
        var query = _context.NotasCongas.AsQueryable();
        
        // procura pelo ativo com mesmo local de instalação ou campo
        query = query.Where(n => n.LocInstalacao == ativo || n.Denominacao == ativo || n.Local == ativo);
        
        // aplica o filtro para procurar a nota pelo tipo passado
        if (!string.IsNullOrWhiteSpace(tipoNota))
        {
            query = query.Where(n => n.TipoNota == tipoNota);
        }
        
        // realiza um select que traz somente número e data da nota 
        var fichas = await query.Select(n => new
        {
            NumerFicha = n.IdNota,
            Data = n.DataNota,
            Tipo = n.TipoNota
        }).ToListAsync();
        
        // retorna o formato
        return new
        {
            Ativo = ativo,
            Quantidade = fichas.Count,
            Fichas = fichas
        };
    }
}