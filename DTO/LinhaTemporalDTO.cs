using app_pm_back.Models.Domain;

namespace app_pm_back.DTO.Restauracao;

/// <summary>
/// Item da linha temporal retornado para o frontend
/// </summary>
public class LinhaTemporalDTO
{
    /// <summary>
    /// ID da Localização de Instalação (LI)
    /// </summary>
    public string IdLi { get; set; } = string.Empty;

    /// <summary>
    /// DNS IoT do ativo (nome alternativo)
    /// </summary>
    public string? DnsIot { get; set; }

    /// <summary>
    /// Data de referência do ativo (vinda da API externa)
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Lista de eventos (notas e ordens) relacionados a esta LI
    /// </summary>
    public List<EventoLinhaTemporalDTO> Eventos { get; set; } = new();
}

/// <summary>
/// Evento individual na linha temporal (nota ou ordem)
/// </summary>
public class EventoLinhaTemporalDTO
{
    /// <summary>
    /// Número da nota ou ordem (ex: "123" ou "OS-2024-001")
    /// </summary>
    public string Numero { get; set; } = string.Empty;

    /// <summary>
    /// Data do evento (DataNota para notas, DataCriacao para ordens)
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Tipo do evento: "Nota" ou "Ordem"
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// ID de referência (IdNota ou IdOrdem)
    /// </summary>
    public long IdReferencia { get; set; }

    /// <summary>
    /// Descrição breve do evento
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Classificação do evento (Preventiva, Corretiva, etc.)
    /// </summary>
    public string? Classificacao { get; set; }

    /// <summary>
    /// Se a confiabilidade foi restaurada
    /// </summary>
    public string? ConfiabilidadeRestaurada { get; set; }
}

/// <summary>
/// Request para buscar linha temporal
/// </summary>
public class LinhaTemporalRequestDTO
{
    /// <summary>
    /// Lista de IDs das LIs (Localizações de Instalação)
    /// </summary>
    public List<string> IdsLi { get; set; } = new();
}