using System.ComponentModel.DataAnnotations;

namespace app_pm_back.DTO;

public class CreateNotaPetroDto
{
    /// <summary>
    /// O campo de seleção destinado na nota
    /// </summary>
    /// <example> AA-0000</example>
    [Required(ErrorMessage = "Campo é obrigatório")]
    public string Campo { get; set; }
    
    /// <summary>
    /// Descrição do estado do ativo relatado na nota
    /// </summary>
    /// <example> AA-0000 Em falha</example>
    [Required(ErrorMessage = "A descrição é obrigatória")]
    public string Descricao { get; set; }
    
    /// <summary>
    /// Local de instalação do campo de seleção
    /// </summary>
    /// <example>PDL1.000</example>
    [Required(ErrorMessage = "O Local de instalação é obrigatório")]
    public string LocInstalacao { get; set; }
    
    /// <summary>
    /// Local de instalação no qual é afetado pelo campo de seleção e estado atual
    /// </summary>
    /// <example>PDL1.000</example>
    public string? LocInstAfetado { get; set; }
    
    /// <summary>
    /// Área de operação do campo de seleção
    /// </summary>
    /// <example>019</example>
    public int AreaOp { get; set; }
    
    /// <summary>
    /// Centro de trabalho principal do campo seleção
    /// </summary>
    /// <example>ELETILU</example>>
    public string CenTrabPrinc { get; set; }
    
    /// <summary>
    /// Data de emissão da nota 
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [Required(ErrorMessage = "A data é obrigatória")]
    public DateTime DataNota { get; set; }
    
    /// <summary>
    /// Sigla da localização do campo de seleção
    /// </summary>
    /// <example>U-000</example>>
    public string Localizacao { get; set; }

    /// <summary>
    /// Grupo planejado para o campo de seleção
    /// </summary>
    /// <example>ELE</example>
    public string GrupoPlanej { get; set; }
    
    /// <summary>
    /// Tipo de nota gerada
    /// </summary>
    /// <example>ZF</example>
    public string TipoNota { get; set; }
    
    /// <summary>
    /// Nota para o impacto da falha, de 0 a 6 
    /// </summary>
    /// <example>5</example>>
    [Required(ErrorMessage = "O impacto da falha é necessário")]
    [Range(0, 6, ErrorMessage = "O impacto deve estar entre 0 e 6")]
    public int Impacto { get; set; }
    
    /// <summary>
    /// Se ja foi declarada parada ou não, um X representa a parada
    /// </summary>
    /// <example>X</example>
    public char? Parada { get; set; }
    
    /// <summary>
    /// Início do período de indisponibilidade do campo de seleção
    /// </summary>
    /// <example>DD/MM/AAAA</example>>
    public DateTime? InicioIndisp { get; set; }
    
    /// <summary>
    /// Fim do período de indisponibilidade do campo de seleção
    /// </summary>
    /// <example>DD/MM/AAAA</example>>
    public DateTime? FimIndisp { get; set; }
    
    /// <summary>
    /// Condição da intalação antes da avaria do campo de seleção, de 1 a 3
    /// </summary>
    [Range(1,3)]
    public int? CondInstAntAvar { get; set; }
        
    /// <summary>
    /// Condição da intalação depois da avaria do campo de seleção, de 1 a 3
    /// </summary>
    [Range(1,3)]
    public int? CondInstDepAvar { get; set; }
    
    /// <summary>
    /// Condição da intalação após a medição do campo de seleção, de 1 a 3
    /// </summary>
    [Range(1,3)]
    public int? CondInstAposMed { get; set; }
}