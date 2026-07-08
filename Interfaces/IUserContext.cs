namespace app_pm_back.Interfaces;

/// <summary>
/// Interface para controle do contexto do usuário, incluindo escopos e identificação do usuário atual.
/// </summary>
public interface IUserContext
{
    List<long> GetEscoposDoUsuario();

    int? GetUsuarioIdAtual();
}