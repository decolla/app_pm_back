using app_pm_back.Attributes;
using System.Collections.Generic;

namespace app_pm_back.DTO.ComgasDTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de importacao de notas de manutencao Comgas.
/// </summary>
public class NotaManutencaoComgasDto
{
    [SheetColumn("Id Nota")]
    public long IdNota { get; set; }

    [SheetColumn("Nota")]
    public long NumeroNota { get; set; }

    [SheetColumn("Tipo de nota")]
    public string? TipoNota { get; set; }

    [SheetColumn("Classificação")]
    public string? Classificacao { get; set; }

    [SheetColumn("Método classificação")]
    public string? MetodoClassificacao { get; set; }

    [SheetColumn("Classificado por")]
    public string? ClassificadoPor { get; set; }
    
    [SheetColumn("Plano manut.")]
    public long? PlanoManutencaoCodigo { get; set; }

    [SheetColumn("Data classificação")]
    public DateTime? DataClassificacao { get; set; }

    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    [SheetColumn("Texto")]
    public string? DescricaoDetalhada { get; set; }

    [SheetColumn("Status sistema")] 
    public string? StatusNota { get; set; } = "Aberta";

    [SheetColumn("Data da nota")]
    public DateTime DataNota { get; set; }
    
    [SheetColumn("Data fim")]
    public DateTime? DataFim { get; set; }

    [SheetColumn("Prioridade")]
    public int Prioridade { get; set; }

    [SheetColumn("Início desejado")]
    public DateTime? InicioDesejado { get; set; }

    [SheetColumn("Concl.desejada")]
    public DateTime? DateTime { get; set; }

    [SheetColumn("Equipamento")]
    public string? EquipamentoTag { get; set; }

    [SheetColumn("Loc.instalação")]
    public int? LocalInstalacaoCodigo { get; set; }

    [SheetColumn("Criado por")]
    public int? CriadoPorUsuarioId { get; set; }
    
    [SheetColumn("Área operacion.")]
    public string? AreaOperacional { get; set; }
    
    [SheetColumn("Referência externa")]
    public string? ReferenciaLocalizacaoAtivo { get; set; }
    
    public string? CategoriaNota { get; set; }
    
    [SheetColumn("InícioAvar")]
    public DateTime? DataAvaria { get; set; }

    /// <summary>
    /// Hora de início da avaria. No caso de notas sem avaria, ou com hora de início de avaria desconhecida, o valor será 00:00:00.
    /// </summary>
    /// <example>01:30:00</example>
    [SheetColumn("HoraInícioAvar.")] 
    public TimeSpan? HoraAvaria { get; set; } = 
        new TimeSpan(0, 0, 0, 0);
    
    public string? EquipamentoDisponivel { get; set; }
    
    public bool? AnaliseRiscoOperacional { get; set; }
    
    public string? AnomaliaObservada { get; set; }
    
    public string? ObservacoesComplementares { get; set; }
    
    [SheetColumn("Bairro")]
    public string? PontoReferencia { get; set; }

    [SheetColumn("Tipo prioridade")]
    public string? TipoPrioridade { get; set; }

    [SheetColumn("Tipo de nota")]
    public string? TipoDeNota { get; set; }

    [SheetColumn("Txt.grp.codific")]
    public string? TxtGrpCodific { get; set; }

    [SheetColumn("Txt.code codif.")]
    public string? TxtCodeCodif { get; set; }

    [SheetColumn("Codificação")]
    public string? Codificacao { get; set; }

    [SheetColumn("Conseqüência")]
    public int? Consequencia { get; set; }

    [SheetColumn("Grupo de codes")]
    public string? GrupoDeCodes { get; set; }
    
    [SheetColumn("Finalidade")]
    public string? Finalidade { get; set; }

    public List<ItemNotaManutencaoComgasDto> Itens { get; set; } = new();

    public List<VariavelNotaComgasDto> Variaveis { get; set; } = new();

    public List<MedidaNotaComgasDto> Medidas { get; set; } = new();

    public List<AtividadeNotaComgasDto> Atividades { get; set; } = new();
}

#pragma warning restore CS1591
