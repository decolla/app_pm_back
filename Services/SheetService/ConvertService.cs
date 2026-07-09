using System.Buffers;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using ExcelDataReader;

namespace api.Communications.Service.Impl.Data;

/// <summary>
/// Classe ConvertService.
/// </summary>
public class ConvertService
{
    // armazena os valores de carácteres especiais 
    private static readonly SearchValues<char> SpecialChars = SearchValues.Create(new[] { ';', '"', '\n', '\r' });

    /// <summary>
    /// Converte xlsx para um arquivo csv.
    /// </summary>
    /// <param name="xlsxStream">Stream xlsx.</param>
    /// <param name="csvPath">Caminho do csv.</param>
    public Task ConvertXlsxStreamToCsvFileAsync(
        Stream xlsxStream,
        string csvPath,
        int sheetIndex = 0,
        char separator = ';',
        bool utf8WithBom = true,
        bool includeHeader = true,
        int maxDegreeOfParallelism = -1,
        CancellationToken cancellationToken = default
    )
    {
        var encoding = utf8WithBom ? new UTF8Encoding(true) : new UTF8Encoding(false);

        var fileStream = new FileStream(
            csvPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 262144,
            useAsync: true);

        return ConvertXlsxStreamToCsvStreamAsync(
            xlsxStream,
            fileStream,
            encoding,
            sheetIndex,
            separator,
            includeHeader,
            maxDegreeOfParallelism,
            leaveOpen: false,
            cancellationToken: cancellationToken
            );
    }

    /// <summary>
    /// Converte xlsx para csv stream.
    /// </summary>
    /// <param name="xlsxStream">xlsx stream.</param>
    /// <param name="outputStream">Objeto de saída em stream csv.</param>
    /// <param name="encoding">Parâmetro do método.</param>
    public async Task ConvertXlsxStreamToCsvStreamAsync(
        Stream xlsxStream,
        Stream outputStream,
        Encoding encoding,
        int sheetIndex = 0,
        char separator = ';',
        bool includeHeader = true,
        int maxDegreeOfParallelism = -1,
        bool leaveOpen = true,
        CancellationToken cancellationToken = default)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var reader = ExcelReaderFactory.CreateReader(xlsxStream);

        for (int i = 0; i < sheetIndex; i++)
        {
            if (!reader.NextResult())
                throw new ArgumentException($"Planilha {sheetIndex} não encontrada.");
        }

        var rows = new List<object[]>();
        int fieldCount = 0;

        bool isFirstRow = true;
        while (reader.Read())
        {
            if (isFirstRow)
            {
                fieldCount = reader.FieldCount;
                if (!includeHeader)
                {
                    isFirstRow = false;
                    continue;
                }

                isFirstRow = false;
            }

            var row = new object[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }

            rows.Add(row);
        }

        if (rows.Count == 0)
        {
            // garante que o stream fique vazio
            return;
        }

        var lines = new ConcurrentBag<(int index, string line)>();
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism > 0
                ? maxDegreeOfParallelism
                : Environment.ProcessorCount
        };

        var partitioner = Partitioner.Create(0, rows.Count,
            Math.Max(rows.Count / (options.MaxDegreeOfParallelism * 4), 50));

        Parallel.ForEach(partitioner, options, range =>
        {
            var sb = new StringBuilder(fieldCount * 30);

            for (int rowIndex = range.Item1; rowIndex < range.Item2; rowIndex++)
            {
                sb.Clear();
                var row = rows[rowIndex];

                for (int i = 0; i < fieldCount; i++)
                {
                    if (i > 0) sb.Append(separator);

                    var value = row[i];
                    string text = value switch
                    {
                        double d => d.ToString(CultureInfo.InvariantCulture),
                        float f => f.ToString(CultureInfo.InvariantCulture),
                        int n => n.ToString(CultureInfo.InvariantCulture),
                        long l => l.ToString(CultureInfo.InvariantCulture),
                        DateTime dt => dt.ToString("o"),
                        bool b => b ? "True" : "False",
                        null => string.Empty,
                        _ => value.ToString() ?? string.Empty
                    };

                    AppendEscapedFast(sb, text, separator);
                }

                lines.Add((rowIndex, sb.ToString()));
            }
        });

        using var writer = new StreamWriter(outputStream, encoding, bufferSize: 262144, leaveOpen: leaveOpen);

        var orderedLines = lines.OrderBy(x => x.index).Select(x => x.line);

        foreach (var line in orderedLines)
        {
            await writer.WriteLineAsync(line);
        }

        await writer.FlushAsync();
    }

    /// <summary>
    /// Converte xlsx para csv em memória.
    /// </summary>
    /// <param name="xlsxStream">Stream xlsx.</param>
    /// <returns>Resultado da operação.</returns>
    public async Task<MemoryStream> ConvertXlsxStreamToCsvMemoryAsync(
        Stream xlsxStream,
        int sheetIndex = 0,
        char separator = ';',
        bool utf8WithBom = true,
        bool includeHeader = true,
        int maxDegreeOfParallelism = -1)
    {
        var encoding = utf8WithBom ? new UTF8Encoding(true) : new UTF8Encoding(false);
        var ms = new MemoryStream();

        await ConvertXlsxStreamToCsvStreamAsync(
            xlsxStream,
            ms,
            encoding,
            sheetIndex,
            separator,
            includeHeader,
            maxDegreeOfParallelism,
            leaveOpen: true);

        ms.Position = 0;
        return ms;
    }

    private static void AppendEscapedFast(StringBuilder sb, ReadOnlySpan<char> field, char sep)
    {
        if (field.IsEmpty)
            return;

        bool mustQuote = field.ContainsAny(SpecialChars) || field.IndexOf(sep) >= 0;

        if (!mustQuote)
        {
            sb.Append(field);
            return;
        }

        sb.Append('"');

        foreach (char c in field)
        {
            if (c == '"')
                sb.Append("\"\"");
            else
                sb.Append(c);
        }

        sb.Append('"');
    }
}