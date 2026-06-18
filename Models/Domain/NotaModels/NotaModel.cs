using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa uma nota de manutenção ou ocorrência no sistema de gestão de ativos.
/// Esta entidade centraliza informações sobre anomalias, manutenções preventivas e corretivas,
/// permitindo rastreamento completo do ciclo de vida de cada ocorrência.
/// </summary>
/// <example>
/// var nota = new NotaModel
/// {
///     IdNota = 202401001,
///     NumeroNota = 1,
///     DataNota = DateTime.Now,
///     TipoNota = "Corretiva",
///     StatusNota = "Pendente",
///     Prioridade = 1,
///     AnomaliaObservada = "Vazamento de óleo no compressor",
///     IdPlano = 2024,
///     IdAtivo = 150
/// };
/// </example>
public class NotaModel : IClassifiable
{
    #region Identificação e Controle

    /// <summary>
    /// Identificador único da nota, gerado a partir do IdPlano e NumeroNota.
    /// Este valor é definido manualmente e não é auto-incrementado pelo banco de dados.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdNota { get; set; }
    
    /// <summary>
    /// Identificador para string no caso do id do SAP.
    /// </summary>
    public string? SapCode { get; set; }

    /// <summary>
    /// Número sequencial da nota dentro do contexto do plano de manutenção.
    /// Utilizado para ordenação e referência rápida.
    /// </summary>
    /// <example>1, 2, 3...</example>
    [Required(ErrorMessage = "O número de nota é necessário")]
    public long NumeroNota { get; set; }

    /// <summary>
    /// Data e hora de criação da nota no sistema.
    /// Registra quando a ocorrência foi reportada ou documentada.
    /// </summary>
    /// <example>2024-01-15T14:30:00</example>
    public DateTime? DataNota { get; set; }
    
    /// <summary>
    /// Tipo da nota que define a natureza da manutenção ou ocorrência.
    /// Campo obrigatório para categorização primária da nota.
    /// </summary>
    /// <example>"Corretiva", "Preventiva", "Preditiva", "Inspeção"</example>
    [MaxLength(100, ErrorMessage = "O tipo da nota não pode exceder 100 caracteres.")]
    public string? TipoNota { get; set; }

    #endregion

    #region Localização e Contexto

    /// <summary>
    /// Referência adicional sobre a localização física do ativo.
    /// Facilita a identificação do local exato onde a intervenção deve ocorrer.
    /// </summary>
    /// <example>"Setor A - Linha de Produção 2 - Próximo ao compressor principal"</example>
    [MaxLength(200, ErrorMessage = "A referência de localização não pode exceder 200 caracteres.")]
    public string? ReferenciaLocalizacaoAtivo { get; set; }

    /// <summary>
    /// Finalidade ou objetivo da nota.
    /// Descreve de forma sucinta o propósito da intervenção ou registro.
    /// </summary>
    /// <example>"Corrigir vazamento de óleo", "Inspeção preventiva trimestral"</example>
    [MaxLength(120, ErrorMessage = "A finalidade não pode exceder 120 caracteres.")]
    public string? Finalidade { get; set; }

    #endregion

    #region Prioridade e Prazos

    /// <summary>
    /// Nível de prioridade da nota para gestão de recursos e agendamento.
    /// Valores menores indicam maior prioridade (ex: 1 = Crítica, 2 = Alta, 3 = Média, 4 = Baixa).
    /// </summary>
    /// <example>1</example>
    public Prioridade? Prioridade { get; set; }

    /// <summary>
    /// Data e hora desejada para conclusão da intervenção.
    /// Define o prazo esperado para resolução da ocorrência.
    /// </summary>
    /// <example>2024-01-20T18:00:00</example>
    public DateTime? FimDesejado { get; set; }

    #endregion

    #region Informações da Avaria

    /// <summary>
    /// Data em que a avaria ou problema foi identificado.
    /// Importante para cálculos de tempo de resposta e análise de tendências.
    /// </summary>
    /// <example>2024-01-15</example>
    public DateTime? DataAvaria { get; set; }

    /// <summary>
    /// Hora específica em que a avaria ocorreu.
    /// Complementa a DataAvaria com informação de horário preciso.
    /// </summary>
    /// <example>14:30:00</example>
    public TimeSpan? HoraAvaria { get; set; }

    #endregion

    #region Status e Disponibilidade

