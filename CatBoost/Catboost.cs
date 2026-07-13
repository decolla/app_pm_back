using System.Runtime.InteropServices;

namespace teste_catboost_cs;

/// <summary>
/// Classe que encapsula o uso do modelo CatBoost via FFI.
/// Gerencia o ciclo de vida do modelo e fornece métodos para predição.
/// </summary>
public class Cbm : IDisposable
{
    private readonly IntPtr _modelHandle;

    /// <summary>
    /// Inicializa o modelo CatBoost a partir de um arquivo.
    /// </summary>
    /// <param name="modelPath">Caminho para o arquivo do modelo.</param>
    /// <exception cref="ExternalException">Lançada se o modelo não puder ser carregado.</exception>
    public Cbm(string modelPath)
    {
        _modelHandle = FFI.ModelCalcerCreate();
        if (_modelHandle == IntPtr.Zero)
            throw new ExternalException("Failed to create model handle: " + FFI.GetErrorString());

        if (!FFI.LoadFullModelFromFile(_modelHandle, modelPath))
            throw new ExternalException("Failed to load model from file: " + FFI.GetErrorString());
    }

    public ulong DimensionsCount => FFI.GetDimensionsCount(_modelHandle);
    public ulong FloatFeaturesCount => FFI.GetFloatFeaturesCount(_modelHandle);
    public ulong CatFeaturesCount => FFI.GetCatFeaturesCount(_modelHandle);
    public ulong TextFeaturesCount => FFI.GetTextFeaturesCount(_modelHandle);

    public void Dispose()
    {
        FFI.ModelCalcerDelete(_modelHandle);
    }

    /// <summary>
    /// Realiza a predição do modelo usando features float e categóricas.
    /// </summary>
    /// <param name="floatFeatures">Matriz de features float.</param>
    /// <param name="catFeatures">Matriz de features categóricas.</param>
    /// <returns>Array de predições (após aplicação da função sigmoide).</returns>
    /// <exception cref="ExternalException">Lançada se a predição falhar.</exception>
    public double[] Predict(float[][] floatFeatures, string[][] catFeatures)
    {
        ulong
            floatFeaturesSize = (ulong)floatFeatures.FirstOrDefault([]).Length,
            catFeaturesSize = (ulong)catFeatures.FirstOrDefault([]).Length,
            objCount = (ulong)Math.Max(catFeatures.Length, floatFeatures.Length),
            resultSize = DimensionsCount * objCount;
        var result = new double[resultSize];
        if (catFeaturesSize != CatFeaturesCount)
            Console.WriteLine("Warning: catFeaturesSize does not match model's CatFeaturesCount");
        if (floatFeaturesSize != FloatFeaturesCount)
            Console.WriteLine("Warning: floatFeaturesSize does not match model's FloatFeaturesCount");

        IntPtr[]?
            floatFeaturesPtrs = null,
            catFeaturesPtrs = null;

        try
        {
            floatFeaturesPtrs = FFI.MarshalNestedArray(floatFeatures);
            catFeaturesPtrs = FFI.MarshalNestedArray(catFeatures);

            if (!FFI.CalcModelPrediction(
                    _modelHandle,
                    objCount,
                    floatFeaturesPtrs, floatFeaturesSize,
                    catFeaturesPtrs, catFeaturesSize,
                    result, resultSize
                ))
            {
                throw new ExternalException("Failed to calculate model prediction: " + FFI.GetErrorString());
            }

            return result.Select(Sigmoid).ToArray();
        }
        finally
        {
            FFI.FreeNestedArray(floatFeaturesPtrs);
            FFI.FreeNestedStringArray(catFeaturesPtrs, catFeatures.Select(s => s.Length).ToArray());
        }
    }

    /// <summary>
    /// Realiza a predição do modelo usando features float, categóricas e de texto.
    /// </summary>
    /// <param name="floatFeatures">Matriz de features float.</param>
    /// <param name="catFeatures">Matriz de features categóricas.</param>
    /// <param name="textFeatures">Matriz de features de texto.</param>
    /// <returns>Array de predições (após aplicação da função sigmoide).</returns>
    /// <exception cref="ExternalException">Lançada se a predição falhar.</exception>
    public double[] Predict(float[][] floatFeatures, string[][] catFeatures, string[][] textFeatures)
    {
        ulong
            floatFeaturesSize = (ulong)floatFeatures.FirstOrDefault([]).Length,
            catFeaturesSize = (ulong)catFeatures.FirstOrDefault([]).Length,
            textFeaturesSize = (ulong)textFeatures.FirstOrDefault([]).Length,
            objCount = (ulong)Math.Max(catFeatures.Length, Math.Max(textFeatures.Length, floatFeatures.Length)),
            resultSize = DimensionsCount * objCount;
        var result = new double[resultSize];
        if (catFeaturesSize != CatFeaturesCount)
            Console.WriteLine("Warning: catFeaturesSize does not match model's CatFeaturesCount");
        if (textFeaturesSize != TextFeaturesCount)
            Console.WriteLine("Warning: textFeaturesSize does not match model's TextFeaturesCount");
        if (floatFeaturesSize != FloatFeaturesCount)
            Console.WriteLine("Warning: floatFeaturesSize does not match model's FloatFeaturesCount");

        IntPtr[]?
            floatFeaturesPtrs = null,
            catFeaturesPtrs = null,
            textFeaturesPtrs = null;

        try
        {
            floatFeaturesPtrs = FFI.MarshalNestedArray(floatFeatures);
            catFeaturesPtrs = FFI.MarshalNestedArray(catFeatures);
            textFeaturesPtrs = FFI.MarshalNestedArray(textFeatures);

            if (!FFI.CalcModelPredictionText(
                    _modelHandle,
                    objCount,
                    floatFeaturesPtrs, floatFeaturesSize,
                    catFeaturesPtrs, catFeaturesSize,
                    textFeaturesPtrs, textFeaturesSize,
                    result, resultSize
                ))
            {
                throw new ExternalException("Failed to calculate model prediction: " + FFI.GetErrorString());
            }

            return result.Select(Sigmoid).ToArray();
        }
        finally
        {
            FFI.FreeNestedArray(floatFeaturesPtrs);
            FFI.FreeNestedStringArray(catFeaturesPtrs, catFeatures.Select(s => s.Length).ToArray());
            FFI.FreeNestedStringArray(textFeaturesPtrs, textFeatures.Select(s => s.Length).ToArray());
        }
    }

