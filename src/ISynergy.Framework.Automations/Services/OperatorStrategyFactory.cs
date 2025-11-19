using ISynergy.Framework.Automations.Abstractions;
using ISynergy.Framework.Automations.Enumerations;
using ISynergy.Framework.Automations.Services.Operators;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Automations.Services;

/// <summary>
/// Factory for resolving operator strategies.
/// </summary>
public class OperatorStrategyFactory : IOperatorStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<OperatorTypes, IOperatorStrategy> _strategies;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperatorStrategyFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving strategies.</param>
    public OperatorStrategyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _strategies = new Dictionary<OperatorTypes, IOperatorStrategy>
        {
            { OperatorTypes.And, _serviceProvider.GetRequiredService<AndOperatorStrategy>() },
            { OperatorTypes.Or, _serviceProvider.GetRequiredService<OrOperatorStrategy>() }
        };
    }

    /// <summary>
    /// Gets an operator strategy for the specified operator type.
    /// </summary>
    /// <param name="operatorType">The operator type.</param>
    /// <returns>An operator strategy for the specified type.</returns>
    public IOperatorStrategy GetStrategy(OperatorTypes operatorType)
    {
        if (_strategies.TryGetValue(operatorType, out var strategy))
        {
            return strategy;
        }

        throw new NotSupportedException($"Operator type {operatorType} is not supported.");
    }
}

