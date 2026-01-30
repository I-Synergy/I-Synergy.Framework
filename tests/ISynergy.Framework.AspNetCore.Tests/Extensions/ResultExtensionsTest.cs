using ISynergy.Framework.AspNetCore.Tests.Models;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Models.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.AspNetCore.Tests.Extensions;

[TestClass]
public class ResultExtensionsTest : ControllerBase
{
    [TestMethod]
    public void NoContentActionResultTest()
    {
        // Returns 204 (No content)
        var result = Result<Product>.Success(default(Product)!);
        var output = result.Match<Product, IActionResult>(
            value => value is not null ? Ok(value) : NoContent(),
            () => NotFound()
        );
        Assert.AreEqual(typeof(NoContentResult), output.GetType());
    }

    [TestMethod]
    public void OkObjectActionResultTest()
    {
        // Returns 200 (OK)
        var result = Result<Product>.Success(new Product());
        var output = result.Match<Product, IActionResult>(
            value => value is not null ? Ok(value) : NoContent(),
            () => NotFound()
        );
        Assert.AreEqual(typeof(OkObjectResult), output.GetType());
    }

    [TestMethod]
    public void OkActionResultTest()
    {
        // Returns 200 (OK)
        var result = Result<Product>.Success(new Product());
        var output = result.Match<Product, IActionResult>(
            value => value is not null ? Ok() : NoContent(),
            () => NotFound()
        );
        Assert.AreEqual(typeof(OkResult), output.GetType());
    }

    [TestMethod]
    public void NotFoundActionResultTest()
    {
        // Returns 404 (Not found)
        var result = Result<Product>.Fail();
        var output = result.Match<Product, IActionResult>(
            value => value is not null ? Ok(value) : NoContent(),
            () => NotFound()
        );
        Assert.AreEqual(typeof(NotFoundResult), output.GetType());
    }
}
