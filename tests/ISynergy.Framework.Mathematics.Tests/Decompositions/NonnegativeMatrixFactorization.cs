﻿namespace ISynergy.Framework.Mathematics.Tests;

using ISynergy.Framework.Mathematics;
using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class NonnegativeMatrixFactorizationTest
{

    [TestMethod]
    public void NonNegativeMatrixFactorizationConstructorTest()
    {
        Generator.Seed = 0;

        double[,] expected =
        {
            { 1,     0,     5 },
            { 1,     2,     1 },
            { 0,     6,     1 },
            { 2,     6,     5 },
            { 2,     1,     1 },
            { 5,     1,     1 }
        };


        NonnegativeMatrixFactorization nmf = new(expected, 3);

        double[,] H = nmf.RightNonnegativeFactors;
        double[,] W = nmf.LeftNonnegativeFactors;

        double[,] actual = Matrix.Dot(W, H).Transpose();

        for (int i = 0; i < actual.GetLength(0); i++)
        {
            for (int j = 0; j < actual.GetLength(1); j++)
            {
                double x = actual[i, j];
                double y = expected[i, j];
                Assert.IsTrue(Matrix.IsEqual(actual[i, j], expected[i, j], 0.05));
            }
        }

    }

    [TestMethod]
    public void NonNegativeMatrixFactorizationConstructorTest2()
    {
        double[] data =
        {
            0.814723686, 0.157613082, 0.655740699, 0.706046088, 0.43874436, 0.276025077, 0.751267059, 0.840717256, 0.351659507, 0.07585429,
            0.905791937, 0.970592782, 0.035711679, 0.031832846, 0.381558457, 0.679702677, 0.255095115, 0.254282179, 0.830828628, 0.053950119,
            0.126986816, 0.957166948, 0.849129306, 0.276922985, 0.765516788, 0.655098004, 0.505957052, 0.814284826, 0.585264091, 0.530797553,
            0.913375856, 0.485375649, 0.933993248, 0.046171391, 0.795199901, 0.162611735, 0.699076723, 0.243524969, 0.549723608, 0.77916723,
            0.632359246, 0.800280469, 0.678735155, 0.097131781, 0.186872605, 0.118997682, 0.890903253, 0.929263623, 0.917193664, 0.934010684,
            0.097540405, 0.141886339, 0.757740131, 0.823457828, 0.489764396, 0.498364052, 0.959291425, 0.349983766, 0.285839019, 0.129906208,
            0.278498219, 0.421761283, 0.743132468, 0.694828623, 0.445586201, 0.959743959, 0.54721553, 0.19659525, 0.757200229, 0.568823661,
            0.546881519, 0.915735525, 0.39222702, 0.31709948, 0.64631301, 0.340385727, 0.138624443, 0.251083858, 0.753729094, 0.469390641,
            0.957506835, 0.79220733, 0.65547789, 0.950222049, 0.709364831, 0.585267751, 0.149294006, 0.616044676, 0.380445847, 0.01190207,
            0.964888535, 0.959492426, 0.171186688, 0.034446081, 0.754686682, 0.223811939, 0.257508254, 0.473288849, 0.567821641, 0.337122644
        };

        double[,] input = data.Reshape(10, 10);

        NonnegativeMatrixFactorization nmf = new(input, 2);

        double[,] H = nmf.RightNonnegativeFactors;
        double[,] W = nmf.LeftNonnegativeFactors;

        Assert.IsFalse(H.Has(0));
        Assert.IsFalse(W.Has(0));
    }

}
