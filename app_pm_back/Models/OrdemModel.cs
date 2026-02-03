using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app_pm_back.Models;

public class OrdemModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdMongo { get; set; } // Id gerado pelo mongo
    public string IdNota { get; set; } // Id da nota 
    public string StatusNota { get; set; } // status da nota
    public string IdOrdem { get; set; } // Id da ordem

    public ObjetoReferencia ObjetoReferencia { get; set; } // objeto de referência
    
    public Responsabilidades Responsabilidades { get; set; } // responsabilidades da ordem
    
    public Database Database { get; set; } // datas estabelecidas para a ordem 
    
    public Situacao Situacao { get; set; } // situação da ordem
    
    public DdsAvaria DdsAvaria { get; set; } // inicio e fim planejado para a ordem
}

public class ObjetoReferencia
{
    public string LocalInstalacao { get; set; } // local de instalação do ativo (autopreencher)
    public string TAG { get; set; } // tag do ativo (autopreencher)
    public string? Conjunto { get; set; } // conjunto do ativo
}

public class Responsabilidades
{
    public (string tipo, string cod) GrupoPlanej { get; set; } // tipo e cod do grupo de planejamento
    public (string tipo, string cod) CenTrabPrinc { get; set; } // tipo e cod do centro de trabalho principal
    
    public (string cod, string gerencias) GerenciaSolid { get; set; } // cod e tags das gerências
    public (string cod, string nome) Reponsavel { get; set; } // cod e nome do responsável pela ordem
    
    public string Notificador { get; set; } // tipo de notificador
    public DateTime DataNota { get; set; } // data de criação da nota (autopreencher)
}

public class Database
{
    public DateTime InicioDesejado { get; set; } // data de início desejada (autopreencher)
    public DateTime ConclDesejado { get; set; } // data de conclusão desejada (autopreencher)
    public Prioridade Prioridade { get; set; } // nível de prioridade da ordem
}

public enum Prioridade
{
    Nenhuma = 0,
    Minima = 1,
    Pequena = 2,
    Baixa = 3,
    Normal = 4,
    Media = 5,
    Elevada = 6,
    Alta = 7,
    Urgente = 8,
    Critica = 9,
}

public class Situacao
{
    public (string sigla, string tag) ModoFalha { get; set; } // modo de falha da ordem
    public string Anomalia { get; set; } // anomalia no aparelho detectado
    public string Complemento { get; set; } // descrição complementar da anomalia 
}

public class DdsAvaria
{
    public DateTime InicioIndsp { get; set; }
    public DateTime FimIndisp { get; set; }
    public TimeOnly DuracaoParada { get; set; } // duração em horas
}