    /// <summary>
    /// Status atual da nota no fluxo de trabalho.
    /// Permite rastreamento do ciclo de vida da ocorrência.
    /// </summary>
    /// <example>"Pendente", "Em Andamento", "Aguardando Peças", "Concluída", "Cancelada"</example>
    public Status? StatusNota { get; set; }

    /// <summary>
    /// Indicador de disponibilidade atual do equipamento afetado.
    /// Auxilia no planejamento de recursos e impacto operacional.
    /// </summary>
    /// <example>"Disponível", "Indisponível", "Operação Limitada", "Em Manutenção"</example>
    [MaxLength(50, ErrorMessage = "O status de disponibilidade não pode exceder 50 caracteres.")]
    public string? EquipamentoDisponivel { get; set; }

    /// <summary>
    /// Indica se a confiabilidade do equipamento foi restaurada após a intervenção.
    /// Importante para métricas de eficácia de manutenção.
    /// </summary>
    /// <example>RestauraAtivo</example>
    public ConfiabilidadeRestaurada? ConfiabilidadeRestaurada { get; set; }

    #endregion

    #region Descrição e Observações
    
    /// <summary>
    /// Descrição breve ou título resumido da nota.
    /// Fornece uma visão rápida do conteúdo da nota para listagens e resumos.
    /// </summary>
    [MaxLength(100)]
    public string? DescricaoBreve { get; set; }

    /// <summary>
    /// Descrição detalhada da anomalia, problema ou situação observada.
    /// Campo essencial para documentação técnica da ocorrência.
    /// </summary>
    /// <example>"Identificado vazamento de óleo na junta do compressor. Volume estimado: 200ml. Equipamento apresenta ruído anormal."</example>
    [MaxLength(1000, ErrorMessage = "A descrição da anomalia não pode exceder 1000 caracteres.")]
    public string? AnomaliaObservada { get; set; }

    /// <summary>
    /// Observações complementares, comentários adicionais ou informações contextuais.
    /// Campo livre para documentação de detalhes relevantes não cobertos por outros campos.
    /// </summary>
    /// <example>"Verificar histórico de manutenções anteriores. Possível relação com troca de óleo realizada na semana passada."</example>
    [MaxLength(2000, ErrorMessage = "As observações complementares não podem exceder 2000 caracteres.")]
    public string? ObservacoesComplementares { get; set; }

    #endregion

    #region Classificação

    /// <summary>
    /// Categoria ou tipo atribuído à nota após análise.
    /// Permite agrupamento e filtragem de notas por similaridade.
    /// </summary>
    public Classificacao Classificacao { get; set; } = Classificacao.NAOCLASSIFICADA;

    /// <summary>
    /// Método ou técnica utilizada para classificar a nota.
    /// Pode indicar classificação manual, automática ou híbrida.
    /// </summary>
    /// <example>"Manual", "IA - Modelo v2.1", "Regras de Negócio", "Híbrido"</example>
    [MaxLength(100, ErrorMessage = "O método de classificação não pode exceder 100 caracteres.")]
    public MetodoClassificacao MetodoClassificacao { get; set; } = MetodoClassificacao.NAOCLASSIFICADA;

    /// <summary>
    /// Identificação do responsável pela classificação.
    /// Pode ser nome de usuário, algoritmo ou sistema que realizou a classificação.
    /// </summary>
    /// <example>"João Silva", "Sistema IA", "Supervisor Manutenção"</example>
    [MaxLength(150, ErrorMessage = "O identificador do classificador não pode exceder 150 caracteres.")]
    public string? ClassificadoPor { get; set; }

    /// <summary>
    /// Data e hora em que a classificação foi realizada.
    /// Importante para auditoria e rastreamento de mudanças.
    /// </summary>
    /// <example>2024-01-15T15:00:00</example>
    /// <example>2024-01-15T15:00:00</example>
    public DateTime? DataClassificacao { get; set; }

    #endregion

    #region Auditoria

    /// <summary>
    /// Identificação do usuário que criou a nota no sistema.
    /// Utilizado para rastreamento e responsabilização.
    /// </summary>
    /// <example>"maria.santos@empresa.com", "João Silva"</example>
    [MaxLength(150, ErrorMessage = "O identificador do criador não pode exceder 150 caracteres.")]
    public string? CriadaPorUsuario { get; set; }

    /// <summary>
    /// Identificação do usuário que realizou a última modificação na nota.
    /// Mantém histórico de alterações para auditoria.
    /// </summary>
    /// <example>"pedro.oliveira@empresa.com", "Ana Costa"</example>
    [MaxLength(150, ErrorMessage = "O identificador do modificador não pode exceder 150 caracteres.")]
    public string? ModificadaPorUsuario { get; set; }

