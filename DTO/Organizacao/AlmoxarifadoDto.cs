using app_pm_back.Attributes;

namespace app_pm_back.DTO;

#pragma warning disable CS1591

/// <summary>
/// DTO de almoxarifado .
/// </summary>
public class AlmoxarifadoDto
{
    // Identificacao
    [SheetColumn("Id Almoxarifado")] public long IdAlmoxarifado { get; set; }

    [SheetColumn("Nome")] public required string Nome { get; set; }

    [SheetColumn("Organizacao")] public required string Organizacao { get; set; }

    // StatusFichas e descricao (opcionais)
    [SheetColumn("StatusFichas")] public string? Status { get; set; }

    [SheetColumn("DescricaoBreve")] public string? Descricao { get; set; }

    // Escopo externo (armazenar apenas o ID)
    [SheetColumn("Id Escopo")] public long IdEscopo { get; set; }
}

#pragma warning restore CS1591