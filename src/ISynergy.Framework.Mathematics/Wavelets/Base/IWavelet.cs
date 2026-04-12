
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Wavelets.Base;

/// <summary>
///   Common interface for wavelets algorithms.
/// </summary>
/// 
public interface IWavelet
{
    /// <summary>
    ///   1-D Forward Discrete Wavelet Transform.
    /// </summary>
    /// 
    void Forward(double[] data);

    /// <summary>
    ///   2-D Forward Discrete Wavelet Transform.
    /// </summary>
    /// 
    void Forward(double[,] data);

    /// <summary>
    ///   1-D Backward (Inverse) Discrete Wavelet Transform.
    /// </summary>
    /// 
    void Backward(double[] data);

    /// <summary>
    ///   2-D Backward (Inverse) Discrete Wavelet Transform.
    /// </summary>
    /// 
    void Backward(double[,] data);
}
