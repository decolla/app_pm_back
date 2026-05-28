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
public enum CategoriaTipoEstrategia
{
    Manutencao = 0,
    Renovacao = 1
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
/// Tipo de tipo de manutencao/plano.
/// </summary>
public enum TipoTipo
{
    Manutencao = 0,
    Renovacao = 1
}

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