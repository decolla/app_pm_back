using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using app_pm_back.Interfaces;

namespace app_pm_back.Models.Domain;

/// <summary>
/// Representa uma ordem de trabalho no sistema de manutenção.
/// Este modelo gerencia todo o ciclo de vida de uma ordem, desde sua criação até conclusão,
/// incluindo planejamento, execução, classificação e rastreamento de auditoria.
/// </summary>
/// <remarks>
/// A ordem de trabalho é a entidade central para gerenciamento de atividades de manutenção,
/// permitindo controle de prazos, recursos, criticidade e relacionamento com ativos e notas.
/// </remarks>
public class OrdemModel : IClassifiable, IAuditable
{
    #region Identificação e Controle

    /// <summary>
    /// Identificador único da ordem de trabalho.
    /// O ID da ordem é gerado externamente e deve ser fornecido ao criar uma nova ordem.
    /// </summary>
    /// <example>100001</example>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long IdOrdem { get; set; }
    
    /// <summary>
    /// Número sequencial da ordem de trabalho para identificação visual.
    /// Utilizado para facilitar a referência e comunicação sobre ordens específicas.
    /// </summary>
    /// <example>2024001</example>
    public long? NumeroOrdem { get; set; }

    /// <summary>
    /// Data e hora em que a ordem foi criada no sistema.
    /// Utilizado para rastreamento temporal e relatórios de abertura.
    /// </summary>
    /// <example>2024-01-15T08:30:00</example>
    [Required(ErrorMessage = "A data de criação é obrigatória.")]
    public DateTime? DataCriacao { get; set; }

    #endregion

    #region StatusFichas e Classificação

    /// <summary>
    /// StatusFichas atual da ordem de trabalho no fluxo de execução.
    /// Indica o estado operacional da ordem para controle de workflow.
    /// </summary>
    /// <example>Aberta, Em Andamento, Aguardando Peças, Concluída, Cancelada</example>
    public StatusFichas? StatusOrdem { get; set; }

    /// <summary>
    /// Nível de criticidade da ordem para priorização de execução.
    /// Define a urgência e impacto da ordem no contexto operacional.
    /// </summary>
    /// <example>Baixa, Média, Alta, Crítica, Emergencial</example>
    [MaxLength(100, ErrorMessage = "A criticidade não pode exceder 100 caracteres.")]
    public string? Criticidade { get; set; }

    /// <summary>
    /// Classificação da ordem conforme a interface IClassifiable.
    /// Permite categorização customizada da ordem segundo regras de negócio.
    /// </summary>
    /// <example>Preventiva, Corretiva/example>
    public Classificacao Classificacao { get; set; } = Classificacao.NAOCLASSIFICADA;

    /// <summary>
    /// Método ou critério utilizado para classificar a ordem.
    /// Documenta o algoritmo, regra ou processo de decisão utilizado na classificação.
    /// </summary>
    /// <example>Manual, Automático por IA, Baseado em Histórico, Por Template</example>
    [MaxLength(100, ErrorMessage = "O método de classificação não pode exceder 100 caracteres.")]
    public MetodoClassificacao MetodoClassificacao { get; set; } = MetodoClassificacao.NAOCLASSIFICADA;

    /// <summary>
    /// Identificação do usuário ou sistema que classificou a ordem.
    /// Rastreia a origem da classificação para auditoria e responsabilização.
    /// </summary>
    /// <example>usuario@empresa.com, Sistema-AutoClassificador, Admin</example>
    [MaxLength(150, ErrorMessage = "O identificador de quem classificou não pode exceder 150 caracteres.")]
    public string? ClassificadoPor { get; set; }

    /// <summary>
    /// Data e hora em que a ordem foi classificada.
    /// Permite rastreamento temporal do processo de classificação.
    /// </summary>
    /// <example>2024-01-15T09:00:00</example>
    public DateTime? DataClassificacao { get; set; }

    /// <summary>
    /// Tipo da ordem de trabalho conforme taxonomia do sistema.
    /// Define a natureza da intervenção a ser realizada.
    /// </summary>
    /// <example>Manutenção Preventiva, Inspeção, Calibração, Retrofit</example>
    [MaxLength(100, ErrorMessage = "O tipo da ordem não pode exceder 100 caracteres.")]
    public string? TipoOrdem { get; set; }

    /// <summary>
    /// Indica se a confiabilidade operacional do ativo foi restaurada após a execução da ordem.
    /// Utilizado para análise de eficácia das intervenções e gestão de desempenho.
    /// </summary>
    /// <example>true, false</example>
    public ConfiabilidadeRestaurada? ConfiabilidadeRestaurada { get; set; }

    #endregion

    #region Planejamento e Execução

    /// <summary>
    /// Data e hora planejadas para o início da execução da ordem.
    /// Define o cronograma inicial da intervenção para alocação de recursos.
    /// </summary>
    /// <example>2024-01-20T08:00:00</example>
    public DateTime? InicioPlanejado { get; set; }

