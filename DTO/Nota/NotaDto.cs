using app_pm_back.Attributes;
using System.Collections.Generic;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de importacao de notas de manutencao .
/// </summary>
public class NotaDto
{
    [SheetColumn("Nota")] public long NumeroNota { get; set; }

    [SheetColumn("N/A")] public string? TipoNota { get; set; }

    [SheetColumn("cdnotamanutencao")] public string? SapCode { get; set; }

    [SheetColumn("Classificação")] public Classificacao Classificacao { get; set; } = Classificacao.NAOCLASSIFICADA;

    [SheetColumn("Método classificação")]
    public MetodoClassificacao? MetodoClassificacao { get; set; }

    [SheetColumn("Classificado por")] public string? ClassificadoPor { get; set; }

    [SheetColumn("DnsIot")] public string? DnsIot { get; set; }

    [SheetColumn("Gpm")] public long? IdGpm { get; set; }

    [SheetColumn("Data encermto.")] public DateTime? DataFim { get; set; }
    [SheetColumn("Plano manut.")] public long? IdPlano { get; set; }

    [SheetColumn("Data classificação")] public DateTime? DataClassificacao { get; set; }

    [SheetColumn("status_nota")] public StatusFichas? StatusNota { get; set; }

    [SheetColumn("Data da nota")] public DateTime? DataNota { get; set; }

    [SheetColumn("Prioridade")] public Prioridade? Prioridade { get; set; }

    [SheetColumn("Fim avaria")] public DateTime? FimDesejado { get; set; }

    [SheetColumn("Txt.code codif.")] public List<ComponentType>? IdsAtivo { get; set; }

    [SheetColumn("Loc.instalação")] public string? IdLi { get; set; }

    [SheetColumn("Criado por")] public string? CriadaPorUsuario { get; set; }

    [SheetColumn("Modificado por")] public string? ModificadaPorUsuario { get; set; }

    [SheetColumn("Data modificacao")] public DateTime? DataModificacao { get; set; }

    [SheetColumn("Denominacao")] public string? ReferenciaLocalizacaoAtivo { get; set; }

    [SheetColumn("Tipo de nota")] public string? Finalidade { get; set; }

    [SheetColumn("InícioAvar")] public DateTime? DataAvaria { get; set; }

    [SheetColumn("Prioridade")] public string? EquipamentoDisponivel { get; set; }

    [SheetColumn("CondInstApósMed")] public ConfiabilidadeRestaurada? ConfiabilidadeRestaurada { get; set; }

    [SheetColumn("Descrição")] public string? AnomaliaObservada { get; set; }

    [SheetColumn("Txt.code codif.")] public string? ObservacoesComplementares { get; set; }

    [SheetColumn("Txt.grp.codific")] public string? DescricaoBreve { get; set; }
    
    [SheetColumn("TEXTO DESCRICAO LONGO (PROCV)")] public string? DescricaoLonga { get; set; }

    [SheetColumn("Id Usuario")] public int? IdUsuario { get; set; }

    [SheetColumn("Escopo")] public long? Escopo { get; set; }

    [SheetColumn("Corretiva?")] public bool IndicadorCorretiva { get; set; }

    public List<VariavelNotaDto>? Variaveis { get; set; } = new List<VariavelNotaDto>();

    public List<ItemNotaManutencaoDto>? Itens { get; set; } = new List<ItemNotaManutencaoDto>();

    public List<AnexoNotaDto>? Anexos { get; set; } = new List<AnexoNotaDto>();
}

#pragma warning restore CS1591