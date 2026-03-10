using System.Globalization;
using System.Text;
using api.Communications.Service.Impl.Data;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using Microsoft.VisualBasic.FileIO;

namespace app_pm_back.Services;

public class SpreadsheetService
{
    private readonly ConvertService _convertService;

    public SpreadsheetService(ConvertService convertService)
    {
        _convertService = convertService;
    }
    
    public class ResultadoPlanilha
    {
        public string TipoIdentificado { get; set; } = "DESCONHECIDO";
        public List<NotaPetroModel> NotasPetro { get; set; } = new();
        public List<NoteCongasModel> NotasCongas { get; set; } = new();
    }

    public async Task<ResultadoPlanilha> ReadNotasSpreadSheetAsync(IFormFile spreadSheet)
    {
        var resultado = new ResultadoPlanilha();
        
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

        if (parser.EndOfData) return resultado;

        string[]? header = parser.ReadFields();
        if (header == null || header.Length == 0) return resultado;
        
        // Lê os cabeçalhos
        string[] headers = parser.ReadFields();
        Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < headers.Length; i++)
        {
            columns[headers[i].Trim()] = i;
        }
        
        bool isPetro = columns.ContainsKey("Campo seleção") && columns.ContainsKey("CondInstAntAvar");
        bool isCongas = columns.ContainsKey("Nota") && columns.ContainsKey("Tipo de nota");

        if (isPetro)
        {
            resultado.TipoIdentificado = "PETRO";
        }
        else if (isCongas)
        {
            resultado.TipoIdentificado = "CONGAS";
        }
        else
        {
            // Não identificou nenhuma das duas
            return resultado; 
        }
        
        while (!parser.EndOfData)
        {
            string[] row = parser.ReadFields();
            if (row == null || row.All(string.IsNullOrWhiteSpace)) continue;

            if (resultado.TipoIdentificado == "PETRO")
            {
                // SEU MAPEAMENTO ANTIGO DA PETRO VEM PARA CÁ
                var notaPetro = new NotaPetroModel
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
                resultado.NotasPetro.Add(notaPetro);
            }
            else if (resultado.TipoIdentificado == "CONGAS")
            {
                // NOVO MAPEAMENTO DA CONGAS 
                var notaCongas = new NoteCongasModel
                {
                    NumeroNota = SafeLong("Nota", row, columns) ?? 0,
                    TipoNota = SafeText("Tipo de nota", row, columns) ?? "MC",
                    LocInstalacao = SafeText("Loc.instalação", row, columns) ?? "N/A",
                    StatusSistema = SafeText("Status sistema", row, columns) ?? "Sem status",
                    StatusUsuario = SafeText("Status usuário", row, columns) ?? "Sem status",
                    Descricao = SafeText("Descrição", row, columns) ?? "Sem descrição",
                    Rua = SafeText("Rua", row, columns) ?? "N/A",
                    Bairro = SafeText("Bairro", row, columns) ?? "N/A",
                    Local = SafeText("Local", row, columns) ?? "N/A",
                    DataNota = SafeDate("Data da nota", row, columns, culture) ?? DateTime.UtcNow,
                    ModificadoEm = SafeDate("Modificado em", row, columns, culture),
                    ModificadoPor = SafeText("Modificado por", row, columns),
                    ModificadoAs = SafeDate("Modificado às", row, columns, culture),
                    TipoPrioridade = SafeText("Tipo prioridade", row, columns) ?? "Sem prioridade",
                    InicioAvar = SafeDate("InícioAvar", row, columns, culture),
                    FimAvaria = SafeDate("Fim avaria", row, columns, culture),
                    HoraInicioAvaria = SafeDate("HoraInícioAvar.", row, columns, culture),
                    HoraFimAvaria = SafeDate("Hora fim avaria", row, columns, culture),
                    DtReferencia = SafeDate("Dt.referência", row, columns, culture),
                    HoraReferencia = SafeDate("Hora referência", row, columns, culture),
                    Parada = SafeText("Parada", row, columns),
                    DuracParada = SafeInt("Duraç.parada", row, columns),
                    DtCriacao = SafeDate("Dt.criação", row, columns, culture),
                    CriadoPor = SafeText("Criado por", row, columns),
                    CriadoAs = SafeDate("Criado às", row, columns, culture),
                    Notificador = SafeText("Notificador", row, columns),
                    PerfCatalogo = SafeText("Perf.catálogo", row, columns ),
                    CenTrabRespon = SafeText("CenTrab respon.", row, columns),
                    TxtCodeCodif = SafeText("Txt.code codif.", row, columns),
                    TxtGrpCodific = SafeText("Txt.grp.codific", row, columns),
                    LocInstAfetado = SafeText("Loc.inst.afetad", row, columns),
                    Equipamento = SafeText("Equipamento", row, columns),
                    Denominacao = SafeText("Denominação", row, columns),
                    GrpPlnjPM = SafeText("Grp.plnj.PM", row, columns),
                    AreaOperacion = SafeText("Área operacion.",row, columns),
                    Codificacao = SafeText("Codificação", row, columns),
                    Consequencia = SafeInt("Conseqüência", row, columns),
                    GrupoCodes = SafeText("Grupo de codes", row, columns),
                    IdOrdem = SafeInt("Ordem", row, columns)
                };
                resultado.NotasCongas.Add(notaCongas);
            }
        }
        return resultado;
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