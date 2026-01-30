using ISynergy.Framework.Mathematics.Matrices;
using ISynergy.Framework.Mathematics.Random;
using ISynergy.Framework.Mathematics.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace ISynergy.Framework.Mathematics.Tests.Random;
[TestClass]
public class GeneratorTest
{

    [TestMethod]
    public void ParallelTest_zero()
    {
        Generator.Seed = 0;
        double[][] l = create(100, 10, reset: false);
        int sameCount = count(l);
        Assert.IsTrue(sameCount > 50);
    }

    [TestMethod]
    [Ignore("Re-enable this test when GH-870 is implemented")]
    public void ParallelTest_zero_2()
    {
        // https://github.com/accord-net/framework/issues/870
        Generator.Seed = 0;
        int n = 100000;
        int?[] seeds = new int?[n];
        int[] values = new int[n];
        Parallel.For(0, seeds.Length, i =>
        {
            seeds[i] = Generator.ThreadSeed;
            values[i] = Generator.Random.Next();
        });

        Assert.IsTrue(seeds.All(x => x == 0));
        Assert.IsTrue(values.All(x => x == values[0]));
    }

    [TestMethod]
    public void ParallelTest_less_than_zero()
    {
        Generator.Seed = -1;
        double[][] l = create(1000, 10, reset: false);
        int sameCount = count(l);
        Assert.IsTrue(sameCount > 30);
    }

    [TestMethod]
    public void ParallelTest_higher_than_zero()
    {
        Generator.Seed = 1;
        double[][] l = create(100, 10, reset: false);
        int sameCount = count(l);
        Assert.IsTrue(sameCount == 0);
    }

    [TestMethod]
    public void ParallelTest_zero_but_reinit()
    {
        Generator.Seed = 0;

        int rows = 100;
        int cols = 10;

        double[][] l = create(rows, cols, reset: true);

        int sameCount = count(l);
        Assert.IsTrue(sameCount == 0);
    }

    [TestMethod]
    public void ParallelTest_null()
    {
        Generator.Seed = null;

        double[][] l = create(100, 10, reset: false);
        int sameCount = count(l);
        Assert.IsTrue(sameCount == 0);
    }

    [TestMethod]
    public void consistency()
    {
        Generator.Seed = 0;
        int[] actual = random(3);
        int[] expected = [1559595546, 1755192844, 1649316166];
        string str = actual.ToCSharp();
        Assert.IsTrue(expected.IsEqual(actual));

        Generator.Seed = -1;
        actual = random(3);
        expected = [534011718, 237820880, 1002897798];
        str = actual.ToCSharp();
        Assert.IsTrue(expected.IsEqual(actual));

        Generator.Seed = 1;
        actual = random(3);
        expected = [607892308, 1910784178, 911229122];
        str = actual.ToCSharp();
        Assert.IsTrue(expected.IsEqual(actual));
    }

    [TestMethod]
    public void thread_initialization()
    {
        Generator.Seed = 0;

        ConcurrentDictionary<int, List<int>> values = new();
        ConcurrentDictionary<int, List<int?>> seeds = new();

        Thread[] t = new Thread[100];

        for (int i = 0; i < 100; i++)
        {
            t[i] = new Thread(() =>
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                List<int> vd = values.GetOrAdd(threadId, []);
                List<int?> sd = seeds.GetOrAdd(threadId, []);

                int? before = Generator.ThreadSeed;
                System.Random r = Generator.Random;
                int? after = Generator.ThreadSeed;

                int v = r.Next();
                vd.Add(v);
                sd.Add(before);
                sd.Add(after);
            });
            t[i].Start();
        };

        for (int i = 0; i < 100; i++)
            t[i].Join();

        int[] keys = values.Keys.ToArray().Sorted();
        Assert.AreEqual(100, keys.Length);

        int? ex = null;
        for (int i = 0; i < keys.Length; i++)
        {
            List<int> l = values[keys[i]];
            if (ex is null)
                ex = l[0];
            else
                Assert.AreEqual(ex.Value, l[0]);

            List<int?> s = seeds[keys[i]];
            // Assert.AreEqual(2, s.Count);
            Assert.IsNull(s[0]);
            Assert.AreEqual(0, s[1]);
        }
    }








    private static int[] random(int n)
    {
        System.Random r = Generator.Random;
        int[] v = new int[n];
        for (int i = 0; i < v.Length; i++)
            v[i] = r.Next();
        return v;
    }

    private static int count(IList<double[]> l)
    {
        int sameCount = 0;
        for (int i = 0; i < l.Count; i++)
        {
            for (int j = 0; j < l.Count; j++)
            {
                double[] li = l[i];
                double[] lj = l[j];

                if (i != j)
                    if (li.IsEqual(lj, atol: 1e-8))
                        sameCount++;
            }
        }

        return sameCount;
    }

    private static double[][] create(int rows, int cols, bool reset)
    {
        double[][] l = new double[rows][];
        Thread[] t = new Thread[rows];
        for (int i = 0; i < rows; i++)
        {
            t[i] = new Thread(thread(cols, reset, l));
            t[i].Start(i);
        }

        for (int i = 0; i < rows; i++)
            t[i].Join();
        return l;
    }

    private static ParameterizedThreadStart thread(int cols, bool reset, double[][] l)
    {
        return (obj) =>
        {
            int j = (int)obj;
            if (reset)
                Generator.ThreadSeed = j;
            l[j] = Vector.Random(cols);
        };
    }
}
