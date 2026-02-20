using app_pm_back.Models;

// protege de campos gerados pelo sistema 
public class CreateNotaDto
{
    public DadosGerais DadosGeraisDTO { get; set; }
    public CaracteristicasGerais CaracteristicasGerais { get; set; }

    public AutorNota AutorNota { get; set; } // características do autor da nota

    public AnaliseRiscoOperacional?
        AnaliseRiscoOperacional { get; set; } // se apresenta risco operacional em caso de elemento crítico

    public Observacoes Observacoes { get; set; } // Observações da nota pelo autor

    public CheckList CheckList { get; set; } // checklist do equipamento, pode marcar os campos se eles se aplicam ou não

    public string Disponibilidade { get; set; } // O equipamento está disponível
    public DateTime DataAvaria { get; set; } // Data e hora de avaria do equipamento (pode ser nulo)
}

public class DadosGeraisDTO
{
    public TipoNota TipoNota { get; set; } // Nota de Manutenção ou Solicitação de Inspeção
    public string TAG { get; set; } // tag do ativo

    public string LocalInstalacao { get; set; } // local de instalação do ativo (autopreencher)
    public string DenominacaoLocal { get; set; } // denominação do local de instalação do ativo (autopreencher)

    public string Complemento { get; set; } // complemento do local de instalação do ativo 

    public string? Titulo { get; set; } // título da nota para ordem
}
