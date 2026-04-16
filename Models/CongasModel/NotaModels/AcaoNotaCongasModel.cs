using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;

namespace app_pm_back.Models.CongasModel;

public class AcaoNotaCongasModel
{
    /// <summary>
    /// Id da ação
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdAcao { get; set; }

    /// <summary>
    /// Área operacional da ação de nota 
    /// </summary>
    [SheetColumn("Área operacion.")]
    public int? AreaOperac { get; set; }
    
    /// <summary>
    /// Atividade
    /// </summary>
    [SheetColumn("Atividade")]
    public string? Atividade { get; set; }

    /// <summary>
    /// Tipo de catálogo
    /// </summary>
    [SheetColumn("TpCatálogo")]
    public string? TpCatalogo { get; set; }

    /// <summary>
    /// Bairro
    /// </summary>
    [SheetColumn("Bairro")]
    public string? Bairro { get; set; }

    /// <summary>
    /// Canal de distribuição
    /// </summary>
    [SheetColumn("Canal distrib.")]
    public string? CanalDistrib { get; set; }

    /// <summary>
    /// Centro de custo
    /// </summary>
    [SheetColumn("Centro custo")]
    public string? CentroCusto { get; set; }

    /// <summary>
    /// Centro de localização
    /// </summary>
    [SheetColumn("Cen.localiz.")]
    public string? CenLocaliz { get; set; }

    /// <summary>
    /// Centro para centro de trabalho
    /// </summary>
    [SheetColumn("Cen.p/cen.trab.")]
    public string? CenPCenTrab { get; set; }

    /// <summary>
    /// Centro de planejamento
    /// </summary>
    [SheetColumn("Centro planej.")]
    public string? CentroPlanej { get; set; }

    /// <summary>
    /// Centro de trabalho responsável
    /// </summary>
    [SheetColumn("CenTrab respon.")]
    public string? CenTrabRespon { get; set; }

    /// <summary>
    /// País
    /// </summary>
    [SheetColumn("País")]
    public string? Pais { get; set; }

    /// <summary>
    /// Classe de defeitos
    /// </summary>
    [SheetColumn("Classe defeitos")]
    public string? ClasseDefeitos { get; set; }

    /// <summary>
    /// Cliente
    /// </summary>
    [SheetColumn("Cliente")]
    public string? Cliente { get; set; }

    /// <summary>
    /// Codificação
    /// </summary>
    [SheetColumn("Codificação")]
    public string? Codificacao { get; set; }

    /// <summary>
    /// Código parte objeto
    /// </summary>
    [SheetColumn("Code parte obj.")]
    public string? CodeParteObj { get; set; }

    /// <summary>
    /// Código ABC
    /// </summary>
    [SheetColumn("Código ABC")]
    public string? CodigoABC { get; set; }

    /// <summary>
    /// Código de ação
    /// </summary>
    [SheetColumn("Código de ação")]
    public string? CodigoDeAcao { get; set; }

    /// <summary>
    /// Código do dano
    /// </summary>
    [SheetColumn("Código do dano")]
    public string? CodigoDoDano { get; set; }

    /// <summary>
    /// Código postal
    /// </summary>
    [SheetColumn("Código postal")]
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Conclusão desejada
    /// </summary>
    [SheetColumn("Concl.desejada")]
    public DateTime? ConclDesejada { get; set; }

    /// <summary>
    /// Condição de instalação antes da avaria
    /// </summary>
    [SheetColumn("CondInstAntAvar")]
    public string? CondInstAntAvar { get; set; }

    /// <summary>
    /// Condição de instalação depois da avaria
    /// </summary>
    [SheetColumn("CondInstDepAvar")]
    public string? CondInstDepAvar { get; set; }

    /// <summary>
    /// Condição de instalação após medida
    /// </summary>
    [SheetColumn("CondInstApósMed")]
    public string? CondInstAposMed { get; set; }

    /// <summary>
    /// Conjunto (primeira ocorrência)
    /// </summary>
    [SheetColumn("Conjunto")]
    public string? Conjunto1 { get; set; }

