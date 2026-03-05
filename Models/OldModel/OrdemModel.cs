/*using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace app_pm_back.Models;

public class OrdemModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } // Id gerado pelo mongo
    public required string? IdNota { get; set; } // Id da nota 
    public string? StatusNota { get; set; } // status da nota
    public required string IdOrdem { get; set; } // Id da ordem

    public required ObjetoReferencia ObjetoReferencia { get; set; } // objeto de referência
    
    public required Responsabilidades Responsabilidades { get; set; } // responsabilidades da ordem
    
    public required Database Database { get; set; } // datas estabelecidas para a ordem 
    
    public required Situacao Situacao { get; set; } // situação da ordem
    
    public required DdsAvaria DdsAvaria { get; set; } // inicio e fim planejado para a ordem
}

public class ObjetoReferencia
{
    public required string LocalInstalacao { get; set; } // local de instalação do ativo (autopreencher)
    public required string TAG { get; set; } // tag do ativo (autopreencher)
    public string? Conjunto { get; set; } // conjunto do ativo
}

public class Responsabilidades
{
    public required GrupoPlanej GrupoPlanej { get; set; } // tipo e cod do grupo de planejamento
    public required CenTrabPrinc CenTrabPrinc { get; set; } // tipo e cod do centro de trabalho principal
    
    public required GerenciaSolid GerenciaSolid { get; set; } // cod e tags das gerências
    public required Responsavel Responsavel { get; set; } // cod e nome do responsável pela ordem (autopreencher)
    
    public required string Notificador { get; set; } // tipo de notificador
    public DateTime DataAvaria { get; set; } // data de avaria da nota (autopreencher)
}

public class GrupoPlanej
{
    public required string Tipo { get; set; } // tipo do grupo de planejamento
    public required string Cod { get; set; } // código do grupo de planejamento
}

public class CenTrabPrinc
{
    public required string Tipo { get; set; } // tipo do centro de trabalho principal
    public required string Cod { get; set; } // código do centro de trabalho principal
}

public class GerenciaSolid
{
    public required string Cod { get; set; } // código da gerência por executar a ordem
    public required string Tags { get; set; } // tags das gerências que executam a ordem
}

public class Responsavel
{
    public required string Cod { get; set; } // código do responsável pela ordem
    public required string Nome { get; set; } // nome do responsável pela ordem
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
    public required ModoFalha ModoFalha { get; set; } // modo de falha da ordem
    public required string Anomalia { get; set; } // anomalia no aparelho detectado
    public required string Complemento { get; set; } // descrição complementar da anomalia 
}

public class ModoFalha
{
    public required string Sigla { get; set; } // sigla do modo de falha
    public required string Tag { get; set; } // tag do modo de falha
}

public class DdsAvaria
{
    public DateTime InicioIndsp { get; set; } // quanto tempo ficara indisponível
    public DateTime FimIndisp { get; set; }
    public TimeSpan DuracaoParada { get; set; } // duração em horas
}
*/