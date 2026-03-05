using app_pm_back.Data;
using app_pm_back.Models;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class NotasService
{
    private readonly AppDbContext _context;

    // Injeção do contexto do Entity Framework
    public NotasService(AppDbContext context)
    {
        _context = context;
    }

    // cria uma nota e insere
    public async Task<NotaPetroModel> CreateNotaAsync(NotaPetroModel nota)
    {
        _context.NotasPetro.Add(nota);
        await _context.SaveChangesAsync();
        return nota; // O IdNota será preenchido automaticamente pelo PostgreSQL após o SaveChanges
    }

    // atualiza uma nota
    public async Task<NotaPetroModel> UpdateNotaAsync(NotaPetroModel nota)
    {
        _context.NotasPetro.Update(nota);
        await _context.SaveChangesAsync();
        return nota;
    }

    // retorna todas as notas
    public async Task<List<NotaPetroModel>> GetAllNotasAsync()
    {
        return await _context.NotasPetro.ToListAsync();
    }
    
    // retorna uma página de notas
    public async Task<List<NotaPetroModel>> GetNotasPaginadasAsync(int pagina, int tamanho)
    {
        return await _context.NotasPetro.
            OrderBy(n => n.IdNota) // No banco relacional é sempre bom ordenar antes de paginar
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync();
    }

    // retorna uma nota pelo id
    public async Task<NotaPetroModel?> GetNotaByIdAsync(int id)
    {
        return await _context.NotasPetro.FindAsync(id);
    }

    // deleta uma nota
    public async Task<bool> DeleteNotaAsync(int id)
    {
        var nota = await _context.NotasPetro.FindAsync(id);
        if (nota == null) return false;

        _context.NotasPetro.Remove(nota);
        var alteracoes = await _context.SaveChangesAsync();
        
        return alteracoes > 0;
    }

    // retorna a quantidade de notas no banco
    public async Task<long> CountNotasAsync()
    {
        return await _context.NotasPetro.CountAsync();
    }
    

    // retorna a quantidade e os dados resumidos das fichas de um ativo 
    // a busca é feita a partir de um local de instalação ou campo pesquisado
    public async Task<object> GetFichasPorAtivoAsync(string ativo, string? tipoNota)
    {
        // inicia a query na tabela de notas 
        var query = _context.NotasPetro.AsQueryable();
        
        // procura pelo ativo com mesmo local de instalação ou campo
        query = query.Where(n => n.LocInstalacao == ativo || n.Campo == ativo || n.Localizacao == ativo);
        
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