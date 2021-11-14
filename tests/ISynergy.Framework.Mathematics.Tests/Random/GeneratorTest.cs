namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using ISynergy.Framework.Mathematics.Random;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [TestClass]
    public class GeneratorTest
    {

        [TestMethod]
        public void ParallelTest_zero()
        {
            Generator.Seed = 0;
            var l = create(100, 10, reset: false);
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
            var seeds = new int?[n];
            var values = new int[n];
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
            var l = create(1000, 10, reset: false);
            int sameCount = count(l);
            Assert.IsTrue(sameCount > 30);
        }

        [TestMethod]
        public void ParallelTest_higher_than_zero()
        {
            Generator.Seed = 1;
            var l = create(100, 10, reset: false);
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

            var l = create(100, 10, reset: false);
            int sameCount = count(l);
            Assert.IsTrue(sameCount == 0);
        }

        [TestMethod]
        public void consistency()
        {
            Generator.Seed = 0;
            int[] actual = random(3);
            int[] expected = new int[] { 1559595546, 1755192844, 1649316166 };
            var str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual));

            Generator.Seed = -1;
            actual = random(3);
            expected = new int[] { 534011718, 237820880, 1002897798 };
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual));

            Generator.Seed = 1;
            actual = random(3);
            expected = new int[] { 607892308, 1910784178, 911229122 };
            str = actual.ToCSharp();
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void thread_initialization()
        {
            Generator.Seed = 0;

            var values = new ConcurrentDictionary<int, List<int>>();
            var seeds = new ConcurrentDictionary<int, List<int?>>();

            Thread[] t = new Thread[100];

            for (int i = 0; i < 100; i++)
            {
                t[i] = new Thread(() =>
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    var vd = values.GetOrAdd(threadId, new List<int>());
                    var sd = seeds.GetOrAdd(threadId, new List<int?>());

                    int? before = Generator.ThreadSeed;
                    var r = Generator.Random;
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
                var l = values[keys[i]];
                if (ex == null)
                    ex = l[0];
                else
                    Assert.AreEqual(ex.Value, l[0]);

                var s = seeds[keys[i]];
                // Assert.AreEqual(2, s.Count);
                Assert.IsNull(s[0]);
                Assert.AreEqual(0, s[1]);
            }
        }








        private static int[] random(int n)
        {
            var r = Generator.Random;
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
                    var li = l[i];
                    var lj = l[j];

                    if (i != j)
                        if (li.IsEqual(lj, atol: 1e-8))
                            sameCount++;
                }
            }

            return sameCount;
        }

        private static double[][] create(int rows, int cols, bool reset)
        {
            var l = new double[rows][];
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
            return (object obj) =>
            {
                int j = (int)obj;
                if (reset)
                    Generator.ThreadSeed = j;
                l[j] = Vector.Random(cols);
            };
        }
    }

}
