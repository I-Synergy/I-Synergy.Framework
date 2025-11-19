using ISynergy.Framework.AspNetCore.Tests.Models;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ISynergy.Framework.AspNetCore.Tests.StepDefinitions;

/// <summary>
/// Step definitions for Result pattern API responses.
/// Demonstrates BDD testing for ASP.NET Core Result pattern integration.
/// </summary>
[Binding]
public class ResultPatternResponsesSteps : ControllerBase
{
    private readonly ILogger<ResultPatternResponsesSteps> _logger;
    private Result<Product>? _result;
    private IActionResult? _actionResult;
    private List<Result<Product>> _resultChain = new();

    public ResultPatternResponsesSteps()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        _logger = loggerFactory.CreateLogger<ResultPatternResponsesSteps>();
    }

    [Given(@"I have an API controller using Result pattern")]
    public void GivenIHaveAnApiControllerUsingResultPattern()
    {
        _logger.LogInformation("API controller with Result pattern initialized");
        // Controller base is already available through inheritance
    }

    [Given(@"I have a successful result with product data")]
    public void GivenIHaveASuccessfulResultWithProductData()
    {
        _logger.LogInformation("Creating successful result with product data");
        _result = Result<Product>.Success(new Product { ProductId = Guid.NewGuid(), Name = "Test Product", Price = 99.99m });
    }

    [Given(@"I have a successful result without data")]
    public void GivenIHaveASuccessfulResultWithoutData()
    {
        _logger.LogInformation("Creating successful result without data");
        _result = Result<Product>.Success(default(Product)!);
    }

    [Given(@"I have a failed result")]
    public void GivenIHaveAFailedResult()
    {
        _logger.LogInformation("Creating failed result");
        _result = Result<Product>.Fail();
    }

    [Given(@"I have a result with error messages")]
    public void GivenIHaveAResultWithErrorMessages()
    {
        _logger.LogInformation("Creating result with error messages");
        _result = Result<Product>.Fail(["Product not found", "Invalid product ID"]);
    }

    [Given(@"I have multiple result operations")]
    public void GivenIHaveMultipleResultOperations()
    {
        _logger.LogInformation("Creating multiple result operations");
        _resultChain.Add(Result<Product>.Success(new Product { ProductId = Guid.NewGuid(), Name = "Product 1", Price = 10.00m }));
        _resultChain.Add(Result<Product>.Success(new Product { ProductId = Guid.NewGuid(), Name = "Product 2", Price = 20.00m }));
        _resultChain.Add(Result<Product>.Fail("Operation failed"));
    }

    [When(@"I match the result to an action result")]
    public void WhenIMatchTheResultToAnActionResult()
    {
        _logger.LogInformation("Matching result to action result");
        ArgumentNullException.ThrowIfNull(_result);

        _actionResult = _result.Match<Product, IActionResult>(
         value => value is not null ? Ok(value) : NoContent(),
      () => NotFound()
           );
    }

    [When(@"I match the result with error handling")]
    public void WhenIMatchTheResultWithErrorHandling()
    {
        _logger.LogInformation("Matching result with error handling");
        ArgumentNullException.ThrowIfNull(_result);

        _actionResult = _result.Match<Product, IActionResult>(
   value => value is not null ? Ok(value) : NoContent(),
      () => BadRequest(new { Errors = _result.Messages })
     );
    }

    [When(@"I chain the operations together")]
    public void WhenIChainTheOperationsTogether()
    {
        _logger.LogInformation("Chaining result operations");

        // Simulate chaining by checking each result in sequence
        foreach (var result in _resultChain)
        {
            if (!result.Succeeded)
            {
                _result = result;
                break;
            }
        }

        // If all succeeded, use the last one
        if (_result == null || _result.Succeeded)
        {
            _result = _resultChain.Last();
        }
    }

    [Then(@"the response should be an OK result")]
    public void ThenTheResponseShouldBeAnOkResult()
    {
        _logger.LogInformation("Verifying OK result");
        ArgumentNullException.ThrowIfNull(_actionResult);

        if (_actionResult is not OkObjectResult)
        {
            throw new InvalidOperationException($"Expected OkObjectResult but got {_actionResult.GetType().Name}");
        }
    }

    [Then(@"the response should contain the product data")]
    public void ThenTheResponseShouldContainTheProductData()
    {
        _logger.LogInformation("Verifying product data in response");
        ArgumentNullException.ThrowIfNull(_actionResult);

        if (_actionResult is not OkObjectResult okResult)
        {
            throw new InvalidOperationException("Expected OkObjectResult");
        }

        if (okResult.Value is not Product product)
        {
            throw new InvalidOperationException("Expected Product in response");
        }

        if (string.IsNullOrEmpty(product.Name))
        {
            throw new InvalidOperationException("Product should have a name");
        }
    }

    [Then(@"the response should be a NoContent result")]
    public void ThenTheResponseShouldBeANoContentResult()
    {
        _logger.LogInformation("Verifying NoContent result");
        ArgumentNullException.ThrowIfNull(_actionResult);

        if (_actionResult is not NoContentResult)
        {
            throw new InvalidOperationException($"Expected NoContentResult but got {_actionResult.GetType().Name}");
        }
    }

    [Then(@"the response should be a NotFound result")]
    public void ThenTheResponseShouldBeANotFoundResult()
    {
        _logger.LogInformation("Verifying NotFound result");
        ArgumentNullException.ThrowIfNull(_actionResult);

        if (_actionResult is not NotFoundResult)
        {
            throw new InvalidOperationException($"Expected NotFoundResult but got {_actionResult.GetType().Name}");
        }
    }

    [Then(@"the response should include error details")]
    public void ThenTheResponseShouldIncludeErrorDetails()
    {
        _logger.LogInformation("Verifying error details in response");
        ArgumentNullException.ThrowIfNull(_actionResult);

        if (_actionResult is not BadRequestObjectResult badRequest)
        {
            throw new InvalidOperationException("Expected BadRequestObjectResult");
        }

        if (badRequest.Value == null)
        {
            throw new InvalidOperationException("Expected error details in response");
        }
    }

    [Then(@"the response should have appropriate status code")]
    public void ThenTheResponseShouldHaveAppropriateStatusCode()
    {
        _logger.LogInformation("Verifying status code");
        ArgumentNullException.ThrowIfNull(_actionResult);

        var statusCode = (_actionResult as IStatusCodeActionResult)?.StatusCode;
        if (statusCode == null || statusCode == 200)
        {
            throw new InvalidOperationException("Expected non-success status code for failed result");
        }
    }

    [Then(@"all operations should execute in sequence")]
    public void ThenAllOperationsShouldExecuteInSequence()
    {
        _logger.LogInformation("Verifying sequential execution");

        if (_resultChain.Count == 0)
        {
            throw new InvalidOperationException("Expected result chain to have operations");
        }

        _logger.LogInformation("Processed {Count} operations in sequence", _resultChain.Count);
    }

    [Then(@"the final result should reflect the chain outcome")]
    public void ThenTheFinalResultShouldReflectTheChainOutcome()
    {
        _logger.LogInformation("Verifying chain outcome");
        ArgumentNullException.ThrowIfNull(_result);

        // Since one operation in the chain failed, the final result should be a failure
        if (_result.Succeeded)
        {
            throw new InvalidOperationException("Expected final result to be a failure since one operation failed");
        }
    }
}
