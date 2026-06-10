using System;
using app_pm_back.Models.Domain;

namespace app_pm_back.Models;
/// <summary>
/// Classe BuscaOrdemNotaDto.
/// </summary>

public class BuscaOrdemNotaDto
{
    /// <summary>
    /// Status da nota
    /// </summary>
    /// <example>Aberta</example>
    public Status? StatusNota { get; set; }

    /// <summary>
    /// Id de início da nota
    /// </summary>
    public int? NotaIdInicio { get; set; }

    /// <summary>
    /// Id de fim da nota
    /// </summary>
    public int? NotaIdFim { get; set; }

    public int? OrdemIdInicio { get; set; }
    public int? OrdemIdFim { get; set; }

    public int? LocalInstalacaoInicio { get; set; }
    public int? LocalInstalacaoFim { get; set; }

    public string? AtivoInicio { get; set; }
    public string? AtivoFim { get; set; }

    public DateTime? NotaDataInicio { get; set; }
    public DateTime? NotaDataFim { get; set; }

    public DateTime? OrdemDataInicio { get; set; }
    public DateTime? OrdemDataFim { get; set; }

    public int? GpmInicio { get; set; }
    public int? GpmFim { get; set; }

    public Prioridade? PrioridadeInicio { get; set; }
    public Prioridade? PrioridadeFim { get; set; }

    public string? CriadoPorInicio { get; set; }
    public string? CriadoPorFim { get; set; }

    public string? TipoNota { get; set; }
    public string? TipoOrdem { get; set; }

    public Status? StatusOrdem { get; set; }
}
