using app_pm_back.Data;
using app_pm_back.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services.Manutencao;

/// <summary>
/// Classe UsuarioService.
/// </summary>
public class UsuarioService : BaseCrudService<UsuarioModel>
{
    private readonly ILogger<UsuarioService> _logger;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="UsuarioService"/>.
    /// </summary>
    public UsuarioService(AppDbContext context, ILogger<UsuarioService>? logger = null) : base(context)
    {
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<UsuarioService>.Instance;
    }

    /// <summary>
    /// Atribui a reponsabilidade de uma ordem para um usuário
    /// </summary>
    public async Task<bool> AtribuirPermissao(string? nomeUser, long? ordem)
    {
        try
        {
            var usuario = await _context.Set<UsuarioModel>().FirstOrDefaultAsync(u => u.Nome == nomeUser);
            if (usuario != null)
            {
                var ordemModel = await _context.Set<OrdemModel>().FirstOrDefaultAsync(o => o.IdOrdem == ordem);
                if (ordemModel != null)
                {
                    usuario.Ordens.Add(ordemModel);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar responsabilidade da ordem {ordem} ao usuário {nomeUser}", ordem,
                nomeUser);
            throw;
        }
    }

    public async Task<List<UsuarioModel>> SearchUsuario(string? cpf, string? nome, string? funcao, int? schema)
    {
        var usuarios = await _context.Set<UsuarioModel>()
            .Where(u => (string.IsNullOrEmpty(cpf) || u.CPF!.Contains(cpf)) &&
                        (string.IsNullOrEmpty(nome) || u.Nome!.Contains(nome)) &&
                        //(string.IsNullOrEmpty(funcao) || u.Funcao.Contains(funcao)) &&
                        // TODO: Adicionar filtro por FUNÇÃO se necessário
                        (!schema.HasValue || u.Escopos.Contains(schema.Value)))
            .ToListAsync();

        return usuarios;
    }

    public async Task<List<object>> AtribuirFichas(NotaOrdem tipoFicha, long idFicha, long idGrupo,
        List<long> idsUsuarios)
    {
        var usuarios = await _context.Set<UsuarioModel>().Where(u => idsUsuarios.Contains(u.IdUsuario)).ToListAsync();

        if (tipoFicha == NotaOrdem.Ordem)
        {
            OrdemModel? ficha;
            ficha = await _context.Set<OrdemModel>().FirstOrDefaultAsync(o => o.IdOrdem == idFicha);

            if (ficha == null) throw new KeyNotFoundException($"Ordem com ID {idFicha} não encontrada.");

            foreach (var usuario in usuarios) usuario.Ordens.Add(ficha);
        }
        else
        {
            NotaModel? ficha;
            ficha = await _context.Set<NotaModel>().FirstOrDefaultAsync(n => n.IdNota == idFicha);

            if (ficha == null) throw new KeyNotFoundException($"Nota com ID {idFicha} não encontrada.");

            foreach (var usuario in usuarios)
            {
                usuario.Notas.Add(ficha);
            }
        }

        await _context.SaveChangesAsync();
        return new List<object>();
    }
}