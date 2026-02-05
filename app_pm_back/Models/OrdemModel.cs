using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app_pm_back.Models;

public class OrdemModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; } // Id gerado pelo mongo
    public string IdNota { get; set; } // Id da nota 
    public string? StatusNota { get; set; } // status da nota
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
    public GrupoPlanej GrupoPlanej { get; set; } // tipo e cod do grupo de planejamento
    public CenTrabPrinc CenTrabPrinc { get; set; } // tipo e cod do centro de trabalho principal
    
    public GerenciaSolid GerenciaSolid { get; set; } // cod e tags das gerências
    public Responsavel Responsavel { get; set; } // cod e nome do responsável pela ordem (autopreencher)
    
    public string Notificador { get; set; } // tipo de notificador
    public DateTime DataAvaria { get; set; } // data de avaria da nota (autopreencher)
}

public class GrupoPlanej
{
    public string Tipo { get; set; } // tipo do grupo de planejamento
    public string Cod { get; set; } // código do grupo de planejamento
}

public class CenTrabPrinc
{
    public string Tipo { get; set; } // tipo do centro de trabalho principal
    public string Cod { get; set; } // código do centro de trabalho principal
}

public class GerenciaSolid
{
    public string Cod { get; set; } // código da gerência por executar a ordem
    public string Tags { get; set; } // tags das gerências que executam a ordem
}

public class Responsavel
{
    public string Cod { get; set; } // código do responsável pela ordem
    public string Nome { get; set; } // nome do responsável pela ordem
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
    public ModoFalha ModoFalha { get; set; } // modo de falha da ordem
    public string Anomalia { get; set; } // anomalia no aparelho detectado
    public string Complemento { get; set; } // descrição complementar da anomalia 
}

public class ModoFalha
{
    public string Sigla { get; set; } // sigla do modo de falha
    public string Tag { get; set; } // tag do modo de falha
}

public class DdsAvaria
{
    public DateTime InicioIndsp { get; set; } // quanto tempo ficara indisponível
    public DateTime FimIndisp { get; set; }
    public TimeSpan DuracaoParada { get; set; } // duração em horas
}
