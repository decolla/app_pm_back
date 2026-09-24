namespace app_pm_back.DTO;

public class WhatsappMessagePayload
{
    // se não enviado, gera um UUID automaticamente
    public string? TOKEN_MSG { get; set; } 
    public string? telefone { get; set; } = string.Empty;
    public string nome { get; set; } = string.Empty;
    public string mensagem { get; set; } = string.Empty;
    
    // template que aceitam [nome, mensagem] para receber status de volta no Kafka. Pode ser 1 ou 4
    public string tipoTemplateAvisoWhatsApp { get; set; } = "1";
}