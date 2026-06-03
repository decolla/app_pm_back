using System.Globalization;
using System.Reflection;
using System.Text;
using app_pm_back.Attributes;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using Microsoft.VisualBasic.FileIO;
using api.Communications.Service.Impl.Data;

namespace app_pm_back.Services;

/// <summary>
/// Classe SpreadsheetService.
/// </summary>
public class SpreadsheetService
{
    private static readonly (ComponentType Type, string[] Terms)[] ComponentTypeMatchers =
    [
        (ComponentType.Shutoff, new[] { "shut-off", "shutoff", "shut off", "sbr", "sdv", "bloqueio remoto", "sistema bloqueio remoto", "valvula saida"}),
        (ComponentType.PilotValve, new[] { "valvula piloto", "pilot valve", "pilot", "piloto" }),
        (ComponentType.RegulatorValve, new[] { "valvula reguladora", "regulator valve" }),
        (ComponentType.Regulator, new[] { "regulador", "regulator", "reguladores" }),
        (ComponentType.FlowSensor, new[] { "flow", "flowtrail", "flow sensor", "flow trail", "sensor de vazao", "sensor de fluxo", "medidor" }),
        (ComponentType.FillValveV9, new[] { "abastecimento de odorante", "valvula de abastecimento", "fill valve", "v9" }),
        (ComponentType.OdorantPump, new[] { "bomba odorante", "bomba de odorante", "odorante bomba", "pump", "bomba" }),
        (ComponentType.OdorantTank, new[] { "tanque de odorante", "tanque odorante", "reservatorio de odorante" }),
        (ComponentType.ManifoldIn, new[] { "manifold in", "manifold entrada" }),
        (ComponentType.ManifoldDischarge, new[] { "manifold discharge", "manifold saida" }),
        (ComponentType.GasFilter, new[] { "filtro", "filtrante", "filtro de gas" }),
        (ComponentType.ManifoldPneumaticRelay, new[] { "rele pneumatico", "pneumatic relay" }),
        (ComponentType.Ventsorv, new[] { "venteio", "vent" }),
        (ComponentType.ExpansionTank, new[] { "tanque de expansao", "expansion tank" }),
        (ComponentType.AlimentationPanel, new[] { "painel de alimentacao", "nobreak" }),
        (ComponentType.Battery, new[] { "bateria", "baterias" }),
        (ComponentType.SolarPanel, new[] { "painel solar", "solar" }),
        (ComponentType.CommunicationInterface, new[] { "comunicacao", "telemetria", "modem", "vsat", "antena", "chip", "ethernet", "serial" }),
        (ComponentType.Controller, new[] { "controlador", "scada", "ihm", "datalogger", "data logger" }),
        (ComponentType.ChargeControllerPanel, new[] { "controlador de carga", "painel controlador de carga" }),
        (ComponentType.Plc, new[] { "plc" })
    ];

    private readonly ConvertService _convertService;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="SpreadsheetService"/>.
    /// </summary>
    public SpreadsheetService(ConvertService convertService)
    {
        _convertService = convertService;
    }

    /// <summary>
    /// Classe ResultadoPlanilha.
    /// </summary>
    public class ResultadoPlanilha
    {
        /// <summary>
        /// Obtém ou define o valor de TipoIdentificado.
        /// </summary>
        public string TipoIdentificado { get; set; } = "DESCONHECIDO";

        /// <summary>
        /// Obtém ou define o valor de Notas.
        /// </summary>
        public List<NotaModel> Notas { get; set; } = new();

        /// <summary>
        /// Obtém ou define o valor de Ordens.
        /// </summary>
        public List<OrdemModel> Ordens { get; set; } = new();

        /// <summary>
        /// Obtém ou define o valor de Itens.
        /// </summary>
        public List<ItemNotaModel> Itens { get; set; } = new();
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="spreadSheet">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
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

        bool isNota = columns.ContainsKey("Nota") && columns.ContainsKey("Tipo de nota");
        bool isOrdem = columns.ContainsKey("Ordem") && columns.ContainsKey("Texto breve");
        bool isItem = columns.ContainsKey("Item") && columns.ContainsKey("Causa") &&
                      columns.ContainsKey("Texto da causa");

        if (isOrdem)
        {
            resultado.TipoIdentificado = "ORDEM";
        }
        else if (isItem)
        {
            resultado.TipoIdentificado = "ITEM";
        }
        else if (isNota)
        {
            resultado.TipoIdentificado = "NOTA";
        }
        else
        {
            return resultado;
        }

        while (!parser.EndOfData)
        {
            string[] row = parser.ReadFields();
            if (row == null || row.All(string.IsNullOrWhiteSpace)) continue;

            if (resultado.TipoIdentificado == "NOTA")
            {
                var dto = ReadLineForModel<NotaManutencaoDto>(row, columns);
                resultado.Notas.Add(MapNotaDto(dto));
            }
            else if (resultado.TipoIdentificado == "ORDEM")
            {
                var dto = ReadLineForModel<OrdemManutencaoDto>(row, columns);
                resultado.Ordens.Add(MapOrdemDto(dto));
            }
            else if (resultado.TipoIdentificado == "ITEM")
            {
                var dto = ReadLineForModel<ItemNotaManutencaoDto>(row, columns);
                resultado.Itens.Add(MapItemNotaDto(dto));
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

                            if (tipo == typeof(ComponentType) &&
                                string.Equals(property.Name, "IdAtivo", StringComparison.OrdinalIgnoreCase))
                            {
                                valorConvertido = TryTranslateComponentType(valor);
                            }
                            else if (tipo == typeof(string) &&
                                        string.Equals(property.Name, "AnomaliaObservada", StringComparison.OrdinalIgnoreCase))
                            {
                                valorConvertido = TryTranslateComponentType(valor);
                            }
                            else if (tipo == typeof(long) || tipo == typeof(int))
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

    private static ComponentType? TryTranslateComponentType(string rawValue)
    {
        var normalized = NormalizeText(rawValue);
        if (string.IsNullOrEmpty(normalized))
            return null;

        foreach (var (type, terms) in ComponentTypeMatchers)
        {
            if (terms.Any(term => normalized.Contains(term)))
                return type;
        }

        return null;
    }

    private static string NormalizeText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    private static NotaModel MapNotaDto(NotaManutencaoDto dto)
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

    private static OrdemModel MapOrdemDto(OrdemManutencaoDto dto)
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

    private static ItemNotaModel MapItemNotaDto(ItemNotaManutencaoDto dto)
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