    /// <summary>
    /// Conjunto (segunda ocorrência)
    /// </summary>
    [SheetColumn("Conjunto")]
    public string? Conjunto2 { get; set; }

    /// <summary>
    /// Consequência
    /// </summary>
    [SheetColumn("Conseqüência")]
    public string? Consequencia { get; set; }

    /// <summary>
    /// Controle técnico em
    /// </summary>
    [SheetColumn("Control.técn_em")]
    public DateTime? ControlTecnEm { get; set; }

    /// <summary>
    /// Controle técnico de
    /// </summary>
    [SheetColumn("Contr.técn.de")]
    public DateTime? ContrTecnDe { get; set; }

    /// <summary>
    /// Criado às
    /// </summary>
    [SheetColumn("Criado às")]
    public DateTime? CriadoAs { get; set; }

    /// <summary>
    /// Criado por
    /// </summary>
    [SheetColumn("Criado por")]
    public string? CriadoPor { get; set; }

    /// <summary>
    /// Data da nota
    /// </summary>
    [SheetColumn("Data da nota")]
    public DateTime? DataDaNota { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    [SheetColumn("Dt.criação")]
    public DateTime? DtCriacao { get; set; }

    /// <summary>
    /// Início
    /// </summary>
    [SheetColumn("Início")]
    public DateTime? Inicio { get; set; }

    /// <summary>
    /// Data de referência
    /// </summary>
    [SheetColumn("Dt.referência")]
    public DateTime? DtReferencia { get; set; }

    /// <summary>
    /// Data do pedido
    /// </summary>
    [SheetColumn("Data pedido")]
    public DateTime? DataPedido { get; set; }

    /// <summary>
    /// Fim
    /// </summary>
    [SheetColumn("Fim")]
    public DateTime? Fim { get; set; }

    /// <summary>
    /// Dados do dispositivo
    /// </summary>
    [SheetColumn("Dds.dispositivo")]
    public string? DdsDispositivo { get; set; }

    /// <summary>
    /// Denominação do conjunto (primeira ocorrência)
    /// </summary>
    [SheetColumn("Denom.conjunto")]
    public string? DenomConjunto1 { get; set; }

    /// <summary>
    /// Denominação do conjunto (segunda ocorrência)
    /// </summary>
    [SheetColumn("Denom.conjunto")]
    public string? DenomConjunto2 { get; set; }

    /// <summary>
    /// Denominação (primeira ocorrência)
    /// </summary>
    [SheetColumn("Denominação")]
    public string? Denominacao1 { get; set; }

    /// <summary>
    /// Denominação (segunda ocorrência)
    /// </summary>
    [SheetColumn("Denominação")]
    public string? Denominacao2 { get; set; }

    /// <summary>
    /// Descrição
    /// </summary>
    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Dispositivo antes da avaria
    /// </summary>
    [SheetColumn("DispAntesAvaria")]
    public string? DispAntesAvaria { get; set; }

    /// <summary>
    /// Dispositivo após medida
    /// </summary>
    [SheetColumn("Disp.após med.")]
    public string? DispAposMed { get; set; }

    /// <summary>
    /// Dispositivo depois da avaria
    /// </summary>
    [SheetColumn("Disp.depoisAvar")]
    public string? DispDepoisAvar { get; set; }

    /// <summary>
    /// Divisão
    /// </summary>
    [SheetColumn("Divisão")]
    public string? Divisao { get; set; }

    /// <summary>
    /// Documento de vendas
    /// </summary>
    [SheetColumn("Doc.vendas")]
    public string? DocVendas { get; set; }

    /// <summary>
    /// Duração da parada
    /// </summary>
    [SheetColumn("Duraç.parada")]
    public string? DuracParada { get; set; }

    /// <summary>
    /// Empresa
    /// </summary>
    [SheetColumn("Empresa")]
    public string? Empresa { get; set; }

    /// <summary>
    /// Data de encerramento
    /// </summary>
    [SheetColumn("Data encermto.")]
    public DateTime? DataEncermto { get; set; }

    /// <summary>
    /// Equipamento
    /// </summary>
    [SheetColumn("Equipamento")]
    public string? Equipamento { get; set; }

    /// <summary>
    /// Equipamento afetado
    /// </summary>
    [SheetColumn("Equipam.afetado")]
    public string? EquipamAfetado { get; set; }

    /// <summary>
    /// Fator de quantidade
    /// </summary>
    [SheetColumn("Fator quantidad")]
    public decimal? FatorQuantidad { get; set; }

    /// <summary>
    /// Fim da avaria
    /// </summary>
    [SheetColumn("Fim avaria")]
    public DateTime? FimAvaria { get; set; }

    /// <summary>
    /// Grupo de códigos (primeira ocorrência)
    /// </summary>
    [SheetColumn("GrpCódigos")]
    public string? GrpCodigos1 { get; set; }

    /// <summary>
    /// Grupo de códigos (segunda ocorrência)
    /// </summary>
    [SheetColumn("GrpCódigos")]
    public string? GrpCodigos2 { get; set; }

    /// <summary>
    /// Grupo de planejamento PM
    /// </summary>
    [SheetColumn("Grp.plnj.PM")]
    public string? GrpPlnjPM { get; set; }

    /// <summary>
    /// Grupo de codes
    /// </summary>
    [SheetColumn("Grupo de codes")]
    public string? GrupoDeCodes { get; set; }

    /// <summary>
    /// Hora (primeira ocorrência)
    /// </summary>
    [SheetColumn("Hora")]
    public TimeSpan? Hora1 { get; set; }

    /// <summary>
    /// Hora (segunda ocorrência)
    /// </summary>
    [SheetColumn("Hora")]
    public TimeSpan? Hora2 { get; set; }

    /// <summary>
    /// Hora da nota
    /// </summary>
    [SheetColumn("Hora da nota")]
    public TimeSpan? HoraDaNota { get; set; }

    /// <summary>
    /// Hora de encerramento
    /// </summary>
    [SheetColumn("Hora encermto.")]
    public TimeSpan? HoraEncermto { get; set; }

    /// <summary>
    /// Hora de início desejado
    /// </summary>
    [SheetColumn("Hora iníc.des.")]
    public TimeSpan? HoraInicDes { get; set; }

    /// <summary>
    /// Modificado às
    /// </summary>
    [SheetColumn("Modificado às")]
    public DateTime? ModificadoAs { get; set; }

    /// <summary>
    /// Hora de referência
    /// </summary>
    [SheetColumn("Hora referência")]
    public TimeSpan? HoraReferencia { get; set; }

    /// <summary>
    /// Hora fim da avaria
    /// </summary>
    [SheetColumn("Hora fim avaria")]
    public TimeSpan? HoraFimAvaria { get; set; }

    /// <summary>
    /// Hora final desejada
    /// </summary>
    [SheetColumn("H final desej.")]
    public TimeSpan? HFinalDesej { get; set; }

    /// <summary>
    /// Hora de início da avaria
    /// </summary>
    [SheetColumn("HoraInícioAvar.")]
    public TimeSpan? HoraInicioAvar { get; set; }

    /// <summary>
    /// Idioma principal
    /// </summary>
    [SheetColumn("Idiom.principal")]
    public string? IdiomPrincipal { get; set; }

    /// <summary>
    /// Imobilizado
    /// </summary>
    [SheetColumn("Imobilizado")]
    public string? Imobilizado { get; set; }

    /// <summary>
    /// Início da avaria
    /// </summary>
    [SheetColumn("InícioAvar")]
    public DateTime? InicioAvar { get; set; }

    /// <summary>
    /// Início desejado
    /// </summary>
    [SheetColumn("Início desejado")]
    public DateTime? InicioDesejado { get; set; }

    /// <summary>
    /// Item (primeira ocorrência)
    /// </summary>
    [SheetColumn("Item")]
    public string? Item1 { get; set; }

    /// <summary>
    /// Item (segunda ocorrência)
    /// </summary>
    [SheetColumn("Item")]
    public string? Item2 { get; set; }

    /// <summary>
    /// Local
    /// </summary>
    [SheetColumn("Local")]
    public string? Local { get; set; }

    /// <summary>
    /// Local de instalação
    /// </summary>
    [SheetColumn("Loc.instalação")]
    public string? LocInstalacao { get; set; }

    /// <summary>
    /// Local de instalação afetado
    /// </summary>
    [SheetColumn("Loc.inst.afetad")]
    public string? LocInstAfetad { get; set; }

    /// <summary>
    /// Localização
    /// </summary>
    [SheetColumn("Localização")]
    public string? Localizacao { get; set; }

    /// <summary>
    /// Local/Classe contábil
    /// </summary>
    [SheetColumn("Local/ClasContb")]
    public string? LocalClasContb { get; set; }

    /// <summary>
    /// Material
    /// </summary>
    [SheetColumn("Material")]
    public string? Material { get; set; }

    /// <summary>
    /// Modificado em
    /// </summary>
    [SheetColumn("Modificado em")]
    public DateTime? ModificadoEm { get; set; }

    /// <summary>
    /// Modificado por
    /// </summary>
    [SheetColumn("Modificado por")]
    public string? ModificadoPor { get; set; }

    /// <summary>
    /// Moeda
    /// </summary>
    [SheetColumn("Moeda")]
    public string? Moeda { get; set; }

    /// <summary>
    /// Número de ordenação (primeira ocorrência)
    /// </summary>
    [SheetColumn("Nº de ordenação")]
    public string? NroDeOrdenacao1 { get; set; }

    /// <summary>
    /// Número de ordenação (segunda ocorrência)
    /// </summary>
    [SheetColumn("Nº de ordenação")]
    public string? NroDeOrdenacao2 { get; set; }

    /// <summary>
    /// Número de série
    /// </summary>
    [SheetColumn("Nº de série")]
    public string? NroDeSerie { get; set; }

    /// <summary>
    /// Número do pedido
    /// </summary>
    [SheetColumn("Nº do pedido")]
    public string? NroDoPedido { get; set; }

    /// <summary>
    /// Número de endereço
    /// </summary>
    [SheetColumn("Nº endereço")]
    public string? NroEndereco { get; set; }

    /// <summary>
    /// Nome da lista
    /// </summary>
    [SheetColumn("Nome lista")]
    public string? NomeLista { get; set; }

    /// <summary>
    /// Número do objeto
    /// </summary>
    [SheetColumn("Nº objeto")]
    public string? NroObjeto { get; set; }

    /// <summary>
    /// Nota
    /// </summary>
    [SheetColumn("Nota")]
    public string? Nota { get; set; }

    /// <summary>
    /// Notificador
    /// </summary>
    [SheetColumn("Notificador")]
    public string? Notificador { get; set; }

    /// <summary>
    /// Ordem
    /// </summary>
    [SheetColumn("Ordem")]
    public string? Ordem { get; set; }

    /// <summary>
    /// Ordem do cliente
    /// </summary>
    [SheetColumn("Ordem cliente")]
    public string? OrdemCliente { get; set; }

    /// <summary>
    /// Organização de vendas
    /// </summary>
    [SheetColumn("Org.vendas")]
    public string? OrgVendas { get; set; }

    /// <summary>
    /// Parada
    /// </summary>
    [SheetColumn("Parada")]
    public string? Parada { get; set; }

    /// <summary>
    /// Grupo de códigos (terceira ocorrência)
    /// </summary>
    [SheetColumn("GrpCódigos")]
    public string? GrpCodigos3 { get; set; }

    /// <summary>
    /// Perfil de catálogo
    /// </summary>
    [SheetColumn("Perf.catálogo")]
    public string? PerfCatalogo { get; set; }

    /// <summary>
    /// Prioridade (primeira ocorrência)
    /// </summary>
    [SheetColumn("Prioridade")]
    public string? Prioridade1 { get; set; }

    /// <summary>
    /// Região
    /// </summary>
    [SheetColumn("Região")]
    public string? Regiao { get; set; }

    /// <summary>
    /// Rua
    /// </summary>
    [SheetColumn("Rua")]
    public string? Rua { get; set; }

    /// <summary>
    /// Setor de atividade
    /// </summary>
    [SheetColumn("Setor atividade")]
    public string? SetorAtividade { get; set; }

    /// <summary>
    /// Status do sistema
    /// </summary>
    [SheetColumn("Status sistema")]
    public string? StatusSistema { get; set; }

    /// <summary>
    /// Status do usuário
    /// </summary>
    [SheetColumn("Status usuário")]
    public string? StatusUsuario { get; set; }

    /// <summary>
    /// Subnúmero
    /// </summary>
    [SheetColumn("Subnº")]
    public string? SubNro { get; set; }

    /// <summary>
    /// Telefone
    /// </summary>
    [SheetColumn("Telefone")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Denominação (terceira ocorrência)
    /// </summary>
    [SheetColumn("Denominação")]
    public string? Denominacao3 { get; set; }

    /// <summary>
    /// Texto do código de ação
    /// </summary>
    [SheetColumn("Texto code ação")]
    public string? TextoCodeAcao { get; set; }

    /// <summary>
    /// Texto do código de codificação
    /// </summary>
    [SheetColumn("Txt.code codif.")]
    public string? TxtCodeCodif { get; set; }

    /// <summary>
    /// Texto do código parte objeto
    /// </summary>
    [SheetColumn("Txt.cod.part.ob")]
    public string? TxtCodPartOb { get; set; }

    /// <summary>
    /// Texto da atividade
    /// </summary>
    [SheetColumn("Txt.ativ.")]
    public string? TxtAtiv { get; set; }

    /// <summary>
    /// Texto do código de problema
    /// </summary>
    [SheetColumn("Txt.code prob.")]
    public string? TxtCodeProb { get; set; }

    /// <summary>
    /// Texto do grupo de código de ação
    /// </summary>
    [SheetColumn("Txt.grp.cod.aç.")]
    public string? TxtGrpCodAc { get; set; }

    /// <summary>
    /// Texto do grupo de código de problema
    /// </summary>
    [SheetColumn("Txt.grp.cod.pr.")]
    public string? TxtGrpCodPr { get; set; }

    /// <summary>
    /// Texto do grupo de código parte objeto
    /// </summary>
    [SheetColumn("TxtGrpCodPartOb")]
    public string? TxtGrpCodPartOb { get; set; }

    /// <summary>
    /// Texto do grupo de codificação
    /// </summary>
    [SheetColumn("Txt.grp.codific")]
    public string? TxtGrpCodific { get; set; }

    /// <summary>
    /// Texto descritivo
    /// </summary>
    [SheetColumn("Txt.descritivo")]
    public string? TxtDescritivo { get; set; }

    /// <summary>
    /// Texto
    /// </summary>
    [SheetColumn("Texto")]
    public string? Texto { get; set; }

    /// <summary>
    /// Prioridade (segunda ocorrência)
    /// </summary>
    [SheetColumn("Prioridade")]
    public string? Prioridade2 { get; set; }

    /// <summary>
    /// Tipo de nota
    /// </summary>
    [SheetColumn("Tipo de nota")]
    public string? TipoDeNota { get; set; }

    /// <summary>
    /// Tipo de prioridade
    /// </summary>
    [SheetColumn("Tipo prioridade")]
    public string? TipoPrioridade { get; set; }

    /// <summary>
    /// Unidade
    /// </summary>
    [SheetColumn("Unidade")]
    public string? Unidade { get; set; }

    /// <summary>
    /// Versão
    /// </summary>
    [SheetColumn("Versão")]
    public string? Versao { get; set; }
}