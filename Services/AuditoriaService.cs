using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Classe para retorno de mensagens de auditoria
/// </summary>
public class MensagemAuditoria
{
    public string NomeUsuario { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty; 
    public string? Unidade { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataAlteracao { get; set; }
}

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
    public async Task<object> SearchAuditoria(long? usuario, EntityState? acao, DateTime? dataInicio,
        DateTime? dataFim, int page, int size)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (usuario > 0)
        {
            var usuarioStr = usuario.Value.ToString();
            query = query.Where(a => a.IdUsuario == usuarioStr);
        }

        if (acao.HasValue)
        {
            query = query.Where(a => a.Acao == acao.Value);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(a => a.DataAlteracao >= dataInicio.Value);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(a => a.DataAlteracao <= dataFim.Value);
        }

        int total = await query.CountAsync();
        var items = await query.OrderBy(e => e.DataAlteracao)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new { Total = total, Result = items };
    }

    /// <summary>
    /// Procura as auditorias de ações realizadas por um usuário
    /// </summary>
    /// <param name="usuario">ID do usuário</param>
    /// <returns>Lista de auditorias</returns>
    public async Task<List<AuditoriaModel>> GetAuditoriaByUsuario(long usuario)
    {
        var usuarioStr = usuario.ToString();
        return await _dbSet.AsNoTracking().Where(a => a.IdUsuario == usuarioStr).ToListAsync();
    }

    /// <summary>
    /// Obtém as mensagens da auditoria paginada 
    /// </summary>
    /// <param name="page">Página a ser exibida</param>
    /// <param name="size">Tamanho da página</param>
    /// <returns></returns>
    public async Task<List<MensagemAuditoria>> GenerateAuditoriaMessageAsync(string tabela, List<string> ids)
    {
        if (ids == null || !ids.Any()) return new List<MensagemAuditoria>();

        var result = await _dbSet
            .AsNoTracking()
            .Where(a => ids.Contains(a.ChavePrimaria!) && a.NomeTabela == tabela)
            .OrderBy(e => e.DataAlteracao)
            .ToListAsync();

        // busca todos os usuários de uma vez
        var userIds = result
            .Where(a => !string.IsNullOrEmpty(a.IdUsuario))
            .Select(a => long.Parse(a.IdUsuario!))
            .Distinct()
            .ToList();

        var usuariosCache = await _context.PM_Usuarios
            .AsNoTracking()
            .Include(u => u.Gpms)
            .Where(u => userIds.Contains(u.IdUsuario))
            .ToDictionaryAsync(u => u.IdUsuario);

        List<MensagemAuditoria> audits = new();

        foreach (var audit in result)
        {
            // obtém a ação realizada em string 
            string ação = "";
            string mensagem = "";

            if (audit.Acao == EntityState.Added)
            {
                ação = "Adicionou";
                mensagem = "Criou o registro";
            }
            else if (audit.Acao == EntityState.Deleted)
            {
                ação = "Excluiu";
                mensagem = "Excluiu o registro";
            }
            else if (audit.Acao == EntityState.Modified)
            {
                ação = "Modificou";
                mensagem = "Alterou para:" + audit.ValoresNovos;
            }
            else if (audit.Acao == EntityState.Unchanged)
                ação = "Não alterou";
            else if (audit.Acao == EntityState.Detached)
                ação = "Desanexou";

            // busca o usuário responsável em memória
            UsuarioModel? user = null;
            if (long.TryParse(audit.IdUsuario, out long uid))
            {
                usuariosCache.TryGetValue(uid, out user);
            }

            string nomeUsuario = user?.Nome ?? "Usuário não encontrado";
            // unidade formatada
            string? unidade = user?.Gpms?.FirstOrDefault()?.Nome;

            audits.Add(new MensagemAuditoria()
            {
                NomeUsuario = nomeUsuario,
                Acao = ação,
                Unidade = unidade,
                Mensagem = mensagem,
                DataAlteracao = audit.DataAlteracao,
            });
        }

        return audits;
    }

