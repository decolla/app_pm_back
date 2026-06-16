using app_pm_back.Attributes;
using System.Collections.Generic;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de importacao de notas de manutencao .
/// </summary>
public class NotaManutencaoDto
{
    [SheetColumn("Id Nota")] public long IdNota { get; set; }

    // ordem
    // campo seleção
    // local instlação
    // loc instalação afetado
    // area operacional
    // centrabprinc
    // grupo planej
    //
    
    [SheetColumn("Nota")] public long NumeroNota { get; set; }

    [SheetColumn("Tipo de nota")] public string? TipoNota { get; set; }
    
    [SheetColumn("cdnotamanutencao")]public string? SapCode { get; set; }

    [SheetColumn("Classificação")] public Classificacao Classificacao { get; set; } = Classificacao.NAOCLASSIFICADA;

    [SheetColumn("Método classificação")] public MetodoClassificacao MetodoClassificacao { get; set; } = MetodoClassificacao.NAOCLASSIFICADA;

    [SheetColumn("Classificado por")] public string? ClassificadoPor { get; set; }

    [SheetColumn("Plano manut.")] public long? IdPlano { get; set; }

    [SheetColumn("Data classificação")] public DateTime? DataClassificacao { get; set; }

    [SheetColumn("status_nota")] public Status? StatusNota { get; set; }

    [SheetColumn("Data da nota")] public DateTime? DataNota { get; set; }

    [SheetColumn("Prioridade")] public string? Prioridade { get; set; }

    [SheetColumn("Concl.desejada")] public DateTime? FimDesejado { get; set; }

    [SheetColumn("Txt.code codif.")] public ComponentType? IdAtivo { get; set; }

    [SheetColumn("Loc.instalação")] public string? IdLi { get; set; }

    [SheetColumn("Criado por")] public string? CriadaPorUsuario { get; set; }

    [SheetColumn("Modificado por")] public string? ModificadaPorUsuario { get; set; }

    [SheetColumn("Data modificacao")] public DateTime? DataModificacao { get; set; }

    [SheetColumn("Referência externa")] public string? ReferenciaLocalizacaoAtivo { get; set; }

    [SheetColumn("Finalidade")] public string? Finalidade { get; set; }

    [SheetColumn("InícioAvar")] public DateTime? DataAvaria { get; set; }

    [SheetColumn("HoraInícioAvar.")] public TimeSpan? HoraAvaria { get; set; }

    [SheetColumn("Equipamento")] public string? EquipamentoDisponivel { get; set; }

    public ConfiabilidadeRestaurada? ConfiabilidadeRestaurada { get; set; }

    [SheetColumn("Descrição")] public string? AnomaliaObservada { get; set; }

    [SheetColumn("Txt.code codif.")] public string? ObservacoesComplementares { get; set; }

    [SheetColumn("Campo seleção")] public string? DescricaoBreve { get; set; }

    [SheetColumn("Id Usuario")] public int? IdUsuario { get; set; }
    
    [SheetColumn("Escopo")] public string? Escopo { get; set; } = "N/A";
}

#pragma warning restore CS1591