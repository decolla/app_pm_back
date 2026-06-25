namespace app_pm_back.Interfaces;

public interface IUserContext
{
    long? GetEscopoAtual();

    int? GetUsuarioIdAtual();
}