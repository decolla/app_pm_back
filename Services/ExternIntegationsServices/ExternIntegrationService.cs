using System.Net.Http.Headers;
using System.Text.Json;

namespace app_pm_back.Services.ExternIntegationsServices;

public class ExternIntegrationService
{
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="ExternIntegrationService"/>.
    /// </summary>
    /// <param name="httpClient"></param>
    public ExternIntegrationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    /// <summary>
    /// Obtém dados de uma rota externa.
    /// </summary>
    /// <param name="rota">Rota externa de requisição</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    public async Task<string> GetDataAsync(string rota, CancellationToken cancellationToken = default)
    {
        var relativeRoute = rota.StartsWith("/") ? rota.Substring(1) : rota;
        var request = new HttpRequestMessage(HttpMethod.Get, relativeRoute);
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());

        // realiza requisição
        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        // verifica se ocorreu
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
    
    /// <summary>
    /// Envia dados para uma rota externa.
    /// </summary>
    /// <param name="rota">Rota externa</param>
    /// <param name="payload">O que será enviado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns></returns>
    public async Task PostDataAsync(string rota, object payload, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var relativeRoute = rota.StartsWith("/") ? rota.Substring(1) : rota;
        var request = new HttpRequestMessage(HttpMethod.Post, relativeRoute)
        {
            Content = content
        };
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}