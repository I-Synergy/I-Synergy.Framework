using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Extensions;

[TestClass]
public class UuidExtensionsTests
{
    [TestMethod]
    public void TestUuid7Guid()
    {
        var s1 = Uuidv7.NewGuid();              // e.g. 06338364-8305-788f-8000-9ada942eb663
        string s2 = Uuidv7.NewGuid().ToString();    //      06338364-8305-7b74-8000-de4963503139
        Assert.IsFalse(s1.Equals(s2));
        Assert.IsLessThan(0, String.Compare(s1.ToString(), s2));
    }

    [TestMethod]
    public void TestToId25()
    {
        string s1 = Uuidv7.NewGuid().ToId25();  // e.g. 0q994uri6sp53j3eu7bty4wsv
        string s2 = Uuidv7.NewGuid().ToId25();  //      0q994uri70qe0gjxrq8iv4iyu
        Assert.IsFalse(s1.Equals(s2));
        Assert.IsLessThan(0, String.Compare(s1, s2));
    }

    [TestMethod]
    public void TestFixedTimes()
    {
        long t1 = Uuidv7.CurrentTime();
        long t2 = t1 + 1;
        string s1 = Uuidv7.NewGuid(t1).ToId25(); // e.g. 0q996kioxxyfds1stmjqajen6
        string s2 = Uuidv7.NewGuid(t2).ToId25(); //      0q996kioxxyfj83w8bqp67d2j
        string s3 = Uuidv7.NewGuid(t2).ToId25(); //      0q996kioxxyfj83z4pmujhrx4
        Assert.IsLessThan(0, String.Compare(s1, s2));
        Assert.IsLessThan(0, String.Compare(s2, s3));

        // Using same timestamp give different values due to sequence counter and randomness
        var g1 = Uuidv7.NewGuid(t1);
        var g2 = Uuidv7.NewGuid(t1);
        Assert.AreNotEqual(g2, g1);
    }

    [TestMethod]
    public void TestNoRandomness()
    {
        // Two Id25s from a Uuid7 Guid input add no further randomness
        var uuid7 = Uuidv7.NewGuid();
        long t = Uuidv7.CurrentTime();
        Guid g = Uuidv7.NewGuid(t);
        string s1 = g.ToId25();
        string s2 = g.ToId25();
        Assert.AreEqual(s2, s1);
    }

    [TestMethod]
    public void TestCantUseUuidv4()
    {
        // Attempting to form a Id25 from a Uuid 4 fails
        var g = new Guid(); // UUID v4
        string s = g.ToId25();

        Assert.AreEqual(25, s.Length);
        Assert.AreEqual(new string('0', 25), s);
    }

    [TestMethod]
    public void TestEmpty()
    {
        //var uuid7 = new Uuid7();
        var s1 = Uuidv7.Empty().ToString();
        Assert.AreEqual("00000000-0000-0000-0000-000000000000", s1);
        var s2 = Uuidv7.Empty().ToId25();
        Assert.AreEqual("0000000000000000000000000", s2);

    }
}
