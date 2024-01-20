

using System;
using System.Collections.Generic;
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
