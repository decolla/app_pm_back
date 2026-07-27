using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

public class AuditoriaService : BaseCrudService<AuditoriaModel>
{

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="AuditoriaService"/>.
    /// </summary>
    public AuditoriaService(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Procura pelas auditorias com base nos filtros estabelecidos 
    /// </summary>
    /// <param name="usuario">Usuário que realizou a ação</param>
    /// <param name="acao">Ação realizada</param>
    /// <param name="dataInicio">Data de início</param>
    /// <param name="dataFim">Data de fim</param>
    /// <returns></returns>
    public async Task<List<AuditoriaModel>> SearchAuditoria(long? usuario, EntityState? acao, DateTime? dataInicio,
        DateTime? dataFim, int page, int size)
    {
        var query = _dbSet.AsQueryable();

        if (usuario > 0)
        {
            query = query.Where(a => a.IdUsuario == usuario);
        }

        if (acao.HasValue)
        {
            query = query.Where(a => a.Acao == acao.Value);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(a => a.DataAlteracao >= dataInicio.Value);
        }

        return await query.OrderBy(e => e.DataAlteracao)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    /// <summary>
    /// Procura as auditorias de ações realizadas por um usuário
    /// </summary>
    /// <param name="usuario">ID do usuário</param>
    /// <returns>Lista de auditorias</returns>
    public async Task<List<AuditoriaModel>> GetAuditoriaByUsuario(long usuario)
    {
        return await _dbSet.Where(a => a.IdUsuario == usuario).ToListAsync();
    }

    /// <summary>
    /// Obtém as mensagens da auditoria paginada 
    /// </summary>
    /// <param name="page">Pagina a ser exibida</param>
    /// <param name="size">Tamanho da pagina</param>
    /// <returns></returns>
    public async Task<List<object>> GetAuditoriaMessage(string tabela, string id, int page, int size)
    {
        int count = await _dbSet.CountAsync();

        var result = await _dbSet
            .Where(a => a.ChavePrimaria == id && a.NomeTabela ==  tabela)
            .OrderBy(e => e.DataAlteracao)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        List<object> audits = new List<object>();
        
        foreach (var audit in result)
        {
            // obtém a ação realizada em string 
            string ação = "";
            if (audit.Acao == EntityState.Added)
                ação = "Adicionou";
            else if (audit.Acao == EntityState.Deleted)
                ação = "Excluiu";
            else if (audit.Acao == EntityState.Modified)
                ação = "Modificou";
            else if (audit.Acao == EntityState.Unchanged)
                ação = "Não alterou";
            else if (audit.Acao == EntityState.Detached)
                ação = "Desanexou";

            // busca o usuário responsável (TALVEZ REMOVER POR PESO DE CONSULTA POR CADA AUDITORIA)
            UsuarioModel? user = await _context.PM_Usuarios.Where(u => u.IdUsuario == audit.IdUsuario)
                .Include(usuarioModel => usuarioModel.Gpms)
                .FirstOrDefaultAsync();

            string nomeUsuario = user?.Nome ?? "Usuário não encontrado";
            string mensagem = nomeUsuario + " " + ação + ": " + audit.ValoresNovos;
            
            object obj = new
            {
                NomeUsuario = nomeUsuario,
                Ação = ação,
                Unidade = user?.Gpms,
                Mensagem = mensagem,
                DataAlteracao = audit.DataAlteracao,
            };
            
            audits.Add(obj);
        }
        
        return audits;
    }
    
    public async Task<int> GetAuditoriaCount()
    {
        return await _dbSet.CountAsync();
    }
    
}