    /// <summary>
    /// Obtém uma mensagem de auditoria para um registro específico
    /// </summary>
    /// <param name="tabela">Tabela a ser buscada</param>
    /// <param name="id">Id a ser buscado</param>
    /// <returns>Lista de mensagens</returns>
    public async Task<List<MensagemAuditoria>> GetAuditoriaMessageAsync(string tabela, string id)
    {
        return await GenerateAuditoriaMessageAsync(tabela, new List<string> { id });
    }

    /// <summary>
    /// Obtém a mensagem de auditoria para uma nota e seus dependentes 
    /// </summary>
    /// <param name="id">Id da nota a ser buscada</param>
    /// <returns>Lista de mensagens</returns>
    public async Task<List<MensagemAuditoria>> GetAuditoriaMessageNotaAsync(long id)
    {
        NotaModel? nota = await _context.PM_Notas
            .Include(n => n.Itens)
            .Include(n => n.Variaveis)
            .Include(n => n.Anexos)
            .FirstOrDefaultAsync(n => n.IdNota == id);

        if (nota == null)
        {
            throw new Exception("Nota não encontrada");
        }

        List<string> idsItens = nota.Itens.Select(i => i.IdItemNota.ToString()).ToList();
        List<string> idsVariaveis = nota.Variaveis.Select(v => v.IdVariavel.ToString()).ToList();
        List<string> idsAnexos = nota.Anexos.Select(a => a.IdAnexoNota.ToString()).ToList();

        List<MensagemAuditoria> notasMessage = await GetAuditoriaMessageAsync("PM_Notas", id.ToString());
        List<MensagemAuditoria> itensMessage = await GenerateAuditoriaMessageAsync("PM_ItensNota", idsItens);
        List<MensagemAuditoria> variaveisMessage = await GenerateAuditoriaMessageAsync("PM_VariaveisNota", idsVariaveis);
        List<MensagemAuditoria> anexosMessage = await GenerateAuditoriaMessageAsync("PM_AnexosNota", idsAnexos);
        
        var result = notasMessage.Concat(itensMessage).Concat(variaveisMessage).Concat(anexosMessage);
        
        return [.. result.OrderByDescending(e => e.DataAlteracao)];
    }

    /// <summary>
    /// Obtém a mensagem de auditoria para uma ordem e seus dependentes 
    /// </summary>
    /// <param name="id">Id da ordem a ser buscada</param>
    /// <returns>Lista de mensagens</returns>
    public async Task<List<MensagemAuditoria>> GetAuditoriaMessageOrdemAsync(long id)
    {
        OrdemModel? ordem = await _context.PM_Ordens
            .Include(o => o.Operacoes)
            .Include(o => o.Anexos)
            .FirstOrDefaultAsync(o => o.IdOrdem == id);

        if (ordem == null) throw new Exception("Ordem não encontrada");
        
        List<string> idsOperacoes = ordem.Operacoes.Select(v => v.IdOperacao.ToString()).ToList();
        List<string> idsAnexos = ordem.Anexos.Select(a => a.IdAnexoOrdem.ToString()).ToList();
        
        List<MensagemAuditoria> ordensMessage = await GetAuditoriaMessageAsync("PM_Ordens", id.ToString());
        List<MensagemAuditoria> operacoesMessage = await GenerateAuditoriaMessageAsync("PM_Operacoes", idsOperacoes);
        List<MensagemAuditoria> anexosMessage = await GenerateAuditoriaMessageAsync("PM_AnexosOrdem", idsAnexos);
        
        var result = ordensMessage.Concat(operacoesMessage).Concat(anexosMessage);
        
        return result.OrderByDescending(e => e.DataAlteracao).ToList();
    }

    public async Task<int> GetAuditoriaCount()
    {
        return await _dbSet.CountAsync();
    }
}