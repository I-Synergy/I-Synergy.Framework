using ISynergy.Framework.Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests.Collections;

[TestClass]
public class RedBlackDictionaryTest
{

    [TestMethod]
    public void DuplicateTest()
    {
        RedBlackTreeDictionary<int, string> map = new()
        {
            [1] = "1"
        };

        Assert.AreEqual("1", map[1]);

        map[2] = "2";

        Assert.AreEqual("2", map[2]);

        map[1] = "3";

        Assert.AreEqual("3", map[1]);

        Assert.AreEqual(2, map.Count);
    }

    [TestMethod]
    public void EmptyTest()
    {
        RedBlackTreeDictionary<int, string> map = [];

        bool thrown;

        thrown = false;
        try { string c = map[1]; }
        catch (KeyNotFoundException) { thrown = true; }
        Assert.IsTrue(thrown);

        thrown = false;
        try { KeyValuePair<int, string> min = map.Min(); }
        catch (InvalidOperationException) { thrown = true; }
        Assert.IsTrue(thrown);

        thrown = false;
        try { KeyValuePair<int, string> max = map.Max(); }
        catch (InvalidOperationException) { thrown = true; }
        Assert.IsTrue(thrown);

        thrown = false;
        try { KeyValuePair<int, string> next = map.GetNext(0); }
        catch (KeyNotFoundException) { thrown = true; }
        Assert.IsTrue(thrown);

        thrown = false;
        try { KeyValuePair<int, string> prev = map.GetPrevious(0); }
        catch (KeyNotFoundException) { thrown = true; }
        Assert.IsTrue(thrown);

    }

    [TestMethod]
    public void NextPrevTest()
    {
        bool thrown = false;

        RedBlackTreeDictionary<int, string> map = new()
        {
            [0] = "0",
            [1] = "1",
            [2] = "2"
        };


        {
            KeyValuePair<int, string> a = map.GetNext(0);
            Assert.AreEqual(1, a.Key);
            Assert.IsTrue(map.TryGetNext(0, out a));
            Assert.AreEqual(1, a.Key);

            KeyValuePair<int, string> b = map.GetNext(1);
            Assert.AreEqual(2, b.Key);
            Assert.IsTrue(map.TryGetNext(1, out b));
            Assert.AreEqual(2, b.Key);

            thrown = false;
            try { map.GetNext(2); }
            catch (KeyNotFoundException) { thrown = true; }
            Assert.IsTrue(thrown);
            Assert.IsFalse(map.TryGetNext(2, out b));

            thrown = false;
            try { map.GetNext(-1); }
            catch (KeyNotFoundException) { thrown = true; }
            Assert.IsTrue(thrown);
            Assert.IsFalse(map.TryGetNext(-1, out b));
        }

        {
            KeyValuePair<int, string> a;

            thrown = false;
            try { a = map.GetPrevious(0); }
            catch (KeyNotFoundException) { thrown = true; }
            Assert.IsTrue(thrown);
            Assert.IsFalse(map.TryGetPrevious(0, out a));
            Assert.AreEqual(0, a.Key);

            KeyValuePair<int, string> b = map.GetPrevious(1);
            Assert.AreEqual(0, b.Key);
            Assert.IsTrue(map.TryGetPrevious(1, out b));
            Assert.AreEqual(0, b.Key);

            KeyValuePair<int, string> c = map.GetPrevious(2);
            Assert.AreEqual(1, c.Key);
            Assert.IsTrue(map.TryGetPrevious(2, out b));
            Assert.AreEqual(1, b.Key);

            thrown = false;
            try { map.GetNext(3); }
            catch (KeyNotFoundException) { thrown = true; }
            Assert.IsTrue(thrown);
        }
    }

}
