
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Random;

/// <summary>
///     Interface for random number generators.
/// </summary>
/// <remarks>
///     <para>
///         The interface defines set of methods and properties, which should
///         be implemented by different algorithms for random numbers generation.
///     </para>
/// </remarks>
public interface IRandomNumberGenerator<T>
{
    /// <summary>
    ///     Generates a random vector of observations from the current distribution.
    /// </summary>
    /// <param name="samples">The number of samples to generate.</param>
    /// <returns>A random vector of observations drawn from this distribution.</returns>
    T[] Generate(int samples);

    /// <summary>
    ///     Generates a random vector of observations from the current distribution.
    /// </summary>
    /// <param name="samples">The number of samples to generate.</param>
    /// <param name="result">The location where to store the samples.</param>
    /// <returns>A random vector of observations drawn from this distribution.</returns>
    T[] Generate(int samples, T[] result);

    /// <summary>
    ///     Generates a random observation from the current distribution.
    /// </summary>
    /// <returns>A random observations drawn from this distribution.</returns>
    T Generate();
}