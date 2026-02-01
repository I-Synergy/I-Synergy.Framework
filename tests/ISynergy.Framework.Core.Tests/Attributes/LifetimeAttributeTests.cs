using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Attributes.Tests;

[TestClass]
public class LifetimeAttributeTests
{
    [Lifetime(Lifetimes.Singleton)]
    public abstract class baseA
    {
    }

    [Lifetime(Lifetimes.Scoped)]
    public abstract class baseB
    {
    }

    public class A : baseA
    {
    }

    public class B : baseB
    {
    }

    [Lifetime(Lifetimes.Scoped)]
    public class C : baseA
    {

    }

    [Lifetime(Lifetimes.Singleton)]
    public class D : baseB
    {

    }

    [TestMethod]
    public void IsSingletonTest()
    {
        Assert.IsTrue(typeof(baseA).IsSingleton());
        Assert.IsTrue(typeof(A).IsSingleton());
    }

    [TestMethod]
    public void IsScopedTest()
    {
        Assert.IsTrue(typeof(baseB).IsScoped());
        Assert.IsTrue(typeof(B).IsScoped());
    }

    [TestMethod]
    public void IsInheritedSingletonTest()
    {
        Assert.IsTrue(typeof(D).IsSingleton());
        Assert.IsFalse(typeof(D).IsScoped());
    }

    [TestMethod]
    public void IsInheritedScopedTest()
    {
        Assert.IsFalse(typeof(C).IsSingleton());
        Assert.IsTrue(typeof(C).IsScoped());
    }
}
