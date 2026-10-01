using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

public class SyncUsuarioDto
{
    // dados do Portal
    public string Id { get; set; } = string.Empty; // UUID do portal
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // dados do PM
    public string? Cpf { get; set; }
    public string? Telefone { get; set; }
    public NivelPermissao? NivelPermissao { get; set; }
    public PermissoesTabela? Permissoes { get; set; }
    public List<long> Escopos { get; set; } = new();
    public List<long> GpmIds { get; set; } = new();
    public List<long> CtmIds { get; set; } = new();
}