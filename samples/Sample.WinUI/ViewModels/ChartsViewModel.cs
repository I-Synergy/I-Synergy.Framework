using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.ViewModels;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.ViewModels;

public class ChartsViewModel : ViewModelNavigation<TestItem>
{
    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    public override string Title { get { return LanguageService.Default.GetString("Converters"); } }

    /// <summary>
    /// Gets or sets the categories with least demand property value.
    /// </summary>
    public List<Measurement> CategoriesWithLeastDemand
    {
        get => GetValue<List<Measurement>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the OperationBudget property value.
    /// </summary>
    public List<Measurement> OperationBudget
    {
        get => GetValue<List<Measurement>>();
        set => SetValue(value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectionTestViewModel"/> class.
    /// </summary>
    /// <param name="commonServices">The common services.</param>
    /// <param name="logger"></param>
    public ChartsViewModel(ICommonServices commonServices, ILogger<ChartsViewModel> logger)
        : base(commonServices, logger)
    {
        CategoriesWithLeastDemand =
        [
            new Measurement(Guid.NewGuid(), "Category 5", 40),
            new Measurement(Guid.NewGuid(), "Category 4", 55),
            new Measurement(Guid.NewGuid(), "Category 3", 60),
            new Measurement(Guid.NewGuid(), "Category 2", 75),
            new Measurement(Guid.NewGuid(), "Category 1", 100)
        ];

        OperationBudget = BuildOperationBudget(DateTimeOffset.Parse("2021-01-01"), 12, 1250, 1500);
    }

    private List<Measurement> BuildOperationBudget(DateTimeOffset startDate, int totalMonths, double earnings, double expenses)
    {
        List<Measurement> result = [];

        double buffer = 0d;

        for (int i = 0; i < totalMonths; i++)
        {
            buffer = (earnings + buffer) - expenses;

            result.Add(
                new Measurement(
                    startDate.AddMonths(i * 1).ToString("MM-yyyy"),
                    buffer));
        }

        return result;
    }
}
