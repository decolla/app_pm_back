// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Text.Json.Serialization;
// using app_pm_back.Attributes;
//
// namespace app_pm_back.Models.CongasModel;
//
// public class ItemNotaCongasModel
// {
//     /// <summary>
//     /// Id do item
//     /// </summary>
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     public int IdItem { get; set; }
//     
//     /// <summary>
//     /// Id da nota de congas associada ao item
//     /// </summary>
//     public int? NoteCongasIdNota { get; set; }
//     
//     /// <summary>
//     /// Nota de congas relacionada ao item
//     /// </summary>
//     [JsonIgnore] // impede que o serializador tente ler a nota dentro do item 
//     public virtual NoteCongasModel? NoteCongas {get; set;}
//     
//     /// <summary>
//     /// Area operacional do item
//     /// </summary>
//     [SheetColumn("Área operacion")]
//     public int? AreaOperac { get; set; }
//     
//     /// <summary>
//     /// Bairro do item
//     /// </summary>
//     [SheetColumn("Bairro")]
//     public string? Bairro { get; set; }
//     
//     /// <summary>
//     /// Canal de distribuição do item
//     /// </summary>
//     [SheetColumn("Canal distrib.")]
//     public string? CanalDistrib { get; set; }
//     
//     /// <summary>
//     /// Causa da avaria do item
//     /// </summary>
//     [SheetColumn("Causa")]
//     public int? Causa { get; set; }
//
//     /// <summary>
//     /// Centro de custo do item
//     /// </summary>
//     [SheetColumn("Centro custo")]
//     public string? CentroCusto { get; set; }
//
//     /// <summary>
//     /// Centro de localização do item
//     /// </summary>
//     [SheetColumn("Cen.localiz.")]
//     public string? CenLocaliz { get; set; }
//
//     /// <summary>
//     /// Centro por centro de trabalho do item
//     /// </summary>
//     [SheetColumn("Cen.p/cen.trab.")]
//     public string? CenPCenTrab { get; set; }
//
//     /// <summary>
//     /// Centro de planejamento do item
//     /// </summary>
//     [SheetColumn("Centro planej.")]
//     public string? CentroPlanej { get; set; }
//
//     /// <summary>
//     /// Centro de trabalho responsável do item
//     /// </summary>
//     [SheetColumn("CenTrab respon.")]
//     public string? CenTrabRespon { get; set; }
//     
//     /// <summary>
//     /// País do item
//     /// </summary>
//     [SheetColumn("País")]
//     public string? Pais { get; set; }
//
//     /// <summary>
//     /// Classe de defeitos do item
//     /// </summary>
//     [SheetColumn("Classe defeitos")]
//     public string? ClasseDefeitos { get; set; }
//
//     /// <summary>
//     /// Cliente do item
//     /// </summary>
//     [SheetColumn("Cliente")]
//     public string? Cliente { get; set; }
//
//     /// <summary>
//     /// Codificação do item
//     /// </summary>
//     [SheetColumn("Codificação")]
//     public string? Codificacao { get; set; }
//
//     /// <summary>
//     /// Código da parte do objeto do item
//     /// </summary>
//     [SheetColumn("Code parte obj.")]
//     public string? CodeParteObj { get; set; }
//
//     /// <summary>
//     /// Código ABC do item
//     /// </summary>
//     [SheetColumn("Código ABC")]
//     public string? CodigoABC { get; set; }
//
//     /// <summary>
//     /// Código da causa do item
//     /// </summary>
//     [SheetColumn("Cód.causa")]
//     public string? CodCausa { get; set; }
//
//     /// <summary>
//     /// Código do dano do item
//     /// </summary>
//     [SheetColumn("Código do dano")]
//     public string? CodigoDoDano { get; set; }
//
//     /// <summary>
//     /// Código postal do item
//     /// </summary>
//     [SheetColumn("Código postal")]
//     public string? CodigoPostal { get; set; }
//
//     /// <summary>
//     /// Conclusão desejada do item
//     /// </summary>
//     [SheetColumn("Concl.desejada")]
//     public DateTime? ConclDesejada { get; set; }
//
//     /// <summary>
//     /// Condição da instalação antes da avaria do item
//     /// </summary>
//     [SheetColumn("CondInstAntAvar")]
//     public int? CondInstAntAvar { get; set; }
//
//     /// <summary>
//     /// Condição da instalação depois da avaria do item
//     /// </summary>
//     [SheetColumn("CondInstDepAvar")]
//     public int? CondInstDepAvar { get; set; }
//
//     /// <summary>
//     /// Condição da instalação após medição do item
//     /// </summary>
//     [SheetColumn("CondInstApósMed")]
//     public int? CondInstAposMed { get; set; }
//
//     /// <summary>
//     /// Conjunto do item
//     /// </summary>
//     [SheetColumn("Conjunto")]
//     public string? Conjunto { get; set; }
//
//     /// <summary>
//     /// Consequência do item
//     /// </summary>
//     [SheetColumn("Conseqüência")]
//     public int? Consequencia { get; set; }
//
//     /// <summary>
//     /// Controle técnico em do item
//     /// </summary>
//     [SheetColumn("Control.técn_em")]
//     public string? ControlTecnEm { get; set; }
//
//     /// <summary>
//     /// Controle técnico de do item
//     /// </summary>
//     [SheetColumn("Contr.técn.de")]
//     public string? ContrTecnDe { get; set; }
//
//     /// <summary>
//     /// Criado às do item
//     /// </summary>
//     [SheetColumn("Criado às")]
//     public DateTime? CriadoAs { get; set; }
//
//     /// <summary>
//     /// Criado por do item
//     /// </summary>
//     [SheetColumn("Criado por")]
//     public string? CriadoPor { get; set; }
//
//     /// <summary>
//     /// Data da nota do item
//     /// </summary>
//     [SheetColumn("Data da nota")]
//     public DateTime? DataDaNota { get; set; }
//
//     /// <summary>
//     /// Data de criação do item
//     /// </summary>
//     [SheetColumn("Dt.criação")]
//     public DateTime? DtCriacao { get; set; }
//
//     /// <summary>
//     /// Data de referência do item
//     /// </summary>
//     [SheetColumn("Dt.referência")]
//     public DateTime? DtReferencia { get; set; }
//
//     /// <summary>
//     /// Data do pedido do item
//     /// </summary>
//     [SheetColumn("Data pedido")]
//     public DateTime? DataPedido { get; set; }
//
//     /// <summary>
//     /// DDS dispositivo do item
//     /// </summary>
//     [SheetColumn("Dds.dispositivo")]
//     public string? DdsDispositivo { get; set; }
//
//     /// <summary>
//     /// Denominação do conjunto do item
//     /// </summary>
//     [SheetColumn("Denom.conjunto")]
//     public string? DenomConjunto { get; set; }
//
//     /// <summary>
//     /// Denominação do item
//     /// </summary>
//     [SheetColumn("Denominação")]
//     public string? Denominacao1 { get; set; }
//
//     /// <summary>
//     /// Denominação do item
//     /// </summary>
//     [SheetColumn("Denominação")]
//     public string? Denominacao2 { get; set; }
//
//     /// <summary>
//     /// Denominação do item
//     /// </summary>
//     [SheetColumn("Denominação")]
//     public string? Denominacao3 { get; set; }
//
//     /// <summary>
//     /// Descrição do item
//     /// </summary>
//     [SheetColumn("Descrição")]
//     public string? Descricao { get; set; }
//
//     /// <summary>
//     /// Dispositivo antes da avaria do item
//     /// </summary>
//     [SheetColumn("DispAntesAvaria")]
//     public string? DispAntesAvaria { get; set; }
//
//     /// <summary>
//     /// Dispositivo após medição do item
//     /// </summary>
//     [SheetColumn("Disp.após med.")]
//     public string? DispAposMed { get; set; }
//
//     /// <summary>
//     /// Dispositivo depois da avaria do item
//     /// </summary>
//     [SheetColumn("Disp.depoisAvar")]
//     public string? DispDepoisAvar { get; set; }
//
//     /// <summary>
//     /// Divisão do item
//     /// </summary>
//     [SheetColumn("Divisão")]
//     public string? Divisao { get; set; }
//
//     /// <summary>
//     /// Documento de vendas do item
//     /// </summary>
//     [SheetColumn("Doc.vendas")]
//     public string? DocVendas { get; set; }
//
//     /// <summary>
//     /// Duração da parada do item
//     /// </summary>
//     [SheetColumn("Duraç.parada")]
//     public string? DuracParada { get; set; }
//
//     /// <summary>
//     /// Empresa do item
//     /// </summary>
//     [SheetColumn("Empresa")]
//     public string? Empresa { get; set; }
//
//     /// <summary>
//     /// Data de encerramento do item
//     /// </summary>
//     [SheetColumn("Data encermto.")]
//     public DateTime? DataEncermto { get; set; }
//
//     /// <summary>
//     /// Equipamento do item
//     /// </summary>
//     [SheetColumn("Equipamento")]
//     public string? Equipamento { get; set; }
//
//     /// <summary>
//     /// Equipamento afetado do item
//     /// </summary>
//     [SheetColumn("Equipam.afetado")]
//     public string? EquipamAfetado { get; set; }
//
//     /// <summary>
//     /// Fim da avaria do item
//     /// </summary>
//     [SheetColumn("Fim avaria")]
//     public DateTime? FimAvaria { get; set; }
//
//     /// <summary>
//     /// Grupo de códigos do item
//     /// </summary>
//     [SheetColumn("GrpCódigos")]
//     public string? GrpCodigos1 { get; set; }
//
//     /// <summary>
//     /// Grupo de códigos do item
//     /// </summary>
//     [SheetColumn("GrpCódigos")]
//     public string? GrpCodigos2 { get; set; }
//
//     /// <summary>
//     /// Grupo de planejamento PM do item
//     /// </summary>
//     [SheetColumn("Grp.plnj.PM")]
//     public string? GrpPlnjPM { get; set; }
//
//     /// <summary>
//     /// Grupo de códigos do item
//     /// </summary>
//     [SheetColumn("Grupo de codes")]
//     public string? GrupoDeCodes { get; set; }
//
//     /// <summary>
//     /// Hora da nota do item
//     /// </summary>
//     [SheetColumn("Hora da nota")]
//     public TimeSpan? HoraDaNota { get; set; }
//
//     /// <summary>
//     /// Hora de encerramento do item
//     /// </summary>
//     [SheetColumn("Hora encermto.")]
//     public TimeSpan? HoraEncermto { get; set; }
//
//     /// <summary>
//     /// Hora de início desejado do item
//     /// </summary>
//     [SheetColumn("Hora iníc.des.")]
//     public TimeSpan? HoraInicDes { get; set; }
//
//     /// <summary>
//     /// Modificado às do item
//     /// </summary>
//     [SheetColumn("Modificado às")]
//     public DateTime? ModificadoAs { get; set; }
//
//     /// <summary>
//     /// Hora de referência do item
//     /// </summary>
//     [SheetColumn("Hora referência")]
//     public TimeSpan? HoraReferencia { get; set; }
//
//     /// <summary>
//     /// Hora fim da avaria do item
//     /// </summary>
//     [SheetColumn("Hora fim avaria")]
//     public TimeSpan? HoraFimAvaria { get; set; }
//
//     /// <summary>
//     /// Hora final desejada do item
//     /// </summary>
//     [SheetColumn("H final desej.")]
//     public TimeSpan? HFinalDesej { get; set; }
//
//     /// <summary>
//     /// Hora de início da avaria do item
//     /// </summary>
//     [SheetColumn("HoraInícioAvar.")]
//     public TimeSpan? HoraInicioAvar { get; set; }
//
//     /// <summary>
//     /// Imobilizado do item
//     /// </summary>
//     [SheetColumn("Imobilizado")]
//     public string? Imobilizado { get; set; }
//
//     /// <summary>
//     /// Início da avaria do item
//     /// </summary>
//     [SheetColumn("InícioAvar")]
//     public DateTime? InicioAvar { get; set; }
//
//     /// <summary>
//     /// Início desejado do item
//     /// </summary>
//     [SheetColumn("Início desejado")]
//     public DateTime? InicioDesejado { get; set; }
//
//     /// <summary>
//     /// Item do item
//     /// </summary>
//     [SheetColumn("Item")]
//     public string? Item1 { get; set; }
//
//     /// <summary>
//     /// Item do item
//     /// </summary>
//     [SheetColumn("Item")]
//     public string? Item2 { get; set; }
//
//     /// <summary>
//     /// Local do item
//     /// </summary>
//     [SheetColumn("Local")]
//     public string? Local { get; set; }
//
//     /// <summary>
//     /// Local de instalação do item
//     /// </summary>
//     [SheetColumn("Loc.instalação")]
//     public string? LocInstalacao { get; set; }
//
//     /// <summary>
//     /// Local de instalação afetado do item
//     /// </summary>
//     [SheetColumn("Loc.inst.afetad")]
//     public string? LocInstAfetad { get; set; }
//
//     /// <summary>
//     /// Localização do item
//     /// </summary>
//     [SheetColumn("Localização")]
//     public string? Localizacao { get; set; }
//
//     /// <summary>
//     /// Local/Classe contábil do item
//     /// </summary>
//     [SheetColumn("Local/ClasContb")]
//     public string? LocalClasContb { get; set; }
//
//     /// <summary>
//     /// Material do item
//     /// </summary>
//     [SheetColumn("Material")]
//     public string? Material { get; set; }
//
//     /// <summary>
//     /// Modificado em do item
//     /// </summary>
//     [SheetColumn("Modificado em")]
//     public DateTime? ModificadoEm { get; set; }
//
//     /// <summary>
//     /// Modificado por do item
//     /// </summary>
//     [SheetColumn("Modificado por")]
//     public string? ModificadoPor { get; set; }
//
//     /// <summary>
//     /// Moeda do item
//     /// </summary>
//     [SheetColumn("Moeda")]
//     public string? Moeda { get; set; }
//
//     /// <summary>
//     /// Número de ordenação do item
//     /// </summary>
//     [SheetColumn("Nº de ordenação")]
//     public string? NDeOrdenacao1 { get; set; }
//
//     /// <summary>
//     /// Número de ordenação do item
//     /// </summary>
//     [SheetColumn("Nº de ordenação")]
//     public string? NDeOrdenacao2 { get; set; }
//
//     /// <summary>
//     /// Número de série do item
//     /// </summary>
//     [SheetColumn("Nº de série")]
//     public string? NDeSerie { get; set; }
//
//     /// <summary>
//     /// Número do pedido do item
//     /// </summary>
//     [SheetColumn("Nº do pedido")]
//     public string? NDoPedido { get; set; }
//
//     /// <summary>
//     /// Nome da lista do item
//     /// </summary>
//     [SheetColumn("Nome lista")]
//     public string? NomeLista { get; set; }
//
//     /// <summary>
//     /// Nota do item
//     /// </summary>
//     [SheetColumn("Nota")]
//     public long NumeroNotaOrigem { get; set; }
//
//     /// <summary>
//     /// Notificador do item
//     /// </summary>
//     [SheetColumn("Notificador")]
//     public string? Notificador { get; set; }
//
//     /// <summary>
//     /// Ordem do item
//     /// </summary>
//     [SheetColumn("Ordem")]
//     public string? Ordem { get; set; }
//
//     /// <summary>
//     /// Ordem do cliente do item
//     /// </summary>
//     [SheetColumn("Ordem cliente")]
//     public string? OrdemCliente { get; set; }
//
//     /// <summary>
//     /// Organização de vendas do item
//     /// </summary>
//     [SheetColumn("Org.vendas")]
//     public string? OrgVendas { get; set; }
//
//     /// <summary>
//     /// Parada do item
//     /// </summary>
//     [SheetColumn("Parada")]
//     public string? Parada { get; set; }
//
//     /// <summary>
//     /// Grupo de códigos do item
//     /// </summary>
//     [SheetColumn("GrpCódigos")]
//     public string? GrpCodigos3 { get; set; }
//
//     /// <summary>
//     /// Perfil de catálogo do item
//     /// </summary>
//     [SheetColumn("Perf.catálogo")]
//     public string? PerfCatalogo { get; set; }
//
//     /// <summary>
//     /// Prioridade do item
//     /// </summary>
//     [SheetColumn("Prioridade")]
//     public string? Prioridade1 { get; set; }
//
//     /// <summary>
//     /// Região do item
//     /// </summary>
//     [SheetColumn("Região")]
//     public string? Regiao { get; set; }
//
//     /// <summary>
//     /// Rua do item
//     /// </summary>
//     [SheetColumn("Rua")]
//     public string? Rua { get; set; }
//
//     /// <summary>
//     /// Setor de atividade do item
//     /// </summary>
//     [SheetColumn("Setor atividade")]
//     public string? SetorAtividade { get; set; }
//
//     /// <summary>
//     /// Status do sistema do item
//     /// </summary>
//     [SheetColumn("Status sistema")]
//     public string? StatusSistema { get; set; }
//
//     /// <summary>
//     /// Status do usuário do item
//     /// </summary>
//     [SheetColumn("Status usuário")]
//     public string? StatusUsuario { get; set; }
//
//     /// <summary>
//     /// Subnúmero do item
//     /// </summary>
//     [SheetColumn("Subnº")]
//     public string? SubN { get; set; }
//
//     /// <summary>
//     /// Telefone do item
//     /// </summary>
//     [SheetColumn("Telefone")]
//     public string? Telefone { get; set; }
//
//     /// <summary>
//     /// Denominação do item
//     /// </summary>
//     [SheetColumn("Denominação")]
//     public string? Denominacao4 { get; set; }
//
//     /// <summary>
//     /// Texto do código da causa do item
//     /// </summary>
//     [SheetColumn("Txt.code causa")]
//     public string? TxtCodeCausa { get; set; }
//
//     /// <summary>
//     /// Texto do código de codificação do item
//     /// </summary>
//     [SheetColumn("Txt.code codif.")]
//     public string? TxtCodeCodif { get; set; }
//
//     /// <summary>
//     /// Texto do código da parte do objeto do item
//     /// </summary>
//     [SheetColumn("Txt.cod.part.ob")]
//     public string? TxtCodPartOb { get; set; }
//
//     /// <summary>
//     /// Texto da causa do item
//     /// </summary>
//     [SheetColumn("Texto da causa")]
//     public string? TextoDaCausa { get; set; }
//
//     /// <summary>
//     /// Texto do código de problema do item
//     /// </summary>
//     [SheetColumn("Txt.code prob.")]
//     public string? TxtCodeProb { get; set; }
//
//     /// <summary>
//     /// Texto do grupo da causa do item
//     /// </summary>
//     [SheetColumn("Texto grp.causa")]
//     public string? TextoGrpCausa { get; set; }
//
//     /// <summary>
//     /// Texto do grupo do código de problema do item
//     /// </summary>
//     [SheetColumn("Txt.grp.cod.pr.")]
//     public string? TxtGrpCodPr { get; set; }
//
//     /// <summary>
//     /// Texto do grupo do código da parte do objeto do item
//     /// </summary>
//     [SheetColumn("TxtGrpCodPartOb")]
//     public string? TxtGrpCodPartOb { get; set; }
//
//     /// <summary>
//     /// Texto do grupo de codificação do item
//     /// </summary>
//     [SheetColumn("Txt.grp.codific")]
//     public string? TxtGrpCodific { get; set; }
//
//     /// <summary>
//     /// Texto descritivo do item
//     /// </summary>
//     [SheetColumn("Txt.descritivo")]
//     public string? TxtDescritivo { get; set; }
//
//     /// <summary>
//     /// Texto do item
//     /// </summary>
//     [SheetColumn("Texto")]
//     public string? Texto { get; set; }
//
//     /// <summary>
//     /// Prioridade do item
//     /// </summary>
//     [SheetColumn("Prioridade")]
//     public string? Prioridade2 { get; set; }
//
//     /// <summary>
//     /// Tipo de nota do item
//     /// </summary>
//     [SheetColumn("Tipo de nota")]
//     public string? TipoDeNota { get; set; }
//
//     /// <summary>
//     /// Tipo de prioridade do item
//     /// </summary>
//     [SheetColumn("Tipo prioridade")]
//     public string? TipoPrioridade { get; set; }
//
//     /// <summary>
//     /// Unidade do item
//     /// </summary>
//     [SheetColumn("Unidade")]
//     public string? Unidade { get; set; }
//
//     /// <summary>
//     /// Versão do item
//     /// </summary>
//     [SheetColumn("Versão")]
//     public string? Versao1 { get; set; }
//
//     /// <summary>
//     /// Versão do item
//     /// </summary>
//     [SheetColumn("Versão")]
//     public string? Versao2 { get; set; }
//
//     /// <summary>
//     /// Versão do item
//     /// </summary>
//     [SheetColumn("Versão")]
//     public string? Versao3 { get; set; }
//
// }