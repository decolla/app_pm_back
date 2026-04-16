using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Attributes;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.CongasModel;

/// <summary>
/// Modelo para NotaCongasModel
/// </summary>
public class NoteCongasModel : IClassifiable
{
    /// <summary>
    /// Id da Nota
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdNota { get; set; }
    
    /// <summary>
    /// Número da nota vindo do SAP/Planilha
    /// </summary>
    /// <example>4088673</example>
    [SheetColumn("Nota")]
    public long NumeroNota { get; set; }
    
    /// <summary>
    /// Tipo de nota gerada
    /// </summary>
    /// <example>MC</example>
    [SheetColumn("Tipo de nota")]
    // [Range(1, 2, ErrorMessage = "O tipo de nota deve estar entre 1 e 2")]
    [StringLength(4, ErrorMessage = "O tipo de nota deve ter ate 4 caracteres.")]
    public string? TipoNota { get; set; }
    
    /// <summary>
    /// Area operacional da nota 
    /// </summary>
    [SheetColumn("Área operacion.")]
    public int? AreaOperac { get; set; }
    
    
    /// <summary>
    /// O resultado do tipo da nota
    /// </summary>
    /// <example>CORRETIVA ou PREVENTIVA</example>
    public string? Classificacao { get; set; }
    
    /// <summary>
    /// Quem/o que tomou a decisão da classificação da nota
    /// </summary>
    /// <example>"MANUAL", "FILTRO_POR_PALAVRAS", "IA_CATBOOST"</example>
    public string? MetodoClassificacao { get; set;}
    
    /// <summary>
    /// Identificador de quem classificou a nota. No caso nulo, foi classificado pelo sistema
    /// </summary>
    public string? ClassificadoPor { get; set; }
    
    /// <summary>
    /// Data de quando a classificacao ocorreu
    /// </summary>
    public DateTime? DataClassificacao { get; set; }
    
    /// <summary>
    /// Local de instalação do campo de seleção
    /// </summary>
    /// <example>CG_05_CON_IND_CRM00087</example>
    [SheetColumn("Loc.instalação")]
    public string? LocInstalacao { get; set; }
    
    /// <summary>
    /// Status do estado do ativo relatado na nota
    /// </summary>
    /// <example> MSEN </example>
    [SheetColumn("Status sistema")]
    // [Range(1,4, ErrorMessage = "O status deve estar entre 1 e 4")]
    [StringLength(10, ErrorMessage = "O status do sistema deve ter ate 10 caracteres.")]
    public string? StatusSistema { get; set; }
    
    /// <summary>
    /// Descrição do estado do ativo relatado na nota
    /// </summary>
    /// <example> APRV </example>
    [SheetColumn("Status usuário")]
    // [Range(1,4, ErrorMessage = "O status deve estar entre 1 e 4")]
    [StringLength(10, ErrorMessage = "O status do usuario deve ter ate 10 caracteres.")]
    public string? StatusUsuario { get; set; }
    
    /// <summary>
    /// Descrição do estado do ativo relatado na nota
    /// </summary>
    /// <example>UAR 2014 - SVVA TROCA DA VALVULA ALIVIO</example>
    [SheetColumn("Descrição")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Rua da localização
    /// </summary>
    /// <example>R PASCAL 000000</example>
    [SheetColumn("Rua")]
    public string? Rua { get; set; }
    
    /// <summary>
    /// Bairro da localização
    /// </summary>
    /// <example>CAMPO BELO</example>
    [SheetColumn("Bairro")]
    public string? Bairro { get; set; }
    
    /// <summary>
    /// Local da instalação
    /// </summary>
    /// <example>SAO PAULO</example>
    [SheetColumn("Local")]
    public string? Local { get; set; }
    
    /// <summary>
    /// Data de emissão da nota 
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("Data da nota")]
    public DateTime? DataNota { get; set; }
    
