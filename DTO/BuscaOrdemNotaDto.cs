using System;
using app_pm_back.Models.Domain;

namespace app_pm_back.Models;
/// <summary>
/// Classe BuscaOrdemNotaDto.
/// </summary>

public class BuscaOrdemNotaDto
{
    /// <summary>
    /// StatusFichas da nota
    /// </summary>
    /// <example>Aberta</example>
    public StatusFichas? StatusNota { get; set; }

    /// <summary>
    /// Id de início da nota
    /// </summary>
    public long? NotaIdInicio { get; set; }

    /// <summary>
    /// Id de fim da nota
    /// </summary>
    public long? NotaIdFim { get; set; }

    public long? OrdemIdInicio { get; set; }
    public long? OrdemIdFim { get; set; }

    public string? LocalInstalacaoInicio { get; set; }
    public string? LocalInstalacaoFim { get; set; }

    public List<ComponentType>? ListaAtivos { get; set; }

    public DateTime? NotaDataInicio { get; set; }
    public DateTime? NotaDataFim { get; set; }

    public DateTime? OrdemDataInicio { get; set; }
    public DateTime? OrdemDataFim { get; set; }

    public long? GpmInicio { get; set; }
    public long? GpmFim { get; set; }

    public Prioridade? PrioridadeInicio { get; set; }
    public Prioridade? PrioridadeFim { get; set; }

    public string? CriadoPor { get; set; }

    public Classificacao? Classificacao { get; set; }

    public StatusFichas? StatusOrdem { get; set; }
}
