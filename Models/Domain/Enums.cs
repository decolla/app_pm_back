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
/// Categoria da estrategia.
/// </summary>
/// public enum CategoriaTipoEstrategia
/// {
    ///Manutencao = 0,
    ///Renovacao = 1
///}

/// <summary>
/// Tipo de tipo de manutencao/plano.
/// </summary>
public enum CategoriaTipoManutencao
{
    Manutencao = 0,
    Renovacao = 1
}

/// <summary>
/// Tipo de tipo de manutencao/plano.
/// </summary>
/// public enum TipoTipo
/// {
    /// Manutencao = 0,
    /// Renovacao = 1
/// }

/// <summary>
/// Unidade do ciclo.
/// </summary>
public enum UnidadeCiclo
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
    Plc = 21                        
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
    PREVENTIVACAT = 0,
    CORRETIVACAT = 1,
    PREVENTIVAHUM = 2,
    CORRETIVAHUM = 3,
    NAOCLASSIFICADA =4
}

public enum Status
{
    Reprovada = 0,
    Aberta = 1,
    EmAndamento = 2,
    Concluida = 3
}

public enum Fichas
{
    Ambas = 0,
    Notas = 1,
    Ordens =2
}