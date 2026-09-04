using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text;
using app_pm_back.Attributes;
using app_pm_back.DTO;
using app_pm_back.Models.Domain;
using Microsoft.VisualBasic.FileIO;
using api.Communications.Service.Impl.Data;
using app_pm_back.Data;
using Microsoft.EntityFrameworkCore;

namespace app_pm_back.Services;

/// <summary>
/// Classe SpreadsheetService.
/// </summary>
public class SpreadsheetService
{
    // OTIMIZAÇÃO: Cache para não ler Reflection a cada linha da planilha
    private static readonly ConcurrentDictionary<Type, List<(PropertyInfo Property, string ColName)>> _propertyCache =
        new();

    // mapeamento de string para ComponentType
    private static readonly (ComponentType Type, string[] Terms)[] ComponentTypeMatchers =
    [
        (ComponentType.FlowSensor,
            new[] { "flow", "flowtrail", "flow sensor", "flow trail", "sensor de vazao", "sensor de fluxo" }),

        (ComponentType.Shutoff,
            new[] { "shut-off", "shutoff", "shut off", "sbr", "sdv", "bloqueio remoto", "sistema bloqueio remoto" }),

        (ComponentType.PilotValve,
            new[] { "valvula piloto", "pilot valve", "pilot", "piloto", "pre-piloto" }),

        (ComponentType.RegulatorValve,
            new[] { "valvula reguladora", "regulator valve" }),

        (ComponentType.Regulator,
            new[] { "regulador", "regulator", "reguladores" }),

        (ComponentType.FillValveV9,
            new[] { "abastecimento de odorante", "valvula de abastecimento", "fill valve", "v9" }),

        (ComponentType.OdorantPump,
            new[] { "bomba odorante", "bomba de odorante", "odorante bomba", "pump", "bomba" }),

        (ComponentType.OdorantTank,
            new[] { "tanque de odorante", "tanque odorante", "reservatorio de odorante" }),

        (ComponentType.ManifoldIn,
            new[] { "manifold in", "manifold entrada" }),

        (ComponentType.ManifoldDischarge,
            new[] { "manifold discharge", "manifold saida" }),

        (ComponentType.GasFilter,
            new[] { "filtrante", "filtro de gas" }),

        (ComponentType.ManifoldPneumaticRelay,
            new[] { "rele pneumatico", "pneumatic relay" }),

        (ComponentType.Ventsorv,
            new[] { "venteio", "vent" }),

        (ComponentType.ExpansionTank,
            new[] { "tanque de expansao", "expansion tank" }),

        (ComponentType.AlimentationPanel,
            new[]
            {
                "painel de alimentacao", "nobreak", "painel eletrico", "aterramento"
            }), // DUVIDOSO: 'aterramento' mapeado aqui por falta de categoria elétrica específica

        (ComponentType.Battery,
            new[] { "bateria", "baterias" }),

        (ComponentType.SolarPanel,
            new[] { "painel solar", "solar" }),

        (ComponentType.CommunicationInterface,
            new[]
            {
                "comunicacao", "telemetria", "modem", "vsat", "antena", "chip", "ethernet", "serial", "transmissores",
                "sinal de operadora"
            }),

        (ComponentType.Controller,
            new[]
            {
                "controlador", "scada", "ihm", "datalogger", "data logger", "data-logger", "alarme",
                "caixa do datalogger", "poste do datalogger",
                "crm", "erp", "erd", "cm", "cr", "crc", "ers", "estacao", "indicadores cspe", "civil erp", "civil crc",
                "civil crm", "civil erd",
                "civil marco de sinalizacao", "civil instalacao de alvenaria", "civil pavimentacao",
                "civil poste do datalogger", "civil poste de respiro"
            }), // DUVIDOSO: estações físicas, obras civis genéricas e alarmes

        (ComponentType.ChargeControllerPanel,
            new[] { "controlador de carga", "painel controlador de carga" }),

        (ComponentType.Plc,
            new[] { "plc" }),

        (ComponentType.ERPFilter,
            new[] { "filtro" }),

        (ComponentType.ReliefValve,
            new[] { "valvula de alivio", "alivio" }),

        (ComponentType.LineValve,
            new[]
            {
                "valvula da instalacao", "valvulas de bloqueio", "valvula de retencao", "vb de rede",
                "valvula de entrada", "valvula de saida", "valvula vgb", "civil vgb", "civil valvula de rede"
            }),

        (ComponentType.ERPMeter,
            new[] { "medidor" })
    ];

