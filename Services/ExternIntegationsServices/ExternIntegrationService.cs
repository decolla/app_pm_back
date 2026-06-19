using System.Text.Json;
using System.Net.Http.Headers;

namespace app_pm_back.Services;

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
        var request = new HttpRequestMessage(HttpMethod.Get, rota);
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

        var request = new HttpRequestMessage(HttpMethod.Post, rota)
        {
            Content = content
        };
        request.Headers.Authorization = 
            new AuthenticationHeaderValue("Bearer", TokenService.GenerateToken());
        
        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}