    /// <summary>
    /// Data e hora planejadas para o término da execução da ordem.
    /// Define a expectativa de conclusão para gestão de prazos e SLAs.
    /// </summary>
    /// <example>2024-01-20T17:00:00</example>
    public DateTime? FimPlanejado { get; set; }

    /// <summary>
    /// Data e hora reais em que a execução da ordem foi efetivamente iniciada.
    /// Registra o início efetivo da intervenção para cálculo de lead time real.
    /// </summary>
    /// <example>2024-01-20T08:15:00</example>
    public DateTime? InicioReal { get; set; }

    /// <summary>
    /// Data e hora reais em que a execução da ordem foi efetivamente concluída.
    /// Registra a conclusão efetiva para análise de performance e aderência ao planejado.
    /// </summary>
    /// <example>2024-01-20T16:45:00</example>
    public DateTime? FimReal { get; set; }

    #endregion

    #region Descrição e Documentação

    /// <summary>
    /// Descrição detalhada da ordem de trabalho.
    /// Contém informações sobre o problema, atividades necessárias e contexto da ordem.
    /// </summary>
    /// <example>Substituição do rolamento do motor principal devido a ruído anormal detectado durante inspeção de rotina.</example>
    [MaxLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
    public string? DescricaoBreve { get; set; }

    #endregion

    #region Auditoria e Rastreamento

    /// <summary>
    /// Identificação do usuário que criou a ordem no sistema.
    /// Utilizado para rastreabilidade e responsabilização pela abertura da ordem.
    /// </summary>
    /// <example>joao.silva@empresa.com, Admin-Sistema, Usuario-123</example>
    [MaxLength(150, ErrorMessage = "O identificador do usuário criador não pode exceder 150 caracteres.")]
    public string? CriadaPorUsuario { get; set; }

    /// <summary>
    /// Identificação do usuário que realizou a última modificação na ordem.
    /// Mantém histórico de quem editou a ordem por último para auditoria.
    /// </summary>
    /// <example>maria.santos@empresa.com, Supervisor-Manutencao</example>
    [MaxLength(150, ErrorMessage = "O identificador do usuário modificador não pode exceder 150 caracteres.")]
    public string? ModificadaPorUsuario { get; set; }

    /// <summary>
    /// Data e hora da última modificação realizada na ordem.
    /// Rastreia quando foi a última alteração para controle de versionamento.
    /// </summary>
    /// <example>2024-01-18T14:30:00</example>
    public DateTime? DataModificacao { get; set; }

    #endregion

    #region Relacionamentos

    /// <summary>
    /// O escopo da nota/pordem
    /// </summary>
    public long? Escopo { get; set; }

    /// <summary>
    /// Identificador do ativo relacionado à ordem (referência externa).
    /// Vincula a ordem ao equipamento ou ativo que será objeto da intervenção.
    /// Este campo armazena apenas o ID do ativo, sem relacionamento direto no EF.
    /// </summary>
    /// <example>5001, 10025</example>
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
    /// Identificador da nota associada à ordem.
    /// Chave estrangeira para vincular observações, comentários ou documentação adicional.
    /// </summary>
    /// <example>30001</example>
    public long? IdNota { get; set; }
    
    /// <summary>
    /// Identificador do plano de manutenção associado à ordem.
    /// Permite relacionar a ordem a um plano específico para análise de aderência e planejamento estratégico.
    /// </summary>
    public long? IdPlano { get; set; }

    /// <summary>
    /// Propriedade de navegação para a nota associada à ordem.
    /// Permite carregar informações complementares e observações relacionadas à ordem.
    /// </summary>
    [ForeignKey(nameof(IdNota))]
    public virtual NotaModel? Nota { get; set; }
    
    /// <summary>
    /// Plano de manutenção associado
    /// </summary>
    [ForeignKey(nameof(IdPlano))]
    public virtual PlanoModel? Plano { get; set; }
    
    /// <summary>
    /// Nome da estratégia relacionada ao Plano
    /// </summary>
    [NotMapped]
    [MaxLength(200)]
    public string? NomeEstrategia => Plano?.Estrategia?.Nome;

    /// <summary>
    /// Coleção de anexos (arquivos) associados à ordem.
    /// Inclui documentos, imagens, relatórios e evidências relacionadas à execução da ordem.
    /// Permite armazenamento de múltiplos arquivos para documentação completa.
    /// </summary>
    /// <example>Fotos do equipamento, laudos técnicos, certificados, desenhos técnicos</example>
    public virtual ICollection<AnexoOrdemModel> Anexos { get; set; } = new List<AnexoOrdemModel>();

    /// <summary>
    /// Coleção de operações vinculadas à ordem.
    /// Representa as atividades específicas ou etapas que compõem a execução da ordem.
    /// Permite decomposição da ordem em tarefas menores para melhor controle.
    /// </summary>
    /// <example>Desmontagem, Limpeza, Substituição de peças, Montagem, Teste funcional</example>
    public virtual ICollection<OperacaoModel> Operacoes { get; set; } = new List<OperacaoModel>();

    #endregion
}