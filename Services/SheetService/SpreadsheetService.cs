using System.Globalization;
using System.Reflection;
using System.Text;
using api.Communications.Service.Impl.Data;
using app_pm_back.Attributes;
using app_pm_back.DTO.ComgasDTO;
using app_pm_back.Models;
using app_pm_back.Models.CongasModel;
using app_pm_back.Models.Domain;
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
        public List<NotaManutencaoModel> NotasCongas { get; set; } = new();
        public List<OrdemPetroModel> OrdensPetro { get; set; } = new();
        public List<OrdemManutencaoModel> OrdensCongas { get; set; } = new();
        public List<ItemNotaManutencaoModel> ItensCongas { get; set; } = new();
        public List<EquipamentoAtivoModel> Equipamentos { get; set; } = new();
        public List<AcaoNotaCongasModel> AcoesCongas { get; set; } = new();
    }

    public async Task<ResultadoPlanilha> ReadNotasSpreadSheetAsync(IFormFile spreadSheet)
    {
        var resultado = new ResultadoPlanilha();

        var extension = Path.GetExtension(spreadSheet.FileName).ToLower();
        Stream csvStream;

        // formato de conversão de Excel para CSV
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

        // padrão pt-BR para ler datas como DD/MM/AAAA
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

            if (columns.ContainsKey("Campo seleção") ||
                columns.ContainsKey("Campo selecao") ||
                columns.ContainsKey("Nota") ||
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

        // DETETIVE
        Console.WriteLine("-----CABEÇALHOS DETECTADOS-----");
        Console.WriteLine(string.Join(" | ", columns.Keys));
        Console.WriteLine("-------------------------------\n");

        // A MÁGICA DA IDENTIFICAÇÃO
        bool isNotaPetro = columns.ContainsKey("Campo seleção") || columns.ContainsKey("Campo selecao");
        bool isNotaCongas = columns.ContainsKey("Nota") && columns.ContainsKey("Tipo de nota");
        bool isOrdemCongas = columns.ContainsKey("Ordem") && columns.ContainsKey("Texto breve");
        bool isItemCongas = columns.ContainsKey("Item") && columns.ContainsKey("Causa") &&
                            columns.ContainsKey("Texto da causa");
        bool isAcaoNotaCongas = columns.ContainsKey("Classe defeitos") && columns.ContainsKey("Ordem cliente");
        bool isEquipamento = columns.ContainsKey("Objeto") && columns.ContainsKey("GRID");

        if (isOrdemCongas)
        {
            resultado.TipoIdentificado = "ORDEM_CONGAS";
        }
        else if (isAcaoNotaCongas)
        {
            resultado.TipoIdentificado = "ACAO_NOTA_CONGAS";
        }
        else if (isItemCongas)
        {
            resultado.TipoIdentificado = "ITEM_CONGAS";
        }
        else if (isNotaPetro)
        {
            resultado.TipoIdentificado = "NOTA_PETRO";
        }
        else if (isNotaCongas)
        {
            resultado.TipoIdentificado = "NOTA_CONGAS";
        }
        else if (isEquipamento)
        {
            resultado.TipoIdentificado = "EQUIPAMENTO_SAP";
        }
        else
        {
            return resultado;
        }

        while (!parser.EndOfData)
        {
            string[] row = parser.ReadFields();
            if (row == null || row.All(string.IsNullOrWhiteSpace)) continue;

            if (resultado.TipoIdentificado == "NOTA_PETRO")
            {
                var notaPetro = ReadLineForModel<NotaPetroModel>(row, columns);
                resultado.NotasPetro.Add(notaPetro);
            }
            else if (resultado.TipoIdentificado == "NOTA_CONGAS")
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
            else if (resultado.TipoIdentificado == "ACAO_NOTA_CONGAS")
            {
                var dto = ReadLineForModel<AcaoNotaCongasComgasDto>(row, columns);
                resultado.AcoesCongas.Add(MapAcaoNotaComgasDto(dto));
            }
            else if (resultado.TipoIdentificado == "EQUIPAMENTO_SAP")
            {
                var dto = ReadLineForModel<EquipamentoAtivoComgasDto>(row, columns);
                var equipamento = MapEquipamentoComgasDto(dto);

                PreencherDetalhesTecnicos(equipamento, row, columns);

                if (!string.IsNullOrWhiteSpace(equipamento.TagEquipamento))
                {
                    resultado.Equipamentos.Add(equipamento);
                }
            }
        }

        return resultado;
    }

    public T ReadLineForModel<T>(string[] row, Dictionary<string, int> columns) where T : class, new()
    {
        // objeto vazio para receber os dados
        var model = new T();

        foreach (PropertyInfo property in model.GetType().GetProperties())
        {
            // procura-se a propriedade com a etiqueta 
            var atributo = property.GetCustomAttribute<SheetColumnAttribute>();

            if (atributo != null)
            {
                // pega o nome da coluna do Excel
                string nomeColunaExcel = atributo.NomeDaColuna;

                // procura-se a coluna no CSV
                if (columns.TryGetValue(nomeColunaExcel.ToUpperInvariant(), out var idx) && idx < row.Length)
                {
                    // pega o valor da coluna
                    string valor = row[idx].Trim();

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        try
                        {
                            // converte o valor para o tipo da propriedade, verificando se é nullable
                            Type tipo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object? valorConvertido = null;

                            // 1. Limpar caracteres não numéricos de campos long/int (como 'QM0000...')
                            if (tipo == typeof(long) || tipo == typeof(int))
                            {
                                string numerosApenas = new string(valor.Where(char.IsDigit).ToArray());
                                if (!string.IsNullOrEmpty(numerosApenas))
                                {
                                    valorConvertido = Convert.ChangeType(numerosApenas, tipo);
                                }
                            }
                            // 2. Tratar TimeSpan (Horas)
                            else if (tipo == typeof(TimeSpan))
                            {
                                if (valor == "24:00:00") valor = "23:59:59";
                                if (TimeSpan.TryParse(valor, out var ts))
                                {
                                    valorConvertido = ts;
                                }
                            }
                            // 3. Tratar DateTime (Datas e o lixo "CG01", "PM-MECAN", etc)
                            else if (tipo == typeof(DateTime))
                            {
                                if (DateTime.TryParse(valor, CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None,
                                        out var dt))
                                {
                                    valorConvertido = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                                }
                            }
                            // 4. Conversão padrão para strings e outros tipos
                            else
                            {
                                valorConvertido = Convert.ChangeType(valor, tipo);
                            }

                            // 5. Atribuir o valor se a conversão funcionou
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

    private static NotaManutencaoModel MapNotaComgasDto(NotaManutencaoComgasDto dto)
    {
        return new NotaManutencaoModel
        {
            IdNota = dto.IdNota != 0 ? dto.IdNota : dto.NumeroNota,
            NumeroNota = dto.NumeroNota,
            TipoNota = dto.TipoNota,
            Classificacao = dto.Classificacao,
            MetodoClassificacao = dto.MetodoClassificacao,
            ClassificadoPor = dto.ClassificadoPor,
            DataClassificacao = dto.DataClassificacao,
            Descricao = dto.Descricao,
            DescricaoDetalhada = dto.DescricaoDetalhada,
            StatusNota = dto.StatusNota,
            DataNota = dto.DataNota,
            Prioridade = dto.Prioridade,
            InicioDesejado = dto.InicioDesejado,
            ConclusaoDesejada = dto.ConclusaoDesejada,
            EquipamentoTag = dto.EquipamentoTag,
            LocalInstalacaoCodigo = dto.LocalInstalacaoCodigo,
            CriadoPorUsuarioId = dto.CriadoPorUsuarioId,
            TipoPrioridade = dto.TipoPrioridade,
            TipoDeNota = dto.TipoDeNota,
            TxtGrpCodific = dto.TxtGrpCodific,
            TxtCodeCodif = dto.TxtCodeCodif,
            Codificacao = dto.Codificacao,
            Consequencia = dto.Consequencia,
            GrupoDeCodes = dto.GrupoDeCodes
        };
    }

    private static OrdemManutencaoModel MapOrdemComgasDto(OrdemManutencaoComgasDto dto)
    {
        return new OrdemManutencaoModel
        {
            IdOrdem = dto.IdOrdem,
            TipoOrdem = dto.TipoOrdem,
            Classificacao = dto.Classificacao,
            MetodoClassificacao = dto.MetodoClassificacao,
            ClassificadoPor = dto.ClassificadoPor,
            DataClassificacao = dto.DataClassificacao,
            StatusOrdem = dto.StatusOrdem,
            NotaRelacionadaId = dto.NotaRelacionadaId,
            PlanoManutencaoCodigo = dto.PlanoManutencaoCodigo,
            Prioridade = dto.Prioridade,
            DataEntrada = dto.DataEntrada,
            CriadaPorUsuarioId = dto.CriadaPorUsuarioId,
            ModificadaPorUsuarioId = dto.ModificadaPorUsuarioId,
            CustoEstimado = dto.CustoEstimado,
            CustoPlanejado = dto.CustoPlanejado,
            CustoReal = dto.CustoReal
        };
    }

    private static ItemNotaManutencaoModel MapItemNotaComgasDto(ItemNotaManutencaoComgasDto dto)
    {
        return new ItemNotaManutencaoModel
        {
            IdItemNota = dto.IdItemNota,
            NotaId = dto.NotaId,
            ParteObjeto = dto.ParteObjeto,
            Sintoma = dto.Sintoma,
            TextoSintoma = dto.TextoSintoma,
            Causa = dto.Causa,
            TextoCausa = dto.TextoCausa,
            NDeOrdenacao1 = dto.NDeOrdenacao1,
            TxtGrpCodPartOb = dto.TxtGrpCodPartOb,
            TxtCodPartOb = dto.TxtCodPartOb,
            TxtCodeProb = dto.TxtCodeProb,
            TextoGrpCausa = dto.TextoGrpCausa,
            TxtGrpCodPr = dto.TxtGrpCodPr,
            TxtCodeCausa = dto.TxtCodeCausa
        };
    }

    private static AcaoNotaCongasModel MapAcaoNotaComgasDto(AcaoNotaCongasComgasDto dto)
    {
        return new AcaoNotaCongasModel
        {
            AreaOperac = dto.AreaOperac,
            Atividade = dto.Atividade,
            TpCatalogo = dto.TpCatalogo,
            Bairro = dto.Bairro,
            CanalDistrib = dto.CanalDistrib,
            CentroCusto = dto.CentroCusto,
            CenLocaliz = dto.CenLocaliz,
            CenPCenTrab = dto.CenPCenTrab,
            CentroPlanej = dto.CentroPlanej,
            CenTrabRespon = dto.CenTrabRespon,
            Pais = dto.Pais,
            ClasseDefeitos = dto.ClasseDefeitos,
            Cliente = dto.Cliente,
            Codificacao = dto.Codificacao,
            CodeParteObj = dto.CodeParteObj,
            CodigoDeAcao = dto.CodigoDeAcao,
            CodigoDoDano = dto.CodigoDoDano,
            ConclDesejada = dto.ConclDesejada,
            DataDaNota = dto.DataDaNota,
            Descricao = dto.Descricao,
            Equipamento = dto.Equipamento,
            LocInstalacao = dto.LocInstalacao,
            Nota = dto.Nota,
            Ordem = dto.Ordem,
            Texto = dto.Texto,
            TipoDeNota = dto.TipoDeNota,
            TipoPrioridade = dto.TipoPrioridade
        };
    }

    private static EquipamentoAtivoModel MapEquipamentoComgasDto(EquipamentoAtivoComgasDto dto)
    {
        return new EquipamentoAtivoModel
        {
            TagEquipamento = dto.TagEquipamento ?? string.Empty,
            Tipo = dto.Tipo,
            Denominacao = dto.Denominacao,
            Status = dto.Status,
            ValidoDesde = dto.ValidoDesde,
            ValidoAte = dto.ValidoAte,
            Classe = dto.Classe,
            TipoObjeto = dto.TipoObjeto,
            GrupoAutorizacoes = dto.GrupoAutorizacoes,
            Peso = dto.Peso,
            NumeroInventario = dto.NumeroInventario,
            TipoNotaPadrao = dto.TipoNotaPadrao,
            ValorAquisicao = dto.ValorAquisicao,
            DataAquisicao = dto.DataAquisicao,
            FabricanteId = dto.FabricanteId,
            LocalizacaoId = dto.LocalizacaoId,
            EnderecoId = dto.EnderecoId,
            ImobilizadoTag = dto.ImobilizadoTag,
            GpmId = dto.GpmId,
            CentroTrabalhoId = dto.CentroTrabalhoId,
            LocalInstalacaoCodigo = dto.LocalInstalacaoCodigo
        };
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

    /// <summary>
    /// Lê as colunas extras que não estão mapeadas com [SheetColumn] e atira-as para o JSONB
    /// </summary>
    public void PreencherDetalhesTecnicos(EquipamentoAtivoModel equipamento, string[] row, Dictionary<string, int> columns)
    {
        // 1. Descobrir quais são os nomes das colunas mapeadas de forma estática
        var colunasMapeadas = typeof(EquipamentoAtivoModel).GetProperties()
            .Select(p => p.GetCustomAttribute<SheetColumnAttribute>()?.NomeDaColuna.ToUpperInvariant())
            .Where(nome => nome != null)
            .ToHashSet();

        // 2. Percorrer todas as colunas que vieram do Excel
        foreach (var col in columns)
        {
            string nomeColuna = col.Key.ToUpperInvariant();
            int idx = col.Value;

            // 3. Se a coluna NÃO ESTÁ no modelo fixo e tem valor, vai para o JSON!
            if (!colunasMapeadas.Contains(nomeColuna) && idx < row.Length)
            {
                string valor = row[idx].Trim();
                if (!string.IsNullOrWhiteSpace(valor))
                {
                    // Guarda a chave com o nome original do cabeçalho do Excel
                    equipamento.DetalhesTecnicos[col.Key] = valor;
                }
            }
        }
    }
}