    /// <summary>
    /// Data de modificação da nota 
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("Modificado em")]
    public DateTime? ModificadoEm { get; set; }
    
    /// <summary>
    /// Identificação de quem modificou a nota
    /// </summary>
    /// <example>CS123456</example>
    [SheetColumn("Modificado por")]
    public string? ModificadoPor { get; set; }
    
    /// <summary>
    /// Horário de modificação da nota
    /// </summary>
    /// <example>hh:mm:ss</example>
    [SheetColumn("Modificado às")]
    public DateTime? ModificadoAs { get; set; }
    
    /// <summary>
    /// Tipo de prioridade da nota (PM ou EM)
    /// </summary>
    /// <example>PM</example>
    [SheetColumn("Tipo prioridade")]
    public string? TipoPrioridade { get; set; }
    
    /// <summary>
    /// Data de início da avaria
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("InícioAvar")]
    public DateTime? InicioAvar { get; set; }
    
    /// <summary>
    /// Data de fim da avaria
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("Fim avaria")]
    public DateTime? FimAvaria { get; set; }
    
    /// <summary>
    /// Hora de início da avaria
    /// </summary>
    /// <example>hh:mm:ss</example>
    [SheetColumn("HoraInícioAvar.")]
    public DateTime? HoraInicioAvaria { get; set; }

    /// <summary>
    /// Hora do fim da avaria
    /// </summary>
    /// <example>hh:mm:ss</example>
    [SheetColumn("Hora fim avaria")]
    public DateTime? HoraFimAvaria { get; set; }

    /// <summary>
    /// Data de referência
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("Dt.referência")]
    public DateTime? DtReferencia { get; set; }

    /// <summary>
    /// Hora de referência
    /// </summary>
    /// <example>hh:mm:ss</example>
    [SheetColumn("Hora referência")]
    public DateTime? HoraReferencia { get; set; }

    /// <summary>
    /// Indicador de se terá parada
    /// </summary>
    /// <example>X</example>
    [SheetColumn("Parada")]
    public string? Parada { get; set; }

    /// <summary>
    /// Duração da parada
    /// </summary>
    [SheetColumn("Duraç.parada")]
    public decimal? DuracParada { get; set; }

    /// <summary>
    /// Data de criação da nota
    /// </summary>
    /// <example>DD/MM/AAAA</example>
    [SheetColumn("Dt.criação")]
    public DateTime? DtCriacao { get; set; }

    /// <summary>
    /// Usuário que criou a nota
    /// </summary>
    /// <example>CS123456</example>
    [SheetColumn("Criado por")]
    public string? CriadoPor { get; set; }

    /// <summary>
    /// Hora em que a nota foi criada
    /// </summary>
    /// <example>hh:mm:ss</example>
    [SheetColumn("Criado às")]
    public DateTime? CriadoAs { get; set; }

    /// <summary>
    /// Código do notificador
    /// </summary>
    /// <example>OOOM1</example>
    [SheetColumn("Notificador")]
    public string? Notificador { get; set; }

    /// <summary>
    /// Perfil do catálogo
    /// </summary>
    /// <example>PM_COR_02</example>
    [SheetColumn("Perf.catálogo")]
    public string? PerfCatalogo { get; set; }

    /// <summary>
    /// Centro de trabalho responsável
    /// </summary>
    /// <example>CGP0101</example>
    [SheetColumn("CenTrab respon.")]
    public string? CenTrabRespon { get; set; }

    /// <summary>
    /// Texto do code de codificação
    /// </summary>
    /// <example>ERD</example>
    [SheetColumn("Txt.code codif.")]
    public string? TxtCodeCodif { get; set; }

    /// <summary>
    /// Texto do grupo de codificação
    /// </summary>
    /// <example>NOTA DE MANUTENÇÃO</example>
    [SheetColumn("Txt.grp.codific")]
    public string? TxtGrpCodific { get; set; }

    /// <summary>
    /// Local de instalação afetado
    /// </summary>
    /// <example>CG_08_EST_ECP_ECP00066</example>
    [SheetColumn("Loc.inst.afetad")]
    public string? LocInstAfetado { get; set; }

    /// <summary>
    /// Equipamento envolvido na nota
    /// </summary>
    /// <example>123456789</example>
    [SheetColumn("Equipamento")]
    public string? Equipamento { get; set; }

    /// <summary>
    /// Denominação do ativo
    /// </summary>
    /// <example>ERP123</example>
    [SheetColumn("Denominação")]
    public string? Denominacao { get; set; }

    /// <summary>
    /// Grupo de planejamento PM
    /// </summary>
    /// <example>100</example>
    [SheetColumn("Grp.plnj.PM")]
    public string? GrpPlnjPM { get; set; }

    /// <summary>
    /// Área operacional
    /// </summary>
    /// <example>019</example>
    [SheetColumn("Área operacion.")]
    public string? AreaOperacion { get; set; }

    /// <summary>
    /// Codificação do problema
    /// </summary>
    /// <example>N1</example>
    [SheetColumn("Codificação")]
    public string? Codificacao { get; set; }

    /// <summary>
    /// Nível de consequência do problema, de 1 a 4
    /// </summary>
    /// <example>2</example>
    [SheetColumn("Conseqüência")]
    [Range(1,4,ErrorMessage = "O limite do nível é de 1 a 4")]
    public int? Consequencia { get; set; }

    /// <summary>
    /// Grupo de codes
    /// </summary>
    /// <example>01-NOTA</example>
    [SheetColumn("Grupo de codes")]
    public string? GrupoCodes { get; set; }

    /// <summary>
    /// ID da ordem gerada por esta nota 
    /// </summary>
    /// <example>200000083031</example>
    [SheetColumn("Ordem")]
    public long? IdOrdem { get; set; }

    /// <summary>
    /// Área de controle de custos
    /// </summary>
    [SheetColumn("Área Cont.Csts.")]
    public string? AreaContCsts { get; set; }

    /// <summary>
    /// Campo de ordenação
    /// </summary>
    [SheetColumn("Campo ordenação")]
    public string? CampoOrdenacao { get; set; }

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
    [SheetColumn("Centro p/CenTrb")]
    public string? CentroParaCenTrb { get; set; }

    /// <summary>
    /// Centro para centro de trabalho
    /// </summary>
    [SheetColumn("Cen.p/cen.trab.")]
    public string? CenParaCenTrab { get; set; }

    /// <summary>
    /// Centro de planejamento
    /// </summary>
    [SheetColumn("Centro planej.")]
    public string? CentroPlanej { get; set; }

    /// <summary>
    /// País
    /// </summary>
    [SheetColumn("País")]
    public string? Pais { get; set; }

    /// <summary>
    /// Cliente
    /// </summary>
    [SheetColumn("Cliente")]
    public string? Cliente { get; set; }

    /// <summary>
    /// Código ABC
    /// </summary>
    [SheetColumn("Código ABC")]
    public string? CodigoABC { get; set; }

    /// <summary>
    /// Código postal
    /// </summary>
    [SheetColumn("Código postal")]
    public string? CodigoPostal { get; set; }

    /// <summary>
    /// Data de conclusão desejada
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
    /// Condição de instalação após medição
    /// </summary>
    [SheetColumn("CondInstApósMed")]
    public string? CondInstAposMed { get; set; }

    /// <summary>
    /// Conjunto
    /// </summary>
    [SheetColumn("Conjunto")]
    public string? Conjunto { get; set; }

    /// <summary>
    /// Controle técnico em
    /// </summary>
    [SheetColumn("Control.técn_em")]
    public string? ControlTecnEm { get; set; }

    /// <summary>
    /// Controle técnico de
    /// </summary>
    [SheetColumn("Contr.técn.de")]
    public string? ContrTecnDe { get; set; }

    /// <summary>
    /// Dados do dispositivo
    /// </summary>
    [SheetColumn("Dds.dispositivo")]
    public string? DdsDispositivo { get; set; }

    /// <summary>
    /// Denominação do conjunto
    /// </summary>
    [SheetColumn("Denom.conjunto")]
    public string? DenomConjunto { get; set; }

    /// <summary>
    /// Denominação do executor
    /// </summary>
    [SheetColumn("Denom.executor")]
    public string? DenomExecutor { get; set; }

    /// <summary>
    /// Dispositivo antes da avaria
    /// </summary>
    [SheetColumn("DispAntesAvaria")]
    public string? DispAntesAvaria { get; set; }

    /// <summary>
    /// Dispositivo após medição
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
    /// Indica se está em garantia
    /// </summary>
    [SheetColumn("Em Garantia")]
    public string? EmGarantia { get; set; }

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
    /// Equipamento afetado
    /// </summary>
    [SheetColumn("Equipam.afetado")]
    public string? EquipamAfetado { get; set; }

    /// <summary>
    /// Equipe de vendas
    /// </summary>
    [SheetColumn("Equipe vendas")]
    public string? EquipeVendas { get; set; }

    /// <summary>
    /// Escritório de vendas
    /// </summary>
    [SheetColumn("Escrit.vendas")]
    public string? EscritVendas { get; set; }

    /// <summary>
    /// Executado por
    /// </summary>
    [SheetColumn("Executado por")]
    public string? ExecutadoPor { get; set; }

    /// <summary>
    /// Hora da nota
    /// </summary>
    [SheetColumn("Hora da nota")]
    public DateTime? HoraDaNota { get; set; }

    /// <summary>
    /// Hora de encerramento
    /// </summary>
    [SheetColumn("Hora encermto.")]
    public DateTime? HoraEncermto { get; set; }

    /// <summary>
    /// Hora de início desejado
    /// </summary>
    [SheetColumn("Hora iníc.des.")]
    public DateTime? HoraInicDes { get; set; }

    /// <summary>
    /// Hora final desejada
    /// </summary>
    [SheetColumn("H final desej.")]
    public DateTime? HFinalDesej { get; set; }

    /// <summary>
    /// Início desejado
    /// </summary>
    [SheetColumn("Início desejado")]
    public DateTime? InicioDesejado { get; set; }

    /// <summary>
    /// ID de ramal
    /// </summary>
    [SheetColumn("ID de Ramal")]
    public string? IDRamal { get; set; }

    /// <summary>
    /// Imobilizado
    /// </summary>
    [SheetColumn("Imobilizado")]
    public string? Imobilizado { get; set; }

    /// <summary>
    /// Instalação
    /// </summary>
    [SheetColumn("Instalação")]
    public string? Instalacao { get; set; }

    /// <summary>
    /// Item
    /// </summary>
    [SheetColumn("Item")]
    public string? Item { get; set; }

    /// <summary>
    /// Item de manutenção
    /// </summary>
    [SheetColumn("Item manut.")]
    public long? ItemManut { get; set; }

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
    /// Moeda
    /// </summary>
    [SheetColumn("Moeda")]
    public string? Moeda { get; set; }

    /// <summary>
    /// Número de série
    /// </summary>
    [SheetColumn("Nº de série")]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Número do certificado
    /// </summary>
    [SheetColumn("No. do Certificado")]
    public string? NumeroCertificado { get; set; }

    /// <summary>
    /// Número do pedido
    /// </summary>
    [SheetColumn("Nº do pedido")]
    public string? NumeroPedido { get; set; }

    /// <summary>
    /// Data do pedido
    /// </summary>
    [SheetColumn("Data pedido")]
    public DateTime? DataPedido { get; set; }

    /// <summary>
    /// Número de endereço
    /// </summary>
    [SheetColumn("Nº endereço")]
    public string? NumeroEndereco { get; set; }

    /// <summary>
    /// Nome da lista
    /// </summary>
    [SheetColumn("Nome lista")]
    public string? NomeLista { get; set; }

    /// <summary>
    /// Número do objeto
    /// </summary>
    [SheetColumn("Nº objeto")]
    public string? NumeroObjeto { get; set; }

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
    /// PEP
    /// </summary>
    [SheetColumn("PEP")]
    public string? PEP { get; set; }

    /// <summary>
    /// Plano de manutenção
    /// </summary>
    [SheetColumn("Plano manut.")]
    public long? PlanoManut { get; set; }

    /// <summary>
    /// Prioridade
    /// </summary>
    [SheetColumn("Prioridade")]
    public string? Prioridade { get; set; }

    /// <summary>
    /// Procedência
    /// </summary>
    [SheetColumn("Procedência")]
    public string? Procedencia { get; set; }

    /// <summary>
    /// Referência externa
    /// </summary>
    [SheetColumn("Referência externa")]
    public string? ReferenciaExterna { get; set; }

    /// <summary>
    /// Região
    /// </summary>
    [SheetColumn("Região")]
    public string? Regiao { get; set; }

    /// <summary>
    /// Revisão
    /// </summary>
    [SheetColumn("Revisão")]
    public string? Revisao { get; set; }

    /// <summary>
    /// Segmento
    /// </summary>
    [SheetColumn("Segmento")]
    public string? Segmento { get; set; }

    /// <summary>
    /// Setor de atividade
    /// </summary>
    [SheetColumn("Setor atividade")]
    public string? SetorAtividade { get; set; }

    /// <summary>
    /// Status paging
    /// </summary>
    [SheetColumn("Status paging")]
    public string? StatusPaging { get; set; }

    /// <summary>
    /// Subnúmero
    /// </summary>
    [SheetColumn("Subnº")]
    public string? Subnumero { get; set; }

    /// <summary>
    /// Telefone
    /// </summary>
    [SheetColumn("Telefone")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Texto descritivo
    /// </summary>
    [SheetColumn("Txt.descritivo")]
    public string? TxtDescritivo { get; set; }

    /// <summary>
    /// Unidade
    /// </summary>
    [SheetColumn("Unidade")]
    public string? Unidade { get; set; }

    /// <summary>
    /// Zona de atendimento
    /// </summary>
    [SheetColumn("Zona de Atendimento")]
    public string? ZonaAtendimento { get; set; }

    /// <summary>
    /// Propiedade de navegação
    /// </summary>
    public virtual ICollection<ItemNotaCongasModel> Itens { get; set; } = new List<ItemNotaCongasModel>();
}