using System.Text.RegularExpressions;
using app_pm_back.DTO;

namespace app_pm_back.Services;

public class NotificationService
{
    private readonly KafkaProducerService _kafkaProducerService;
    
    public NotificationService(KafkaProducerService kafkaProducerService)
    {
        _kafkaProducerService = kafkaProducerService;
    }

    /// <summary>
    /// Envia uma notificação via WhatsApp.
    /// </summary>
    /// <param name="mensagem">Mensagem a ser enviada.</param>
    /// <param name="nome">Nome do usuário.</param>
    /// <param name="telefone">Telefone do usuário.</param>
    public async Task SendNotification(string mensagem, string? nome, string telefone)
    {
        if (string.IsNullOrEmpty(telefone))
            throw new Exception("Telefone do usuário não encontrado para notificação. Especifique um telefone válido.");
    
        if (!ValidateTelefone(telefone))
        {
            telefone = FormatTelefone(telefone);
            if (!ValidateTelefone(telefone))
            {
                throw new Exception("Telefone do usuário não é válido");
            }
        }

        var mensagemPayload = new WhatsappMessagePayload
        {
            TOKEN_MSG = Guid.NewGuid().ToString(),
            telefone = telefone,
            nome = nome ?? "Usuário",
            mensagem = mensagem,
            tipoTemplateAvisoWhatsApp = "2"
        };
        
        Confluent.Kafka.Message<string? ,string> message = new Confluent.Kafka.Message<string?, string>
        {
            Key = null,
            Value = System.Text.Json.JsonSerializer.Serialize(mensagemPayload)
        };
            
        await _kafkaProducerService.ProduceAsync("message.wpp.created", message);
    }
    
    public static bool ValidateTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return false;

        // Expressão regular: 
        // ^        -> Início da linha
        // [0-9]{4} -> Exatamente 4 dígitos iniciais (ZZ + XX)
        // [0-9]{8} -> Exatamente 8 dígitos para o número (YYYYYYYY)
        // $        -> Fim da linha
        string padrao = @"^[0-9]{12}$";

        return Regex.IsMatch(telefone, padrao);
    }
    
    public static string FormatTelefone(string telefoneCru)
    {
        if (string.IsNullOrWhiteSpace(telefoneCru)) return string.Empty;

        // remove qualquer caractere que não seja número
        string apenasNumeros = Regex.Replace(telefoneCru, @"[^\d]", "");
        
        // remove 0 9 (posição 4 da string, índice 4)
        if (apenasNumeros.Length == 13 && apenasNumeros[4] == '9')
        {
            // remove 1 caractere a partir do índice 4
            apenasNumeros = apenasNumeros.Remove(4, 1);
        }

        // valida se após o ajuste o número ficou com exatamente 12 dígitos
        if (apenasNumeros.Length != 12)
        {
            throw new ArgumentException("O telefone após o ajuste não possui os 12 dígitos obrigatórios.");
        }

        return apenasNumeros;
    }
}