    private readonly ConvertService _convertService;

    private readonly AppDbContext _appDbContext;

    // memória temporária para evitar consultar o banco centenas de vezes
    private readonly Dictionary<string, long> _sapCodeCache = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="SpreadsheetService"/>.
    /// </summary>
    public SpreadsheetService(ConvertService convertService, AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
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

        /// <summary>
        /// Obtém ou define o valor de Itens.
        /// </summary>
        public List<OperacaoModel> Operacoes { get; set; } = new();
    }

    /// <summary>
    /// Executa a ação correspondente.
    /// </summary>
    /// <param name="spreadSheet">Parâmetro do método.</param>
    /// <returns>Resultado da operação.</returns>
    public virtual async Task<ResultadoPlanilha> ReadNotasSpreadSheetAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var resultado = new ResultadoPlanilha();

        var extension = Path.GetExtension(fileName).ToLower();
        Stream csvStream;

        // Usa o Stream direto da memória. Não podemos colocar "using" aqui pois o Stream pertence ao chamador (Orquestrador).
        if (extension == ".xlsx")
        {
            // Usa o Stream direto da memória. Não podemos colocar "using" aqui pois o Stream pertence ao chamador.
            csvStream = await _convertService.ConvertXlsxStreamToCsvMemoryAsync(
                fileStream,
                sheetIndex: 0,
                separator: ';',
                utf8WithBom: true
            );
        }
        else
        {
            csvStream = fileStream;
        }

        csvStream.Position = 0;

