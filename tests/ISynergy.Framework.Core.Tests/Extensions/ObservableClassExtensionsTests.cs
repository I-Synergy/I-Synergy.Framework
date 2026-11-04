using ISynergy.Framework.Core.Data.TestClasses;
using ISynergy.Framework.Core.Extensions;

namespace ISynergy.Framework.Core.Extensions;

[TestClass]
public class ObservableClassExtensionsTests
{
    private static readonly string[] _alwaysExcluded =
    [
        "Error", "Errors", "IsDirty", "HasErrors", "Properties",
        "Validator", "AutomaticValidationTrigger", "IsValid", "Version", "Item"
    ];

    private static readonly string[] _memoProperties = ["Memo"];

    private static readonly string[] _auditProperties =
        ["CreatedDate", "CreatedBy", "ChangedDate", "ChangedBy"];

    [TestMethod]
    public void GetProperties_Defaults_ExcludesInfrastructureProperties()
    {
        var product = new Product();
        var result = product.GetProperties();

        foreach (var name in _alwaysExcluded)
            Assert.DoesNotContain(p => p.Name == name, result, $"{name} should always be excluded");
    }

    [TestMethod]
    public void GetProperties_Defaults_ExcludesMemoAndAuditProperties()
    {
        var product = new Product();
        var result = product.GetProperties();

        foreach (var name in _memoProperties.Concat(_auditProperties))
            Assert.DoesNotContain(p => p.Name == name, result, $"{name} should be excluded by default");
    }

    [TestMethod]
    public void GetProperties_Defaults_IncludesOwnProperties()
    {
        var product = new Product();
        var result = product.GetProperties();

        Assert.Contains(p => p.Name == nameof(Product.Name), result);
        Assert.Contains(p => p.Name == nameof(Product.ProductId), result);
        Assert.Contains(p => p.Name == nameof(Product.Quantity), result);
        Assert.Contains(p => p.Name == nameof(Product.Price), result);
    }

    [TestMethod]
    public void GetProperties_IncludeMemoTrue_IncludesMemo()
    {
        var product = new Product();
        var result = product.GetProperties(includeMemo: true);

        Assert.Contains(p => p.Name == "Memo", result);
    }

    [TestMethod]
    public void GetProperties_IncludeMemoFalse_ExcludesMemo()
    {
        var product = new Product();
        var result = product.GetProperties(includeMemo: false);

        Assert.DoesNotContain(p => p.Name == "Memo", result);
    }

    [TestMethod]
    public void GetProperties_IncludeAuditTrue_IncludesAllAuditFields()
    {
        var product = new Product();
        var result = product.GetProperties(includeAudit: true);

        foreach (var name in _auditProperties)
            Assert.Contains(p => p.Name == name, result, $"{name} should be included when includeAudit=true");
    }

    [TestMethod]
    public void GetProperties_IncludeAuditFalse_ExcludesAllAuditFields()
    {
        var product = new Product();
        var result = product.GetProperties(includeAudit: false);

        foreach (var name in _auditProperties)
            Assert.DoesNotContain(p => p.Name == name, result, $"{name} should be excluded when includeAudit=false");
    }

    [TestMethod]
    public void GetProperties_BothFlagsTrue_IncludesMemoAndAuditFields()
    {
        var product = new Product();
        var result = product.GetProperties(includeMemo: true, includeAudit: true);

        foreach (var name in _memoProperties.Concat(_auditProperties))
            Assert.Contains(p => p.Name == name, result, $"{name} should be included when both flags are true");
    }

    [TestMethod]
    public void GetProperties_BothFlagsTrue_StillExcludesInfrastructureProperties()
    {
        var product = new Product();
        var result = product.GetProperties(includeMemo: true, includeAudit: true);

        foreach (var name in _alwaysExcluded)
            Assert.DoesNotContain(p => p.Name == name, result, $"{name} should always be excluded");
    }
}