    /// <summary>
    /// Data e hora da última modificação realizada na nota.
    /// Complementa a informação de quem modificou para controle temporal.
    /// </summary>
    /// <example>2024-01-16T10:30:00</example>
    public DateTime? DataModificacao { get; set; }

    #endregion

    #region Chaves Estrangeiras
    /// <summary>
    /// O escopo da nota/pordem
    /// </summary>
    [MaxLength(100, ErrorMessage = "O escopo não pode exceder 100 caracteres.")]
    public string? Escopo { get; set; } = "N/A";

    /// <summary>
    /// Chave estrangeira para o plano de manutenção associado.
    /// Vincula a nota a um contexto de planejamento específico.
    /// </summary>
    /// <example>2024</example>
    public long? IdPlano { get; set; }

    /// <summary>
    /// Chave estrangeira para o usuário criador da nota.
    /// Estabelece relacionamento com a entidade de usuário responsável.
    /// </summary>
    /// <example>42</example>
    public long? IdUsuario { get; set; }

    /// <summary>
    /// Chave estrangeira para o ativo associado à nota.
    /// Identifica qual equipamento ou item está relacionado à ocorrência.
    /// </summary>
    /// <example>150</example>
    public ComponentType? IdAtivo { get; set; }
    
    /// <summary>
    /// Chave estrangeira para a localização de instalação do ativo.
    /// Permite vincular a nota a um local específico de instalação.
    /// </summary>
    [MaxLength(100)]
    public string? IdLi { get; set; }
    
    /// <summary>
    /// Nomeclatura alternativa para o ativo, também usada para verificar se existe no APM
    /// </summary>
    [MaxLength(100)]
    public string? DnsIot { get; set; }
    
    /// <summary>
    /// Nome da estratégia relacionada ao Plano
    /// </summary>
    [NotMapped]
    [MaxLength(200)]
    public string NomeEstrategia => Plano?.Estrategia?.Nome;

    #endregion

    #region Navegação

    /// <summary>
    /// Propriedade de navegação para o plano de manutenção pai.
    /// Permite acesso ao contexto completo do plano ao qual a nota pertence.
    /// </summary>
    [ForeignKey(nameof(IdPlano))]
    public virtual PlanoModel? Plano { get; set; }

    /// <summary>
    /// Propriedade de navegação para o usuário criador.
    /// Fornece acesso aos dados completos do usuário responsável pela criação.
    /// </summary>
    [ForeignKey(nameof(IdUsuario))]
    public virtual UsuarioModel? Usuario { get; set; }

    #endregion

    #region Coleções

    /// <summary>
    /// Coleção de variáveis customizadas associadas à nota.
    /// Permite extensibilidade através de campos dinâmicos específicos por tipo de nota.
    /// </summary>
    /// <example>
    /// Variaveis.Add(new VariavelNotaModel { Nome = "Temperatura", Valor = "85°C" });
    /// </example>
    public virtual ICollection<VariavelNotaModel> Variaveis { get; set; } = new List<VariavelNotaModel>();

    /// <summary>
    /// Coleção de itens de material ou serviço associados à nota.
    /// Registra recursos necessários ou utilizados na resolução da ocorrência.
    /// </summary>
    /// <example>
    /// Itens.Add(new ItemNotaModel { DescricaoBreve = "Junta de vedação", Quantidade = 2 });
    /// </example>
    public virtual ICollection<ItemNotaModel> Itens { get; set; } = new List<ItemNotaModel>();

    /// <summary>
    /// Coleção de anexos (arquivos, fotos, documentos) vinculados à nota.
    /// Permite documentação visual e complementar da ocorrência.
    /// </summary>
    /// <example>
    /// Anexos.Add(new AnexoNotaModel { Nome = "foto_vazamento.jpg", TipoArquivo = "image/jpeg" });
    /// </example>
    public virtual ICollection<AnexoNotaModel> Anexos { get; set; } = new List<AnexoNotaModel>();

    /// <summary>
    /// Coleção de ordens de serviço geradas a partir desta nota.
    /// Permite rastreamento da execução e desdobramentos da ocorrência.
    /// </summary>
    /// <example>
    /// Ordens.Add(new OrdemModel { NumeroOrdem = "OS-2024-001", Status = "Aberta" });
    /// </example>
    public virtual ICollection<OrdemModel> Ordens { get; set; } = new List<OrdemModel>();

    #endregion
}