
#pragma warning disable S1244 // float equality is intentional in numerical algorithms
#pragma warning disable S3776 // cognitive complexity is inherent in numerical algorithms
#pragma warning disable S2368 // object overloads are part of the library API
#pragma warning disable S1905 // casts may be intentional for type clarity
#pragma warning disable S1199 // nested blocks required in algorithm implementation

namespace ISynergy.Framework.Mathematics.Optimization;

/// <summary>
///   Common interface for specifying objective functions.
/// </summary>
/// 
public interface IObjectiveFunction
{

    /// <summary>
    ///   Gets input variable's labels for the function.
    /// </summary>
    /// 
    IDictionary<string, int> Variables { get; }

    /// <summary>
    ///   Gets the index of each input variable in the function.
    /// </summary>
    /// 
    IDictionary<int, string> Indices { get; }

    /// <summary>
    ///   Gets the number of input variables for the function.
    /// </summary>
    /// 
    int NumberOfVariables { get; }

    /// <summary>
    ///   Gets the objective function.
    /// </summary>
    /// 
    Func<double[], double> Function { get; }

}
