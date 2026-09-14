namespace app_pm_back.Models.Domain;

/// <summary>
/// Nivel de prioridade do plano.
/// </summary>
public enum Prioridade
{
    Baixa = 1,
    BaixaMedia = 2,
    Media = 3,
    MediaAlta = 4,
    Alta = 5
}

/// <summary>
/// Gerar automaticamente nota, ordem ou ambos.
/// </summary>
public enum GerarAutomaticamente
{
    Nenhum = 0,
    Nota = 1,
    Ordem = 2,
    Ambos = 3
}

/// <summary>
/// Acoes selecionadas em caso de falha.
/// </summary>
public enum AcoesFalha
{
    AbrirNota = 0,
    AbrirOrdem = 1,
    OpcaoTemplate = 2,
    Nada = 3
}

/// <summary>
/// Categoria do anexo.
/// </summary>
public enum CategoriaAnexo
{
    Outros = 0,
    Nota = 1,
    Ordem = 2
}

/// <summary>
/// Visibilidade do tipo de plano.
/// </summary>
public enum Visivel
{
    NaoVisivel = 0,
    Visivel = 1
}

/// <summary>
/// Tipo de tipo de manutencao/plano.
/// </summary>
public enum CategoriaTipoManutencao
{
    Manutencao = 0,
    Renovacao = 1
}

/// <summary>
/// Unidade do ciclo.
/// </summary>
public enum UnidadeTempo
{
    Dias = 0,
    Meses = 1,
    Anos = 2
}
/// <summary>
/// Confiabilidade restaurada apos a manutencao.
/// </summary>
public enum ConfiabilidadeRestaurada
{
    NaoRestaura = 0,
    RestauraAtivo = 1,
    RestauraLI = 2
}

/// <summary>
/// Origem da UR, se a UR foi gerada a partir de uma nota, ordem ou usuario.
/// </summary>
public enum OrigemUR
{
    Nota = 0,
    Ordem = 1,
    Usuario = 2
}

// ========================= APM ============================

/// <summary>
/// Tipo de componente do ativo/LI. Usado para requisições no APM específica para ativos/LI
/// </summary>
public enum ComponentType
{
    // --- Erp ---
    FlowSensor = 0,                  
    Shutoff = 1,                    
    PilotValve = 2,                 
    Regulator = 3,                  

    // --- Odorant ---
    FillValveV9 = 4,            
    ManifoldIn = 5,                  
    Verometer = 6,                
    OdorantPump = 7,             
    OdorantTank = 8,               
    ManifoldDischarge = 9,          
    GasFilter = 10,                  
    ManifoldPneumaticRelay = 11,     
    Ventsorv = 12,                   
    ExpansionTank = 13,              
    RegulatorValve = 14,             
    AlimentationPanel = 15,          
    Battery = 16,                    
    SolarPanel = 17,                 
    CommunicationInterface = 18,    
    Controller = 19,                 
    ChargeControllerPanel = 20,      
    Plc = 21,
            
    //ERP-New
    ERPFilter = 22,
    ReliefValve = 23,
    LineValve = 24,
    ERPMeter = 25,
}

/// <summary>
/// Tipos possiveis de LI
/// </summary>
public enum TiposLI
{
    Erp = 0,
    Crm = 1,
    Odorant = 2,
    Cable = 3,
    Substation = 4,
    CircuitBreaker = 5,
    Transformer = 6,
    Disconnector = 7,
    Bar = 8,
    ElectricMotor = 9,
    GenericEletricalCharge = 10,
    Feeder = 11
}

/// <summary>
/// Usado para requisições no APM específica para ativos/LI
/// </summary>
public enum OriginType
{
    GAS = 0,
    ELETRICO = 1,
}

/// <summary>
/// Usado para classificar os tipos de fichas em dados renovação
/// </summary>
public enum Classificacao
{
    NAOCLASSIFICADA = 0,
    CORRETIVA = 1,
    PREVENTIVA= 2,
    OCORRENCIAALHEIA = 3
}

/// <summary>
/// Representa o método utilizado para uma classificação.
/// </summary>
public enum MetodoClassificacao
{
    NAOCLASSIFICADA = 0,
    ALGORITMO = 1,
    CATBOOST = 2,
    HUMANA = 3,
}

/// <summary>
/// Status atual das fichas geradas.
/// </summary>
public enum StatusFichas
{
    Reprovada = 0,
    Aberta = 1,
    EmAndamento = 2,
    Concluida = 3
}

/// <summary>
/// Tipo de fichas a serem geradas durante a busca de fichas.
/// </summary>
public enum Fichas
{
    Ambas = 0,
    Notas = 1,
    Ordens =2
}

public enum Grupos
{
    Gpm = 0,
    Ctm = 1
}

/// <summary>
/// Filtros para classificação de fichas.
/// </summary>
public enum FiltroClassificacao
{
    Ambos = 0,
    Catboost = 1,
    Algoritmo = 2,
}

/// <summary>
/// Define se um usuário é Admin ou não. ADICIONAR MAIS NÍVEIS DEPOIS
/// </summary>
public enum NivelPermissao
{
    Usuario = 0, 
    Adm = 1
}

/// <summary>
/// A quais tipos de tabela um usuário terá acesso 
/// </summary>
[Flags]
public enum PermissoesTabela
{
    Nenhuma = 0,
    Notas = 1,          // 2^0
    Ordens = 2,         // 2^1
    Itens = 4,          // 2^2
    Gpms = 8,           
    Ctms = 16,          
    Cpms = 32, 
    Planos = 64,
    Estrategias = 128,
    TiposManutencao = 256,
    Almoxarifados = 512,
    
    
    // combinação de todas as permissões
    Tudo = Notas | Ordens | Itens | Gpms | Ctms | Cpms | Almoxarifados | Planos | Estrategias | TiposManutencao
}

// ======================= CONFIGURAÇÕES GLOBAIS ==========================

public enum OrigemFichasConfiabilidade
{
    ApenasConfirmadasUsuario = 1,
    ConsiderarTambemAlgoritmo = 2
}

public enum ModoClassificacaoAutomatica
{
    NaoClassificar = 0,
    ClassificarCatBoost = 1,
    ClassificarPalavrasChave = 2
}

public enum CriterioDataRenovacao
{
    ApenasFichasPreventivas = 1,
    ApenasFichasCorretivas = 2,
    ConsiderarTodasClassificacoes = 3
}

public enum CriterioConfiabilidadeRenovacao
{
    ApenasRenovacoesConfirmadasUsuario = 1,
    ConsiderarTambemAlgoritmo = 2
}

public enum CriarFichasPreventivas
{
    NaoCriar = 0, 
    Automaticamente = 1
}