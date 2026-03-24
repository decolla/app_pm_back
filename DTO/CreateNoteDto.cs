/*using app_pm_back.Models;

namespace app_pm_back.DTO;

// protege de campos gerados pelo sistema 
public class CreateNotaDto
{
    public required DadosGeraisDto DadosGeraisDto { get; set; }
    public required CaracteristicasGerais CaracteristicasGerais { get; set; }

    public required AutorNota AutorNota { get; set; } // características do autor da nota

    public AnaliseRiscoOperacional?
        AnaliseRiscoOperacional { get; set; } // se apresenta risco operacional em caso de elemento crítico

    public required Observacoes Observacoes { get; set; } // Observações da nota pelo autor

    public required CheckList CheckList { get; set; } // checklist do equipamento, pode marcar os campos se eles se aplicam ou não

    public required string Disponibilidade { get; set; } // O equipamento está disponível
    public DateTime DataAvaria { get; set; } // Data e hora de avaria do equipamento (pode ser nulo)
}

public class DadosGeraisDto
{
    public TipoNota TipoNota { get; set; } // Nota de Manutenção ou Solicitação de Inspeção
    public required string Tag { get; set; } // tag do ativo

    public required string LocalInstalacao { get; set; } // local de instalação do ativo (autopreencher)
    public required string DenominacaoLocal { get; set; } // denominação do local de instalação do ativo (autopreencher)

    public required string Complemento { get; set; } // complemento do local de instalação do ativo 

    public string? Titulo { get; set; } // título da nota para ordem
}*/