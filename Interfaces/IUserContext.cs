namespace app_pm_back.Interfaces;

public interface IUserContext
{
    List<long> GetEscoposDoUsuario();

    int? GetUsuarioIdAtual();
}