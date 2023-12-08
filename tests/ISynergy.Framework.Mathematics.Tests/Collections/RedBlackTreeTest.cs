namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Core.Collections;
using ISynergy.Framework.Mathematics;
using ISynergy.Framework.Mathematics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class RedBlackTreeTest
{

    [TestMethod]
    public void BaseTest()
    {
        run(10);
    }

    [TestMethod]
    public void ExtensiveTest()
    {
        for (int i = 1; i < 10; i++)
            run(i);

        for (int i = 10; i < 1500; i += 47)
            run(i);

        run(2000);
    }


    [TestMethod]
    public void DuplicateTest()
    {
        RedBlackTree<int, string> t = [];
        ICollection<KeyValuePair<int, string>> collection = t;

        KeyValuePair<int, string>[] values = collection.ToArray();
        Assert.AreEqual(0, values.Length);

        t.Add(new KeyValuePair<int, string>(1, "1"));
        values = collection.ToArray();
        Assert.AreEqual(1, values.Length);

        t.Add(new KeyValuePair<int, string>(2, "2"));
        values = collection.ToArray();
        Assert.AreEqual(2, values.Length);
        Assert.AreEqual("1", values[0].Value);
        Assert.AreEqual("2", values[1].Value);


        t.Add(new KeyValuePair<int, string>(1, "bla"));
        values = collection.ToArray();
        Assert.AreEqual(2, values.Length);
        Assert.AreEqual("bla", values[0].Value);
        Assert.AreEqual("2", values[1].Value);

        RedBlackTreeNode<KeyValuePair<int, string>> node1 = t.Remove(new KeyValuePair<int, string>(1, "-"));
        Assert.IsNotNull(node1);
        values = collection.ToArray();
        Assert.AreEqual(1, values.Length);
        Assert.AreEqual("2", values[0].Value);

        RedBlackTreeNode<KeyValuePair<int, string>> node2 = t.Remove(new KeyValuePair<int, string>(1, "-"));
        Assert.IsNull(node2);
        values = collection.ToArray();
        Assert.AreEqual(1, values.Length);
        Assert.AreEqual("2", values[0].Value);

        RedBlackTreeNode<KeyValuePair<int, string>> node3 = t.Remove(new KeyValuePair<int, string>(2, "-"));
        values = collection.ToArray();
        Assert.AreEqual(0, values.Length);
    }

    [TestMethod]
    public void ExtensiveDuplicateTest()
    {
        for (int i = 1; i < 10; i++)
            duplicates(i);

        for (int i = 10; i < 1500; i += 47)
            duplicates(i);
    }


    private static void run(int n)
    {
        Random rand = Generator.Random;

        RedBlackTree<int> t = new(allowDuplicates: true);

        // Create a vector of random numbers
        int[] k = new int[n];
        for (int i = 0; i < k.Length; i++)
            k[i] = rand.Next(k.Length);

        int[] sorted = (int[])k.Clone();
        Array.Sort(sorted);

        // Populate the tree with numbers
        for (int i = 0; i < k.Length; i++)
        {
            RedBlackTreeNode<int> node = t.Add(k[i]);

            Assert.IsNotNull(node);
            Assert.AreEqual(k[i], node.Value);

            Assert.IsTrue(t.check());
        }

        Assert.AreEqual(k.Length, t.Count);


        // Check that all elements are in the tree
        for (int i = 0; i < k.Length; ++i)
        {
            RedBlackTreeNode<int> node = t.Find(k[i]);

            Assert.IsNotNull(node);
            Assert.AreEqual(k[i], node.Value);

            Assert.IsTrue(t.Contains(k[i]));
            Assert.IsTrue(t.Contains(node));
        }

        // Enumerate the values (must be in order)
        int arrayIndex = 0;
        foreach (RedBlackTreeNode<int> node in t)
            Assert.AreEqual(sorted[arrayIndex++], node.Value);

        // Start from min and go navigating up to max
        RedBlackTreeNode<int> min = t.Min();
        Assert.IsNotNull(min);
        Assert.AreEqual(k.Min(), min.Value);

        for (int i = 0; i < k.Length; i++)
        {
            Assert.IsNotNull(min);
            min = t.GetNextNode(min);
        }
        Assert.IsNull(min); // the last should be null.

        // Start from max and go navigating down to min
        RedBlackTreeNode<int> max = t.Max();
        Assert.AreEqual(k.Max(), max.Value);
        for (int i = 0; i < k.Length; i++)
        {
            Assert.IsNotNull(max);
            max = t.GetPreviousNode(max);
        }
        Assert.IsNull(max); // the last should be null.


        // Exercise the tree
        for (int M = k.Length; M > 0; M--)
        {
            int knew = rand.Next(k.Length); // random new key 
            int j = rand.Next(M); // random original key to replace 
            int i;

            for (i = 0; i < k.Length; i++)
                if (k[i] >= 0)
                    if (j-- == 0)
                        break;


            if (i >= k.Length)
                Assert.Fail();

            int kd = k[i];

            RedBlackTreeNode<int> node = t.Find(kd);
            Assert.IsNotNull(node);

            node.Value = knew;

            Assert.IsNotNull(t.Resort(node));
            Assert.IsTrue(t.check());

            k[i] = -1 - knew;

            Assert.AreEqual(k.Length, t.Count);
        }

        for (int i = 0; i < k.Length; i++)
            k[i] = -1 - k[i]; // undo negation above




        // check the localization functions
        for (int i = 0; i < k.Length; i++)
        {
            int kd = (int)(0.01 * (rand.Next() % (k.Length * 150) - k.Length * 25));

            RedBlackTreeNode<int> le = t.FindLessThanOrEqualTo(kd);
            RedBlackTreeNode<int> gt = t.FindGreaterThan(kd);

            RedBlackTreeNode<int> node = t.Min();

            double lek = le is not null ? le.Value : int.MinValue;
            double gtk = gt is not null ? gt.Value : int.MaxValue;


            if (node.Value > kd)
            {
                Assert.IsNull(le);
                Assert.IsNotNull(gt);
                Assert.AreEqual(gt, node);
            }
            else
            {
                RedBlackTreeNode<int> succ = node;
                do
                {
                    node = succ;
                    succ = t.GetNextNode(node);
                } while (succ is not null && succ.Value <= kd);

                Assert.AreEqual(node, le);
                Assert.AreEqual(succ, gt);
            }
        }

        // Remove elements from the tree
        for (int M = k.Length; M > 0; M--)
        {
            int j = rand.Next() % M;
            int i;
            for (i = 0; i < k.Length; i++)
                if (k[i] >= 0)
                    if (j-- == 0)
                        break;

            if (i >= k.Length)
                Assert.Fail();

            int kd = k[i];

            RedBlackTreeNode<int> node = t.Find(kd);
            Assert.IsNotNull(node);

            node = t.Remove(node);
            Assert.IsTrue(t.check());

            k[i] = -1 - k[i];
        }

        // The tree should be empty
        Assert.AreEqual(0, t.Count);
    }

    private static void duplicates(int n)
    {
        Random rand = Generator.Random;

        RedBlackTree<int> t = [];

        // Create a vector of random numbers with duplicates
        int[] k = new int[n];
        for (int i = 0; i < k.Length; i++)
            k[i] = i;

        Vector.Shuffle(k);

        int[] sorted = (int[])k.Clone();
        Array.Sort(sorted);

        // Populate the tree with numbers
        for (int i = 0; i < k.Length; i++)
        {
            RedBlackTreeNode<int> node = t.Add(k[i]);

            Assert.IsNotNull(node);
            Assert.AreEqual(k[i], node.Value);

            Assert.IsTrue(t.check());
        }

        Assert.AreEqual(k.Length, t.Count);


        // Check that all elements are in the tree
        for (int i = 0; i < k.Length; i++)
        {
            RedBlackTreeNode<int> node = t.Find(k[i]);

            Assert.IsNotNull(node);
            Assert.AreEqual(k[i], node.Value);

            Assert.IsTrue(t.Contains(k[i]));
            Assert.IsTrue(t.Contains(node));
        }

        // Enumerate the values (must be in order)
        int arrayIndex = 0;
        foreach (RedBlackTreeNode<int> node in t)
            Assert.AreEqual(sorted[arrayIndex++], node.Value);



        // Populate the tree with the same numbers
        for (int i = 0; i < k.Length; i++)
        {
            RedBlackTreeNode<int> node = t.Add(k[i]);

            Assert.IsNotNull(node);
            Assert.AreEqual(k[i], node.Value);

            Assert.IsTrue(t.check());
        }

        Assert.IsTrue(t.check());

        // Enumerate the values (must be in order)
        arrayIndex = 0;
        foreach (RedBlackTreeNode<int> node in t)
            Assert.AreEqual(sorted[arrayIndex++], node.Value);
    }

}
