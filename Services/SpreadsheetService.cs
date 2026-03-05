using System.Globalization;
using System.Text;
using api.Communications.Service.Impl.Data;
using app_pm_back.Models;
using Microsoft.VisualBasic.FileIO;

namespace app_pm_back.Services;

public class SpreadsheetService
{
    private readonly ConvertService _convertService;

    public SpreadsheetService(ConvertService convertService)
    {
        _convertService = convertService;
    }

    public async Task<List<NotaPetroModel>> ReadNotasSpreadSheetAsync(IFormFile spreadSheet)
    {
        var extension = Path.GetExtension(spreadSheet.FileName).ToLower();
        Stream csvStream;

        if (extension == ".xlsx")
        {
            await using var xlsxStream = spreadSheet.OpenReadStream();
            csvStream = await _convertService.ConvertXlsxStreamToCsvMemoryAsync(
                xlsxStream,
                sheetIndex: 0,
                separator: ';',
                utf8WithBom: true
            );
        }
        else
        {
            csvStream = spreadSheet.OpenReadStream();
        }

        csvStream.Position = 0;
        List<NotaPetroModel> notas = new();
        const string separator = ";";
        
        // Padrão pt-BR para ler datas como DD/MM/AAAA
        CultureInfo culture = CultureInfo.GetCultureInfo("pt-BR");

        using TextFieldParser parser = new TextFieldParser(csvStream, Encoding.UTF8, detectEncoding: true, leaveOpen: false)
        {
            TextFieldType = FieldType.Delimited
        };

        parser.SetDelimiters(separator);
        parser.HasFieldsEnclosedInQuotes = true;
        parser.TrimWhiteSpace = true;

        if (parser.EndOfData) return notas;

        string[]? header = parser.ReadFields();
        if (header == null || header.Length == 0) return notas;

        Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < header.Length; i++)
        {
            string title = header[i].Trim();
            if (!string.IsNullOrWhiteSpace(title))
                columns[title.ToUpperInvariant()] = i;
        }

        while (!parser.EndOfData)
        {
            string[]? row = parser.ReadFields();
            if (row == null) continue;

            try
            {
                // NOTA: Ajuste as strings do 1º parâmetro para bater EXATAMENTE
                // com os nomes dos cabeçalhos na sua planilha Excel/CSV
                var nota = new NotaPetroModel
                {
                    IdOrdem = SafeLong("Ordem", row, columns),
    
                    // Nomes mapeados EXATAMENTE como estão no cabeçalho do Excel/CSV
                    Campo = SafeText("Campo seleção", row, columns) ?? "N/A", 
                    Descricao = SafeText("Descrição", row, columns) ?? "Sem descrição",
                    LocInstalacao = SafeText("Loc.instalação", row, columns) ?? "N/A",
                    LocInstAfetado = SafeText("Loc.inst.afetad", row, columns),
                    AreaOp = SafeInt("Área operacion.", row, columns) ?? 0,
                    CenTrabPrinc = SafeText("CenTrabPrinc.", row, columns) ?? "",
                    DataNota = SafeDate("Data da nota", row, columns, culture) ?? DateTime.UtcNow,
                    Localizacao = SafeText("Localização", row, columns) ?? "",
                    GrupoPlanej = SafeText("Grupo planej.", row, columns) ?? "",
                    TipoNota = SafeText("Tipo de nota", row, columns) ?? "M1",
                    Impacto = SafeInt("Impacto da Falha", row, columns) ?? 0,
                    Parada = SafeChar("Parada", row, columns),
                    InicioIndisp = SafeDate("Início indisp", row, columns, culture),
                    FimIndisp = SafeDate("Fim indisp", row, columns, culture),
                    CondInstAntAvar = SafeInt("CondInstAntAvar", row, columns),
                    CondInstDepAvar = SafeInt("CondInstDepAvar", row, columns),
                    CondInstAposMed = SafeInt("CondInstApósMed", row, columns)
                };

                notas.Add(nota);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar linha: {ex.Message}");
            }
        }

        return notas;
    }

    // ================= MÉTODOS AUXILIARES SEGUROS =================

    private string? SafeText(string colName, string[] row, Dictionary<string, int> columns)
    {
        if (!columns.TryGetValue(colName.ToUpperInvariant(), out var idx)) return null;
        if (idx < 0 || idx >= row.Length) return null;
        string value = row[idx].Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private int? SafeInt(string colName, string[] row, Dictionary<string, int> columns)
    {
        var text = SafeText(colName, row, columns);
        if (string.IsNullOrWhiteSpace(text)) return null;
        return int.TryParse(text, out var val) ? val : null;
    }

    private long? SafeLong(string colName, string[] row, Dictionary<string, int> columns)
    {
        var text = SafeText(colName, row, columns);
        if (string.IsNullOrWhiteSpace(text)) return null;
        return long.TryParse(text, out var val) ? val : null;
    }

    private DateTime? SafeDate(string colName, string[] row, Dictionary<string, int> columns, CultureInfo culture)
    {
        var text = SafeText(colName, row, columns);
        if (string.IsNullOrWhiteSpace(text)) return null;
    
        if (DateTime.TryParse(text, culture, DateTimeStyles.None, out var dt))
        {
            // Força o C# a dizer para o Postgres que essa data é UTC
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc); 
        }
        return null;
    }

    private char? SafeChar(string colName, string[] row, Dictionary<string, int> columns)
    {
        var text = SafeText(colName, row, columns);
        if (string.IsNullOrWhiteSpace(text) || text.Length == 0) return null;
        return text[0]; // Retorna 'X' por exemplo
    }
}