    public static double Sigmoid(double x)
    {
        return 1.0 / (1.0 + Math.Exp(-x));
    }
}

/// <summary>
/// Classe interna para interoperação com a biblioteca nativa CatBoost.
/// </summary>
internal static partial class FFI
{
    [LibraryImport("catboostmodel", EntryPoint = "ModelCalcerCreate")]
    public static partial IntPtr ModelCalcerCreate();

    [LibraryImport("catboostmodel", EntryPoint = "ModelCalcerDelete")]
    public static partial void ModelCalcerDelete(IntPtr modelHandle);

    [LibraryImport("catboostmodel", EntryPoint = "GetErrorString")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static partial string GetErrorString();

    [LibraryImport("catboostmodel", EntryPoint = "LoadFullModelFromFile")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool LoadFullModelFromFile(IntPtr modelHandle, [MarshalAs(UnmanagedType.LPStr)] string fileName);

    [LibraryImport("catboostmodel", EntryPoint = "CalcModelPredictionText")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool CalcModelPredictionText(
        IntPtr modelHandle,
        ulong docCount,
        [In] IntPtr[] floatFeatures, ulong floatFeaturesSize,
        [In] IntPtr[] catFeatures, ulong catFeaturesSize,
        [In] IntPtr[] textFeatures, ulong textFeaturesSize,
        [Out] double[] result, ulong resultSize
    );

    [LibraryImport("catboostmodel", EntryPoint = "CalcModelPrediction")]
    [return: MarshalAs(UnmanagedType.I1)]
    public static partial bool CalcModelPrediction(
        IntPtr modelHandle,
        ulong docCount,
        [In] IntPtr[] floatFeatures, ulong floatFeaturesSize,
        [In] IntPtr[] catFeatures, ulong catFeaturesSize,
        [Out] double[] result, ulong resultSize
    );

    [LibraryImport("catboostmodel", EntryPoint = "GetDimensionsCount")]
    public static partial ulong GetDimensionsCount(IntPtr modelHandle);

    [LibraryImport("catboostmodel", EntryPoint = "GetFloatFeaturesCount")]
    public static partial ulong GetFloatFeaturesCount(IntPtr modelHandle);

    [LibraryImport("catboostmodel", EntryPoint = "GetCatFeaturesCount")]
    public static partial ulong GetCatFeaturesCount(IntPtr modelHandle);

    [LibraryImport("catboostmodel", EntryPoint = "GetTextFeaturesCount")]
    public static partial ulong GetTextFeaturesCount(IntPtr modelHandle);

    public static IntPtr[] MarshalNestedArray(float[][] array)
    {
        var subArrays = new IntPtr[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            subArrays[i] = Marshal.AllocHGlobal(array[i].Length * sizeof(float));
            Marshal.Copy(array[i], 0, subArrays[i], array[i].Length);
        }

        return subArrays;
    }

    public static IntPtr[] MarshalNestedArray(string[][] array)
    {
        var subArrays = new IntPtr[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            var stringPtrs = new IntPtr[array[i].Length];
            for (var j = 0; j < array[i].Length; j++)
                stringPtrs[j] = Marshal.StringToHGlobalAnsi(array[i][j]);

            subArrays[i] = Marshal.AllocHGlobal(stringPtrs.Length * IntPtr.Size);
            Marshal.Copy(stringPtrs, 0, subArrays[i], stringPtrs.Length);
        }

        return subArrays;
    }

    public static void FreeNestedArray(IntPtr[]? nestedArray)
    {
        if (nestedArray == null)
            return;
        foreach (var array in nestedArray)
            Marshal.FreeHGlobal(array);
    }

    public static void FreeNestedStringArray(IntPtr[]? nestedArray, int[] innerArraysLengths)
    {
        if (nestedArray == null)
            return;

        for (var i = 0; i < nestedArray.Length; i++)
        {
            var innerArrayPtr = nestedArray[i];
            var innerLength = innerArraysLengths[i];

            if (innerLength > 0)
            {
                var stringPtrs = new IntPtr[innerLength];
                Marshal.Copy(innerArrayPtr, stringPtrs, 0, innerLength);
                foreach (var ptr in stringPtrs)
                    Marshal.FreeHGlobal(ptr);
            }
            Marshal.FreeHGlobal(innerArrayPtr);
        }
    }
}
