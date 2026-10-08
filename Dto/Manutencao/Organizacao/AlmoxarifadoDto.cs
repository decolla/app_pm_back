using System.ComponentModel.DataAnnotations;
using app_pm_back.Attributes;
using app_pm_back.Models.Domain;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de almoxarifado .
/// </summary>
public class AlmoxarifadoDto
{
    // Identificacao
    [SheetColumn("Id Almoxarifado")] public long? IdAlmoxarifado { get; set; }

    [SheetColumn("Nome")] public string? Nome { get; set; }

    [SheetColumn("Organizacao")] public string? Organizacao { get; set; }

    // StatusFichas e descricao (opcionais)
    [EnumDataType(typeof(StatusAlmoxarifado))]
    [SheetColumn("StatusFichas")] public StatusAlmoxarifado? Status { get; set; }

    [SheetColumn("DescricaoBreve")] public string? Descricao { get; set; }

    // Escopo externo (armazenar apenas o ID)
    [SheetColumn("Id Escopo")] public string? IdEscopo { get; set; }

    /// <summary>
    /// Identificadores das ferramentas que serão vinculadas ao almoxarifado.
    /// </summary>
    public List<long> IdsFerramentas { get; set; } = new();

    /// <summary>
    /// Identificadores dos usuários que serão vinculados ao almoxarifado.
    /// </summary>
    public List<long> IdsUsuarios { get; set; } = new();
}

#pragma warning restore CS1591