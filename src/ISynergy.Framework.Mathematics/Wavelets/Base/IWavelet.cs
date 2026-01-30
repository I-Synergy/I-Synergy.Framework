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
