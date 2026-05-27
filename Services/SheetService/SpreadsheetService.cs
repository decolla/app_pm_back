using System.Globalization;
using System.Reflection;
using System.Text;
using app_pm_back.Attributes;
using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models.Domain;
using Microsoft.VisualBasic.FileIO;
using api.Communications.Service.Impl.Data;

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
        public List<NotaModel> NotasCongas { get; set; } = new();
        public List<OrdemModel> OrdensCongas { get; set; } = new();
        public List<ItemNotaModel> ItensCongas { get; set; } = new();
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

        CultureInfo culture = CultureInfo.GetCultureInfo("pt-BR");

        using TextFieldParser parser =
            new TextFieldParser(csvStream, Encoding.UTF8, detectEncoding: true, leaveOpen: false)
            {
                TextFieldType = FieldType.Delimited
            };

        parser.SetDelimiters(";");
        parser.HasFieldsEnclosedInQuotes = false;
        parser.TrimWhiteSpace = true;

        string[]? headers = null;
        Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);

        while (!parser.EndOfData)
        {
            headers = parser.ReadFields();
            if (headers == null) continue;

            columns.Clear();
            for (int i = 0; i < headers.Length; i++)
            {
                string cleanHeader = headers[i].Replace("\uFEFF", "").Replace("\"", "").Trim();
                if (!string.IsNullOrWhiteSpace(cleanHeader))
                {
                    columns[cleanHeader] = i;
                }
            }

            if (columns.ContainsKey("Nota") ||
                columns.ContainsKey("Ordem") ||
                columns.ContainsKey("Item") ||
                columns.ContainsKey("Objeto") ||
                columns.ContainsKey("Área operacion.")
               )
            {
                break;
            }
        }

        if (columns.Count == 0) return resultado;

        bool isNotaCongas = columns.ContainsKey("Nota") && columns.ContainsKey("Tipo de nota");
        bool isOrdemCongas = columns.ContainsKey("Ordem") && columns.ContainsKey("Texto breve");
        bool isItemCongas = columns.ContainsKey("Item") && columns.ContainsKey("Causa") &&
                            columns.ContainsKey("Texto da causa");

        if (isOrdemCongas)
        {
            resultado.TipoIdentificado = "ORDEM_CONGAS";
        }
        else if (isItemCongas)
        {
            resultado.TipoIdentificado = "ITEM_CONGAS";
        }
        else if (isNotaCongas)
        {
            resultado.TipoIdentificado = "NOTA_CONGAS";
        }
        else
        {
            return resultado;
        }

        while (!parser.EndOfData)
        {
            string[] row = parser.ReadFields();
            if (row == null || row.All(string.IsNullOrWhiteSpace)) continue;

            if (resultado.TipoIdentificado == "NOTA_CONGAS")
            {
                var dto = ReadLineForModel<NotaManutencaoComgasDto>(row, columns);
                resultado.NotasCongas.Add(MapNotaComgasDto(dto));
            }
            else if (resultado.TipoIdentificado == "ORDEM_CONGAS")
            {
                var dto = ReadLineForModel<OrdemManutencaoComgasDto>(row, columns);
                resultado.OrdensCongas.Add(MapOrdemComgasDto(dto));
            }
            else if (resultado.TipoIdentificado == "ITEM_CONGAS")
            {
                var dto = ReadLineForModel<ItemNotaManutencaoComgasDto>(row, columns);
                resultado.ItensCongas.Add(MapItemNotaComgasDto(dto));
            }
        }

        return resultado;
    }

    public T ReadLineForModel<T>(string[] row, Dictionary<string, int> columns) where T : class, new()
    {
        var model = new T();

        foreach (PropertyInfo property in model.GetType().GetProperties())
        {
            var atributo = property.GetCustomAttribute<SheetColumnAttribute>();

            if (atributo != null)
            {
                string nomeColunaExcel = atributo.NomeDaColuna;

                if (columns.TryGetValue(nomeColunaExcel.ToUpperInvariant(), out var idx) && idx < row.Length)
                {
                    string valor = row[idx].Trim();

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        try
                        {
                            Type tipo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object? valorConvertido = null;

                            if (tipo == typeof(long) || tipo == typeof(int))
                            {
                                string numerosApenas = new string(valor.Where(char.IsDigit).ToArray());
                                if (!string.IsNullOrEmpty(numerosApenas))
                                {
                                    valorConvertido = Convert.ChangeType(numerosApenas, tipo);
                                }
                            }
                            else if (tipo == typeof(TimeSpan))
                            {
                                if (valor == "24:00:00") valor = "23:59:59";
                                if (TimeSpan.TryParse(valor, out var ts))
                                {
                                    valorConvertido = ts;
                                }
                            }
                            else if (tipo == typeof(DateTime))
                            {
                                if (DateTime.TryParse(valor, CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None,
                                        out var dt))
                                {
                                    valorConvertido = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                                }
                            }
                            else
                            {
                                valorConvertido = Convert.ChangeType(valor, tipo);
                            }

                            if (valorConvertido != null)
                            {
                                property.SetValue(model, valorConvertido);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao converter o valor da coluna '{nomeColunaExcel}': {ex.Message}");
                        }
                    }
                }
            }
        }

        return model;
    }

    private static NotaModel MapNotaComgasDto(NotaManutencaoComgasDto dto)
    {
        // converte string em tipo Prioridade
        Enum.TryParse<Prioridade>(dto.Prioridade, out Prioridade prioridade);
        return new NotaModel
        {
            IdNota = dto.IdNota != 0 ? dto.IdNota : dto.NumeroNota,
            NumeroNota = dto.NumeroNota,
            Classificacao = dto.Classificacao,
            MetodoClassificacao = dto.MetodoClassificacao,
            StatusNota = dto.StatusNota,
            DataNota = dto.DataNota,
            Prioridade = prioridade,
            FimDesejado = dto.FimDesejado,
            DataAvaria = dto.DataAvaria,
            HoraAvaria = dto.HoraAvaria,
            EquipamentoDisponivel = dto.EquipamentoDisponivel,
            AnomaliaObservada = dto.AnomaliaObservada,
            ObservacoesComplementares = dto.ObservacoesComplementares,
            ReferenciaLocalizacaoAtivo = dto.ReferenciaLocalizacaoAtivo,
            Finalidade = string.IsNullOrWhiteSpace(dto.Finalidade) ? "N/A" : dto.Finalidade,
            IdPlano = dto.IdPlano,
            IdUsuario = dto.IdUsuario,
            IdAtivo = dto.IdAtivo,
            IdLi = dto.IdLi,
            CriadaPorUsuario = dto.CriadaPorUsuario,
            ModificadaPorUsuario = dto.ModificadaPorUsuario,
            DataModificacao = dto.DataModificacao,
            ConfiabilidadeRestaurada = dto.ConfiabilidadeRestaurada
        };
    }

    private static OrdemModel MapOrdemComgasDto(OrdemManutencaoComgasDto dto)
    {
        return new OrdemModel
        {
            IdOrdem = dto.IdOrdem,
            Classificacao = dto.Classificacao,
            MetodoClassificacao = dto.MetodoClassificacao,
            StatusOrdem = dto.StatusOrdem,
            IdNota = dto.IdNota,
            Criticidade = dto.Prioridade?.ToString() ?? dto.Criticidade,
            DataCriacao = dto.DataCriacao ?? DateTime.UtcNow,
            ConfiabilidadeRestaurada = dto.ConfiabilidadeRestaurada ?? ConfiabilidadeRestaurada.NaoRestaura,
            IdAtivo = dto.IdAtivo
        };
    }

    private static ItemNotaModel MapItemNotaComgasDto(ItemNotaManutencaoComgasDto dto)
    {
        return new ItemNotaModel
        {
            IdItemNota = dto.IdItemNota,
            IdNota = dto.IdNota,
            Componente = dto.Componente,
            Sintoma = dto.Sintoma,
            Causa = dto.Causa
        };
    }
}