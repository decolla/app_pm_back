using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app_pm_back.Models;

public class NotaModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } // Id gerado pelo mongo

    public string IdOrdem { get; set; }

    public DadosGerais DadosGerais { get; set; } // dados gerais da nota 

    public CaracteristicasGerais CaracteristicasGerais { get; set; } // características gerais da nota

    public AutorNota AutorNota { get; set; } // características do autor da nota

    public AnaliseRiscoOperacional?
        AnaliseRiscoOperacional { get; set; } // se apresenta risco operacional em caso de elemento crítico

    public Observacoes Observacoes { get; set; } // Observações da nota pelo autor

    public CheckList
        CheckList { get; set; } // checklist do equipamento, pode marcar os campos se eles se aplicam ou não

    public string Disponibilidade { get; set; } // O equipamento está disponível
    public DateTime DataAvaria { get; set; } // Data e hora de avaria do equipamento (pode ser nulo)
}

public enum TipoNota
{
    Manutencao = 0,
    Inspecao = 1
}

public class DadosGerais
{
    public TipoNota TipoNota { get; set; } // Nota de Manutenção ou Solicitação de Inspeção
    public string TAG { get; set; } // tag do ativo

    public string LocalInstalacao { get; set; } // local de instalação do ativo (autopreencher)
    public string DenominacaoLocal { get; set; } // denominação do local de instalação do ativo (autopreencher)

    public string Complemento { get; set; } // complemento do local de instalação do ativo 

    public string? Status { get; set; } // status da nota para ordem (autopreencher)
    public string? Titulo { get; set; } // título da nota para ordem
    public string? NroOrdem { get; set; } // número da ordem 
}

public class CaracteristicasGerais
{
    public string NroEtiqueta { get; set; } = "0000000000"; // número da ediqueta

    public string Finalidade { get; set; } // finalidade da nota
    public string OutroCritico { get; set; } // outro crítico?

    public DateTime DataNota { get; set; } // data de criação da nota 
    public DateTime FimDesejado { get; set; } // data de fim desejada na nota
}

public class AutorNota
{
    public string AreaOperacional { get; set; } // area do operador autor
    public string GPM { get; set; } // 
    public string AutorChave { get; set; } // Chave do autor da nota
}

public class AnaliseRiscoOperacional
{
    public string? Risco { get; set; } // string descrevendo o risco
    public bool? Realizado { get; set; } = false; // se o risco é Realizado AC ou ARO não aplicável
}

public class Observacoes
{
    public string? Anomalia { get; set; } // anomalia observada no ativo
    public string? Complemento { get; set; } // Observações complementares
    public string? PontoRef { get; set; } // ponto de referência 
}

public class CheckList
{
    public string? MatrizLibra { get; set; } = "";
    public float? Temperatura { get; set; } = 0;
    public float? Tensão { get; set; } = 0;
    public string? Fluído { get; set; } = "";
    public float? Pressao { get; set; } = 0;
    public string? Plaqueta { get; set; } = "000000000";
}