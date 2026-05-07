using app_pm_back.Attributes;

namespace app_pm_back.DTO.ComgasDTO;


/// <summary>
/// DTO de endereco para importacao de localizacao.
/// </summary>
public class EnderecoComgasDto
{
    [SheetColumn("Nº endereço")]
    public int IdEndereco { get; set; }

    [SheetColumn("Rua")]
    public string? Rua { get; set; }

    [SheetColumn("Código postal")]
    public string? Cep { get; set; }

    [SheetColumn("Local")]
    public string? Cidade { get; set; }

    [SheetColumn("Estado")]
    public string? Estado { get; set; }

    [SheetColumn("País")]
    public string? Pais { get; set; }

    [SheetColumn("Telefone")]
    public string? Telefone { get; set; }
}

