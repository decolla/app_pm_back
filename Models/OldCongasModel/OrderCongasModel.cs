// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using app_pm_back.Attributes;
// using app_pm_back.Interfaces;
//
// namespace app_pm_back.Models.CongasModel;
//
// /// <summary>
// /// Modelo para OrdemCongasModel
// /// </summary>
// public class OrderCongasModel : IClassifiable
// {
//     /// <summary>
//     /// Número da Ordem (Chave Primária)
//     /// </summary>
//     /// <example>200000081207</example>
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.None)] // O ID vem da planilha, não gera automaticamente
//     [SheetColumn("Ordem")]
//     public long IdOrdem { get; set; }
//     
//     /// <summary>
//     /// O resultado do tipo da nota
//     /// </summary>
//     /// <example>CORRETIVA ou PREVENTIVA</example>
//     public string? Classificacao { get; set; }
//     
//     /// <summary>
//     /// Quem/o que tomou a decisão da classificação da nota
//     /// </summary>
//     /// <example>"MANUAL", "FILTRO_POR_PALAVRAS", "IA_CATBOOST"</example>
//     public string? MetodoClassificacao { get; set;}
//     
//     /// <summary>
//     /// Identificador de quem classificou a nota. No caso nulo, foi classificado pelo sistema
//     /// </summary>
//     public string? ClassificadoPor { get; set; }
//     
//     /// <summary>
//     /// Data de quando a classificacao ocorreu
//     /// </summary>
//     public DateTime? DataClassificacao { get; set; }
//
//     /// <summary>
//     /// Local de instalação do ativo
//     /// </summary>
//     /// <example>CG_05_CON_IND_CRM00073</example>
//     [Required]
//     [SheetColumn("Loc.instalação")]
//     public string LocInstalacao { get; set; }
//     
//     /// <summary>
//     /// Área de controle de custos
//     /// </summary>
//     [SheetColumn("Área Cont.Csts.")]
//     public string?AreaContCsts { get; set; }
//
//     /// <summary>
//     /// Área operacional
//     /// </summary>
//     /// <example>003</example>
//     [SheetColumn("Área operacion.")]
//     public string? AreaOperacion { get; set; }
//
//     /// <summary>
//     /// Bairro
//     /// </summary>
//     /// <example>VILA ENDRES</example>
//     [SheetColumn("Bairro")]
//     public string? Bairro { get; set; }
//
//     /// <summary>
//     /// Campo de ordenação
//     /// </summary>
//     [SheetColumn("Campo ordenação")]
//     public string? CampoOrdenacao { get; set; }
//
//     /// <summary>
//     /// Canal de distribuição
//     /// </summary>
//     [SheetColumn("Canal distrib.")]
//     public string? CanalDistrib { get; set; }
//
//     /// <summary>
//     /// Canal de distribuição - vendas
//     /// </summary>
//     [SheetColumn("Canal distrib. - ven")]
//     public string? CanalDistribVen { get; set; }
//
//     /// <summary>
//     /// Categoria da ordem
//     /// </summary>
//     [SheetColumn("Categoria ordem")]
//     public string? CategoriaOrdem { get; set; }
//
//     /// <summary>
//     /// Centro de custo responsável
//     /// </summary>
//     [SheetColumn("Cen.cst.respons")]
//     public string? CenCstRespons { get; set; }
//
//     /// <summary>
//     /// Centro
//     /// </summary>
//     [SheetColumn("Centro")]
//     public string? Centro { get; set; }
//
//     /// <summary>
//     /// Centro de custo
//     /// </summary>
//     [SheetColumn("Centro custo")]
//     public string? CentroCusto { get; set; }
//
//     /// <summary>
//     /// Centro de lucro
//     /// </summary>
//     [SheetColumn("Centro de lucro")]
//     public string? CentroDeLucro { get; set; }
//
//     /// <summary>
//     /// Centro de trabalho
//     /// </summary>
//     [SheetColumn("Centro trabalho")]
//     public string? CentroTrabalho { get; set; }
//
//     /// <summary>
//     /// Centro de localização
//     /// </summary>
//     [SheetColumn("Cen.localiz.")]
//     public string? CenLocaliz { get; set; }
//
//     /// <summary>
//     /// Centro de planejamento
//     /// </summary>
//     [SheetColumn("Centro planej.")]
//     public string? CentroPlanej { get; set; }
//
//     /// <summary>
//     /// Centro de trabalho responsável
//     /// </summary>
//     /// <example>ENGICRM</example>
//     [SheetColumn("CenTrab respon.")]
//     public string? CenTrabRespon { get; set; }
//
//     /// <summary>
//     /// Chave de sobretaxa
//     /// </summary>
//     [SheetColumn("Chave sobretaxa")]
//     public string? ChaveSobretaxa { get; set; }
//
//     /// <summary>
//     /// País
//     /// </summary>
//     [SheetColumn("País")]
//     public string? Pais { get; set; }
//
//     /// <summary>
//     /// Cliente
//     /// </summary>
//     [SheetColumn("Cliente")]
//     public string? Cliente { get; set; }
//
//     /// <summary>
//     /// Contador do centro de trabalho
//     /// </summary>
//     [SheetColumn("Cntr.cntr.trab.")]
//     public string? CntrCntrTrab { get; set; }
//
//     /// <summary>
//     /// Código ABC
//     /// </summary>
//     [SheetColumn("Código ABC")]
//     public string? CodigoABC { get; set; }
//
//     /// <summary>
//     /// Código postal
//     /// </summary>
//     [SheetColumn("Código postal")]
//     public string? CodigoPostal { get; set; }
//
//     /// <summary>
//     /// Código do plano da ordem
//     /// </summary>
//     [SheetColumn("Cód.plan.ordem")]
//     public string? CodPlanOrdem { get; set; }
//
//     /// <summary>
//     /// Condição de instalação
//     /// </summary>
//     [SheetColumn("CondInstal")]
//     public string? CondInstal { get; set; }
//
//     /// <summary>
//     /// Conjunto
//     /// </summary>
//     [SheetColumn("Conjunto")]
//     public string? Conjunto { get; set; }
//
//     /// <summary>
//     /// Usuário criador da ordem
//     /// </summary>
//     /// <example>IP1020091109</example>
//     [SheetColumn("Criado por")]
//     public string? CriadoPor { get; set; }
//
//     /// <summary>
//     /// Critério para cálculo
//     /// </summary>
//     [SheetColumn("Critério p/cálc")]
//     public string? CriterioParaCalc { get; set; }
//
//     /// <summary>
//     /// Custos estimados
//     /// </summary>
//     [SheetColumn("Custs.estimados")]
//     public decimal? CustsEstimados { get; set; }
//
//     /// <summary>
//     /// Custos totais planejados
//     /// </summary>
//     [SheetColumn("Custos tot.plan")]
//     public decimal? CustosTotPlan { get; set; }
//
//     /// <summary>
//     /// Custos totais reais
//     /// </summary>
//     [SheetColumn("Cust.tot.reais")]
//     public decimal? CustTotReais { get; set; }
//
//     /// <summary>
//     /// Total de liquidação
//     /// </summary>
//     [SheetColumn("Tot.liquidação")]
//     public decimal? TotLiquidacao { get; set; }
//
//     /// <summary>
//     /// Data base de fim
//     /// </summary>
//     [SheetColumn("Data-base fim")]
//     public DateTime? DataBaseFim { get; set; }
//
//     /// <summary>
//     /// Data base de início
//     /// </summary>
//     /// <example>2009-12-15</example>
//     [SheetColumn("Data-base iníc.")]
//     public DateTime? DataBaseInic { get; set; }
//
//     /// <summary>
//     /// Data de entrada
//     /// </summary>
//     [SheetColumn("Data de entrada")]
//     public DateTime? DataDeEntrada { get; set; }
//
//     /// <summary>
//     /// Hora de início
//     /// </summary>
//     [SheetColumn("Hora de início")]
//     public TimeSpan? HoraDeInicio { get; set; }
//
//     /// <summary>
//     /// Data de modificação da ordem
//     /// </summary>
//     [SheetColumn("Data modif.")]
//     public DateTime? DataModif { get; set; }
//
//     /// <summary>
//     /// Data de referência
//     /// </summary>
//     [SheetColumn("Dt.referência")]
//     public DateTime? DtReferencia { get; set; }
//
//     /// <summary>
//     /// Data do pedido
//     /// </summary>
//     [SheetColumn("Data pedido")]
//     public DateTime? DataPedido { get; set; }
//
//     /// <summary>
//     /// Data real de fim da ordem
//     /// </summary>
//     /// <example>2010-01-24</example>
//     [SheetColumn("Dt.real fim")]
//     public DateTime? DtRealFim { get; set; }
//
//     /// <summary>
//     /// Data de liberação real
//     /// </summary>
//     [SheetColumn("Liberação real")]
//     public DateTime? LiberacaoReal { get; set; }
//
//     /// <summary>
//     /// Dados do dispositivo
//     /// </summary>
//     [SheetColumn("Dds.dispositivo")]
//     public string? DdsDispositivo { get; set; }
//
//     /// <summary>
//     /// Denominação do conjunto
//     /// </summary>
//     [SheetColumn("Denom.conjunto")]
//     public string? DenomConjunto { get; set; }
//
//     /// <summary>
//     /// Denominação do local
//     /// </summary>
//     /// <example>HOSPITAL SAO LUIZ</example>
//     [SheetColumn("Denominação")]
//     public string? Denominacao { get; set; }
//
//     /// <summary>
//     /// Produto/serviço
//     /// </summary>
//     [SheetColumn("Prod.serviço")]
//     public string? ProdServico { get; set; }
//
//     /// <summary>
//     /// Disponível desde data
//     /// </summary>
//     [SheetColumn("Disp.desde data")]
//     public DateTime? DispDesdeData { get; set; }
//
//     /// <summary>
//     /// Disponível desde hora
//     /// </summary>
//     [SheetColumn("Disp.desde Hora")]
//     public TimeSpan? DispDesdeHora { get; set; }
//
//     /// <summary>
//     /// Disponível até
//     /// </summary>
//     [SheetColumn("Disponível até")]
//     public DateTime? DisponivelAte { get; set; }
//
//     /// <summary>
//     /// Disponível até hora
//     /// </summary>
//     [SheetColumn("Disp.até hora")]
//     public TimeSpan? DispAteHora { get; set; }
//
//     /// <summary>
//     /// Divisão
//     /// </summary>
//     [SheetColumn("Divisão")]
//     public string? Divisao { get; set; }
//
//     /// <summary>
//     /// Documento de vendas
//     /// </summary>
//     [SheetColumn("Doc.vendas")]
//     public string? DocVendas { get; set; }
//
//     /// <summary>
//     /// Data real de início da ordem
//     /// </summary>
//     /// <example>2010-01-24</example>
//     [SheetColumn("Dta.início real")]
//     public DateTime? DtaInicioReal { get; set; }
//
//     /// <summary>
//     /// Elemento PEP
//     /// </summary>
//     [SheetColumn("Elemento PEP")]
//     public string? ElementoPEP { get; set; }
//
//     /// <summary>
//     /// Elemento de referência PM/PS
//     /// </summary>
//     [SheetColumn("Elem.ref.PM/PS")]
//     public string? ElemRefPMPS { get; set; }
//
//     /// <summary>
//     /// Em garantia
//     /// </summary>
//     [SheetColumn("Em Garantia")]
//     public string? EmGarantia { get; set; }
//
//     /// <summary>
//     /// Empresa
//     /// </summary>
//     [SheetColumn("Empresa")]
//     public string? Empresa { get; set; }
//
//     /// <summary>
//     /// Equipamento
//     /// </summary>
//     [SheetColumn("Equipamento")]
//     public string? Equipamento { get; set; }
//
//     /// <summary>
//     /// Equipe de vendas
//     /// </summary>
//     [SheetColumn("Equipe vendas")]
//     public string? EquipeVendas { get; set; }
//
//     /// <summary>
//     /// Escritório de vendas
//     /// </summary>
//     [SheetColumn("Escrit.vendas")]
//     public string? EscritVendas { get; set; }
//
//     /// <summary>
//     /// Esquema de cálculo
//     /// </summary>
//     [SheetColumn("Esquema cálculo")]
//     public string? EsquemaCalculo { get; set; }
//
//     /// <summary>
//     /// Existe texto descritivo
//     /// </summary>
//     [SheetColumn("Existe TxtDesc.")]
//     public string? ExisteTxtDesc { get; set; }
//
//     /// <summary>
//     /// Fim base (hora)
//     /// </summary>
//     [SheetColumn("Fim-base (hora)")]
//     public TimeSpan? FimBaseHora { get; set; }
//
//     /// <summary>
//     /// Fim programado
//     /// </summary>
//     [SheetColumn("Fim programado")]
//     public DateTime? FimProgramado { get; set; }
//
//     /// <summary>
//     /// Hora fim
//     /// </summary>
//     [SheetColumn("Hora fim")]
//     public TimeSpan? HoraFim { get; set; }
//
//     /// <summary>
//     /// Hora real de fim
//     /// </summary>
//     /// <example>13:20:00</example>
//     [SheetColumn("Fim real hora")]
//     public DateTime? FimRealHora { get; set; }
//
//     /// <summary>
//     /// Forma de faturamento
//     /// </summary>
//     [SheetColumn("Forma fatmto.")]
//     public string? FormaFatmto { get; set; }
//
//     /// <summary>
//     /// Grupo de planejamento manual de lista de tarefas
//     /// </summary>
//     [SheetColumn("GrpPlnManLTa")]
//     public string? GrpPlnManLTa { get; set; }
//
//     /// <summary>
//     /// Grupo de planejamento PM
//     /// </summary>
//     /// <example>101</example>
//     [SheetColumn("Grp.plnj.PM")]
//     public string? GrpPlnjPM { get; set; }
//
//     /// <summary>
//     /// Grupo de lista de tarefas
//     /// </summary>
//     [SheetColumn("GrpLisTar.")]
//     public string? GrpLisTar { get; set; }
//
//     /// <summary>
//     /// Histórico desde
//     /// </summary>
//     [SheetColumn("Histórico desde")]
//     public DateTime? HistoricoDesde { get; set; }
//
//     /// <summary>
//     /// Hora de referência
//     /// </summary>
//     [SheetColumn("Hora referência")]
//     public TimeSpan? HoraReferencia { get; set; }
//
//     /// <summary>
//     /// Hora real de início
//     /// </summary>
//     /// <example>11:00:00</example>
//     [SheetColumn("InícReal hr.")]
//     public DateTime? InicRealHr { get; set; }
//
//     /// <summary>
//     /// Imobilizado
//     /// </summary>
//     [SheetColumn("Imobilizado")]
//     public string? Imobilizado { get; set; }
//
//     /// <summary>
//     /// Início programado
//     /// </summary>
//     [SheetColumn("Início prog.")]
//     public DateTime? InicioProg { get; set; }
//
//     /// <summary>
//     /// Item
//     /// </summary>
//     [SheetColumn("Item")]
//     public string? Item { get; set; }
//
//     /// <summary>
//     /// Item de manutenção
//     /// </summary>
//     /// <example>36467</example>
//     [SheetColumn("Item manut.")]
//     public long? ItemManut { get; set; }
//
//     /// <summary>
//     /// Número da lista de objetos
//     /// </summary>
//     [SheetColumn("Nº lista obj.")]
//     public string? NrListaObj { get; set; }
//
//     /// <summary>
//     /// Local / Cidade
//     /// </summary>
//     /// <example>SAO PAULO</example>
//     [SheetColumn("Local")]
//     public string? Local { get; set; }
//
//     /// <summary>
//     /// Localização
//     /// </summary>
//     [SheetColumn("Localização")]
//     public string? Localizacao { get; set; }
//
//     /// <summary>
//     /// Marca para eliminação
//     /// </summary>
//     [SheetColumn("Marc.p/elimin.")]
//     public string? MarcPElimin { get; set; }
//
//     /// <summary>
//     /// Material
//     /// </summary>
//     [SheetColumn("Material")]
//     public string? Material { get; set; }
//
//     /// <summary>
//     /// Moeda
//     /// </summary>
//     [SheetColumn("Moeda")]
//     public string? Moeda { get; set; }
//
//     /// <summary>
//     /// Número de série
//     /// </summary>
//     [SheetColumn("Nº de série")]
//     public string? NrDeSerie { get; set; }
//
//     /// <summary>
//     /// Número do pedido
//     /// </summary>
//     [SheetColumn("Nº do pedido")]
//     public string? NrDoPedido { get; set; }
//
//     /// <summary>
//     /// Número de endereço
//     /// </summary>
//     [SheetColumn("Nº endereço")]
//     public string? NrEndereco { get; set; }
//
//     /// <summary>
//     /// Nome da lista
//     /// </summary>
//     [SheetColumn("Nome lista")]
//     public string? NomeLista { get; set; }
//
//     /// <summary>
//     /// Número do objeto
//     /// </summary>
//     [SheetColumn("Nº objeto")]
//     public string? NrObjeto { get; set; }
//
//     /// <summary>
//     /// Nota atrelada à ordem (Chave Estrangeira lógica)
//     /// </summary>
//     /// <example>90003663</example>
//     [SheetColumn("Nota")]
//     public long? NumeroNotaOrigem { get; set; }
//
//     /// <summary>
//     /// Número do grupo de roteiro
//     /// </summary>
//     [SheetColumn("Numdr.grp.rot.")]
//     public string? NumdrGrpRot { get; set; }
//
//     /// <summary>
//     /// Operação superior
//     /// </summary>
//     [SheetColumn("Operaç.superior")]
//     public string? OperacSuperior { get; set; }
//
//     /// <summary>
//     /// Ordem
//     /// </summary>
//     [SheetColumn("Ordem")]
//     public string? Ordem { get; set; }
//
//     /// <summary>
//     /// Ordem principal vinculada
//     /// </summary>
//     [SheetColumn("Ordem princip.")]
//     public string? OrdemPrincip { get; set; }
//
//     /// <summary>
//     /// Ordem superior
//     /// </summary>
//     [SheetColumn("Ordem superior")]
//     public string? OrdemSuperior { get; set; }
//
//     /// <summary>
//     /// Organização de vendas
//     /// </summary>
//     [SheetColumn("Org.vendas")]
//     public string? OrgVendas { get; set; }
//
//     /// <summary>
//     /// Organização de vendas - vendas
//     /// </summary>
//     [SheetColumn("Org.vendas - vendas")]
//     public string? OrgVendasVendas { get; set; }
//
//     /// <summary>
//     /// PEP cabeçalho da ordem
//     /// </summary>
//     [SheetColumn("PEP cabeç.ordem")]
//     public string? PEPCabecOrdem { get; set; }
//
//     /// <summary>
//     /// Plano de manutenção atrelado
//     /// </summary>
//     /// <example>10419</example>
//     [SheetColumn("Plano manut.")]
//     public long? PlanoManut { get; set; }
//
//     /// <summary>
//     /// Prioridade
//     /// </summary>
//     [SheetColumn("Prioridade")]
//     public string? Prioridade { get; set; }
//
//     /// <summary>
//     /// Quantidade
//     /// </summary>
//     [SheetColumn("Quantidade")]
//     public decimal? Quantidade { get; set; }
//
//     /// <summary>
//     /// Receitas totais planejadas
//     /// </summary>
//     [SheetColumn("Rec.totais plan")]
//     public decimal? RecTotaisPlan { get; set; }
//
//     /// <summary>
//     /// Receitas totais reais
//     /// </summary>
//     [SheetColumn("Rec.tot.reais")]
//     public decimal? RecTotReais { get; set; }
//
//     /// <summary>
//     /// Rede parcial para
//     /// </summary>
//     [SheetColumn("Rede parcial p/")]
//     public string? RedeParcialP { get; set; }
//
//     /// <summary>
//     /// Região
//     /// </summary>
//     [SheetColumn("Região")]
//     public string? Regiao { get; set; }
//
//     /// <summary>
//     /// Reserva/Requisição de compra
//     /// </summary>
//     [SheetColumn("Reserva/ReqComp")]
//     public string? ReservaReqComp { get; set; }
//
//     /// <summary>
//     /// Revisão
//     /// </summary>
//     [SheetColumn("Revisão")]
//     public string? Revisao { get; set; }
//
//     /// <summary>
//     /// Rua
//     /// </summary>
//     /// <example>AV STO AMARO 000734</example>
//     [SheetColumn("Rua")]
//     public string? Rua { get; set; }
//
//     /// <summary>
//     /// Setor de atividade
//     /// </summary>
//     [SheetColumn("Setor atividade")]
//     public string? SetorAtividade { get; set; }
//
//     /// <summary>
//     /// Setor de atividade - vendas
//     /// </summary>
//     [SheetColumn("Setor atividade - ve")]
//     public string? SetorAtividadeVe { get; set; }
//
//     /// <summary>
//     /// Status de paging
//     /// </summary>
//     [SheetColumn("Status paging")]
//     public string? StatusPaging { get; set; }
//
//     /// <summary>
//     /// Status do sistema da ordem
//     /// </summary>
//     /// <example>ENTE CONF DMNV MOME NOLQ</example>
//     [SheetColumn("Status sistema")]
//     public string? StatusSistema { get; set; }
//
//     /// <summary>
//     /// Status do usuário
//     /// </summary>
//     /// <example>APRO</example>
//     [SheetColumn("Status usuário")]
//     public string? StatusUsuario { get; set; }
//
//     /// <summary>
//     /// Subnúmero
//     /// </summary>
//     [SheetColumn("Subnº")]
//     public string? Subnr { get; set; }
//
//     /// <summary>
//     /// Telefone
//     /// </summary>
//     [SheetColumn("Telefone")]
//     public string? Telefone { get; set; }
//
//     /// <summary>
//     /// Texto breve com a descrição da ordem
//     /// </summary>
//     /// <example>BAUDUCCO (1)</example>
//     [Required]
//     [SheetColumn("Texto breve")]
//     public string TextoBreve { get; set; }
//
//     /// <summary>
//     /// Tipo de atividade PM
//     /// </summary>
//     /// <example>Z02</example>
//     [SheetColumn("Tipo ativid.PM")]
//     public string? TipoAtividPM { get; set; }
//
//     /// <summary>
//     /// Tipo de ordem PM
//     /// </summary>
//     /// <example>ZM03</example>
//     [Required]
//     [SheetColumn("Tipo de ordem")]
//     public string TipoOrdem { get; set; }
//
//     /// <summary>
//     /// Tipo de prioridade
//     /// </summary>
//     [SheetColumn("Tipo prioridade")]
//     public string? TipoPrioridade { get; set; }
//
//     /// <summary>
//     /// Total planejado
//     /// </summary>
//     [SheetColumn("Total planejado")]
//     public decimal? TotalPlanejado { get; set; }
//
//     /// <summary>
//     /// Total real
//     /// </summary>
//     [SheetColumn("Total real")]
//     public decimal? TotalReal { get; set; }
//
//     /// <summary>
//     /// Usuário que modificou a ordem
//     /// </summary>
//     [SheetColumn("Modificado por")]
//     public string? ModificadoPor { get; set; }
//
//     /// <summary>
//     /// Unidade de medida básica
//     /// </summary>
//     [SheetColumn("UM básica")]
//     public string? UMBasica { get; set; }
// }