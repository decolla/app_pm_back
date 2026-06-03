using System.Text.Json;

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
    /// <returns></returns>
    public async Task<string> GetDataAsync(string rota)
    {
        // realiza requisição
        var response = await _httpClient.GetAsync(rota);
        
        // verifica se ocorreu
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    /// <summary>
    /// Envia dados para uma rota externa.
    /// </summary>
    /// <param name="rota">Rota externa</param>
    /// <param name="payload">O que será enviado</param>
    /// <returns></returns>
    public async Task PostDataAsync(string rota, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(rota, content);
        response.EnsureSuccessStatusCode();
    }
}