        // declara o parser para ler o arquivo csv, usando UTF-8 e detectando a codificação automaticamente,
        // e configurando para usar ; como delimitador, sem campos entre aspas e para trimar os espaços em branco
        string delimitador = ";";
        using (var reader = new StreamReader(csvStream, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            var primeiraLinha = reader.ReadLine() ?? "";

            // se houver mais vírgulas do que ponto-e-vírgulas no cabeçalho, sabemos que é um CSV 
            if (primeiraLinha.Count(c => c == ',') > primeiraLinha.Count(c => c == ';'))
            {
                delimitador = ",";
            }
        }

        // reseta a posição do stream para conseguir ler desde o início
        csvStream.Position = 0;

        CultureInfo culture = CultureInfo.GetCultureInfo("pt-BR");

        //'leaveOpen: true' garante que o parser não destrua o MemoryStream principal prematuramente
        using TextFieldParser parser =
            new TextFieldParser(csvStream, Encoding.UTF8, detectEncoding: true, leaveOpen: true)
            {
                TextFieldType = FieldType.Delimited
            };

        // delimitadro detectado
        parser.SetDelimiters(delimitador);

        // impede quebras 
        parser.HasFieldsEnclosedInQuotes = true;
        parser.TrimWhiteSpace = true;
        string[]? headers = null;
        Dictionary<string, int> columns = new(StringComparer.OrdinalIgnoreCase);

        while (!parser.EndOfData)
        {
            // interrompe o loop se o token for laçado
            cancellationToken.ThrowIfCancellationRequested();

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

            // verifica para colunas da petro
            if (columns.ContainsKey("Nota") || columns.ContainsKey("status_nota") ||
                columns.ContainsKey("Ordem") || columns.ContainsKey("numero_ordem") ||
                columns.ContainsKey("Item") ||
                columns.ContainsKey("Objeto") ||
                columns.ContainsKey("Área operacion.")
               )
            {
                break;
            }
        }

        if (columns.Count == 0) return resultado;

        bool isNota = (columns.ContainsKey("Nota") && columns.ContainsKey("Tipo de nota")) ||
                      (columns.ContainsKey("Nota") && columns.ContainsKey("status_nota"));
        bool isOrdem = columns.ContainsKey("Ordem") && columns.ContainsKey("Texto breve") ||
                       (columns.ContainsKey("numero_ordem") && columns.ContainsKey("status_ordem"));
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
            // interrompe o loop se o token for laçado
            cancellationToken.ThrowIfCancellationRequested();

            string[]? row = parser.ReadFields();
            if (row == null || row.All(string.IsNullOrWhiteSpace)) continue;

            if (resultado.TipoIdentificado == "NOTA")
            {
                var dto = ReadLineForModel<NotaDto>(row, columns);
                resultado.Notas.Add(MapNotaDto(dto));
            }
            else if (resultado.TipoIdentificado == "ORDEM")
            {
                var dto = ReadLineForModel<OrdemDto>(row, columns);
                var ordemModel = MapOrdemDto(dto);

                var dtoAux = ReadLineForModel<OperacaoOrdemDto>(row, columns);
                if (!string.IsNullOrWhiteSpace(dtoAux.Descricao))
                {
                    var operacaoModel = MapOperacaoOrdemDto(dtoAux);

                    // Vincula a referência de memória
                    operacaoModel.Ordem = ordemModel;

                    ordemModel.Operacoes ??= new List<OperacaoModel>();
                    ordemModel.Operacoes.Add(operacaoModel);

                    resultado.Operacoes.Add(operacaoModel);
                }

                resultado.Ordens.Add(ordemModel);
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
        var type = typeof(T);

        //OTIMIZAÇÃO DE REFLECTION
        // Se as propriedades dessa classe ainda não foram lidas, lê e guarda no cache.
        if (!_propertyCache.TryGetValue(type, out var props))
        {
            // Caso não estejam no cache (primeira vez que o sistema vê esta classe), usamos Reflection para ler.
            props = new List<(PropertyInfo, string)>();
            foreach (PropertyInfo property in type.GetProperties())
            {
                var atributo = property.GetCustomAttribute<SheetColumnAttribute>();
                if (atributo != null)
                {
                    props.Add((property, atributo.NomeDaColuna));
                }
            }

            // Guarda o resultado no dicionário estático para uso imediato nas próximas milhares de iterações.
            _propertyCache[type] = props;
        }

        // Iteramos sobre a lista processada armazenada no cache (velocidade máxima), eliminando o uso de GetProperties() aqui.
        foreach (var (property, nomeColunaExcel) in props)
        {
            if (columns.TryGetValue(nomeColunaExcel.ToUpperInvariant(), out var idx) && idx < row.Length)
            {
                string valor = row[idx].Trim();

                if (!string.IsNullOrWhiteSpace(valor))
                {
                    try
                    {
                        Type tipo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        object? valorConvertido = null;

                        if (tipo == typeof(ComponentType) && (string.Equals(property.Name, "IdAtivo",
                                StringComparison.OrdinalIgnoreCase) || string.Equals(property.Name, "Componente",
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            valorConvertido = TryTranslateComponentType(valor);
                        }
                        else if (tipo == typeof(DateTime) && string.Equals(nomeColunaExcel, "data_expectativa_nota",
                                     StringComparison.OrdinalIgnoreCase))
                        {
                            long.TryParse(valor, out long l);
                            DateTimeOffset dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(l);
                            valorConvertido = dataOffset.UtcDateTime;
                        }
                        else if (tipo == typeof(DateTime))
                        {
                            if (DateTime.TryParse(valor, CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None,
                                    out var dt))
                            {
                                valorConvertido = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                            }
                            else
                            {
                                long.TryParse(valor, out long l);
                                DateTimeOffset dataOffset = DateTimeOffset.FromUnixTimeMilliseconds(l);
                                valorConvertido = dataOffset.UtcDateTime;
                            }
                        }
                        else if (string.Equals(nomeColunaExcel, "CondInstApósMed",
                                     StringComparison.OrdinalIgnoreCase) && tipo == typeof(ConfiabilidadeRestaurada) &&
                                 valor.Contains("3"))
                        {
                            valorConvertido = ConfiabilidadeRestaurada.RestauraLI;
                        }
                        else if (string.Equals(nomeColunaExcel, "confiabilidade", StringComparison.OrdinalIgnoreCase) &&
                                 tipo == typeof(ConfiabilidadeRestaurada) && valor == "1")
                        {
                            valorConvertido = ConfiabilidadeRestaurada.RestauraLI;
                        }
                        else if (string.Equals(nomeColunaExcel, "CondInstApósMed",
                                     StringComparison.OrdinalIgnoreCase) && tipo == typeof(ConfiabilidadeRestaurada) &&
                                 valor.Contains("3"))
                        {
                            valorConvertido = ConfiabilidadeRestaurada.RestauraLI;
                        }
                        else if (tipo == typeof(bool))
                        {
                            // Traduz de string para bool
                            valorConvertido = string.Equals(valor, "SIM", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(valor, "S", StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(valor, "TRUE", StringComparison.OrdinalIgnoreCase) ||
                                              valor == "1";
                        }
                        else if (tipo == typeof(StatusFichas) && string.Equals(property.Name, "StatusNota",
                                     StringComparison.OrdinalIgnoreCase))
                        {
                            valorConvertido = TryTranslateStatusNota(valor);
                        }
                        else if (tipo == typeof(StatusFichas) && string.Equals(property.Name, "StatusOrdem",
                                     StringComparison.OrdinalIgnoreCase))
                        {
                            valorConvertido = TryTranslateStatusOrdem(valor);
                        }
                        else if (tipo.IsEnum)
                        {
                            if (Enum.TryParse(tipo, valor, true, out var parsedEnum))
                            {
                                valorConvertido = parsedEnum;
                            }
                            else
                            {
                                string cleaned = NormalizeText(valor);
                                if (Enum.TryParse(tipo, cleaned, true, out var parsedEnumCleaned))
                                {
                                    valorConvertido = parsedEnumCleaned;
                                }
                            }
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
                        else if (tipo == typeof(Classificacao))
                        {
                            valorConvertido = TryTranslateClassificacao(valor);
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

    public long? TryTranslateSapCode(string rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue)) return null;

        // verifica se ja foi consultado o banco para esse código SAP antes
        if (_sapCodeCache.TryGetValue(rawValue, out var cachedIdNota))
        {
            return cachedIdNota;
        }

        var result = _appDbContext.PM_Notas
            .Where(n => n.SapCode == rawValue)
            .Select(n => (long?)n.IdNota)
            .FirstOrDefault();
        if (result.HasValue)
        {
            _sapCodeCache[rawValue] = result.Value;
            return result;
        }

        return null;
    }

    public StatusFichas TryTranslateStatusNota(string rawValue)
    {
        var normalized = NormalizeText(rawValue);
        if (string.IsNullOrEmpty(normalized))
            return StatusFichas.EmAndamento;

        if (normalized.Contains("concluida") || normalized.Contains("msen"))
            return StatusFichas.Concluida;
        if (normalized.Contains("aberta") || normalized.Contains("meab") || normalized.Contains("mspn") ||
            normalized.Contains("mrel"))
            return StatusFichas.Aberta;
        if (normalized.Contains("reprovada") || normalized.Contains("mbar"))
            return StatusFichas.Reprovada;

        return StatusFichas.EmAndamento;
    }

    public StatusFichas TryTranslateStatusOrdem(string rawValue)
    {
        var normalized = NormalizeText(rawValue);
        if (string.IsNullOrEmpty(normalized))
            return StatusFichas.Concluida;

        if (normalized.Contains("concluida")
            || normalized.Contains("exco")
            || normalized.Contains("ftec")
            || normalized.Contains("enco")
            || normalized.Contains("ente")
            || normalized.Contains("ence")
            || normalized.Contains("mrel")
           )
            return StatusFichas.Concluida;
        if (normalized.Contains("aberta")
            || normalized.Contains("impd")
            || normalized.Contains("ppro")
            || normalized.Contains("tria")
            || normalized.Contains("pmau")
            || normalized.Contains("abra")
            || normalized.Contains("lib")
           )
            return StatusFichas.Aberta;
        if (normalized.Contains("reprovada")
            || normalized.Contains("scan")
            || normalized.Contains("excl")
            || normalized.Contains("pmvl")
           )
            return StatusFichas.Reprovada;

        return StatusFichas.EmAndamento;
    }

    private static Classificacao TryTranslateClassificacao(string rawValue)
    {
        var normalized = NormalizeText(rawValue);
        if (string.IsNullOrEmpty(normalized))
            return Classificacao.NAOCLASSIFICADA;

        if (normalized.Contains("corretiva") || normalized.Contains("zf") || normalized.Contains("1"))
            return Classificacao.CORRETIVA;
        if (normalized.Contains("preventiva") || normalized.Contains("0"))
            return Classificacao.PREVENTIVA;
        if (normalized.Contains("zs"))
            return Classificacao.OCORRENCIAALHEIA;

        return Classificacao.NAOCLASSIFICADA;
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

    private static NotaModel MapNotaDto(NotaDto dto)
    {
        // converte string em tipo Prioridade
        return new NotaModel
        {
            NumeroNota = dto.NumeroNota,
            TipoNota = dto.TipoNota,
            Classificacao = dto.IndicadorCorretiva ? Classificacao.CORRETIVA : dto.Classificacao,
            MetodoClassificacao =
                dto.IndicadorCorretiva ? MetodoClassificacao.HUMANA : MetodoClassificacao.NAOCLASSIFICADA,
            StatusNota = StatusFichas.EmAndamento,
            DnsIot = dto.DnsIot,
            DataFim = dto.DataFim,
            DataNota = dto.DataNota,
            Prioridade = dto.Prioridade,
            FimDesejado = dto.FimDesejado,
            DataAvaria = dto.DataAvaria,
            EquipamentoDisponivel = dto.EquipamentoDisponivel,
            AnomaliaObservada = dto.AnomaliaObservada,
            DescricaoBreve = dto.DescricaoBreve,
            DescricaoLonga = dto.DescricaoLonga,
            ObservacoesComplementares = dto.ObservacoesComplementares,
            ReferenciaLocalizacaoAtivo = string.IsNullOrWhiteSpace(dto.ReferenciaLocalizacaoAtivo)
                ? "N/A"
                : dto.ReferenciaLocalizacaoAtivo,
            Finalidade = string.IsNullOrWhiteSpace(dto.Finalidade) ? "N/A" : dto.Finalidade,
            IdPlano = dto.IdPlano,
            IdUsuario = dto.IdUsuario,
            IdsAtivo = dto.IdsAtivo,
            IdLi = dto.IdLi,
            CriadaPorUsuario = dto.CriadaPorUsuario,
            ModificadaPorUsuario = dto.ModificadaPorUsuario,
            DataModificacao = dto.DataModificacao,
            ConfiabilidadeRestaurada = dto.ConfiabilidadeRestaurada
        };
    }

    private static OrdemModel MapOrdemDto(OrdemDto dto)
    {
        return new OrdemModel
        {
            NumeroOrdem = dto.NumeroOrdem,
            TipoOrdem = dto.TipoOrdem,
            Classificacao = dto.Classificacao,
            MetodoClassificacao = MetodoClassificacao.NAOCLASSIFICADA,
            ClassificadoPor = dto.ClassificadoPor,
            DataClassificacao = dto.DataClassificacao,
            StatusOrdem = dto.StatusOrdem ?? StatusFichas.EmAndamento,
            Criticidade = dto.Criticidade,
            DataCriacao = dto.DataCriacao,
            CriadaPorUsuario = dto.CriadaPorUsuario,
            ModificadaPorUsuario = dto.ModificadaPorUsuario,
            DataModificacao = dto.DataModificacao,
            DescricaoBreve = dto.DescricaoBreve,
            DescricaoLonga = dto.DescricaoLonga,
            InicioPlanejado = dto.InicioPlanejado,
            FimPlanejado = dto.FimPlanejado,
            InicioReal = dto.InicioReal,
            FimReal = dto.FimReal,
            IdsAtivo = dto.IdsAtivo,
            IdLi = dto.IdLi,
            Nota = dto.NumeroNota.HasValue ? new NotaModel { NumeroNota = dto.NumeroNota.Value } : null,
            IdNota = null,
            ConfiabilidadeRestaurada = dto.ConfiabilidadeRestaurada ?? ConfiabilidadeRestaurada.NaoRestaura,
            IdGpm = dto.IdGpm,
            DnsIot = dto.DnsIot,
            IdCtm = dto.IdCtm,
            IdPlano = dto.IdPlano,
            Escopo = dto.Escopo
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
            Causa = dto.Causa,
            Descricao = dto.Descricao
        };
    }

    private static OperacaoModel MapOperacaoOrdemDto(OperacaoOrdemDto dto)
    {
        return new OperacaoModel
        {
            IdOperacao = dto.IdOperacao,
            IdOrdem = dto.IdOrdem,
            Nome = dto.Nome,
            Duracao = 24,
            NdeProfissionais = 1,
            Descricao = (dto.Descricao != null && dto.Descricao.Length > 6000)
                ? dto.Descricao.Substring(0, 6000)
                : dto.Descricao,
            InicioEsperado = dto.InicioEsperado,
            FimEsperado = dto.FimEsperado,
            IdCTM = dto.IdCTM,
            IdTipoManutencao = dto.IdTipoManutencao
        };
    }
}