using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Optimization.Losses;
[TestClass]
public class LossesTest
{
    private static readonly int[] expected = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray0 = new int[] { 0, 0, 0, 1 };
    private static readonly int[] expectedArray1 = new int[] { 0, 0, 0, 1 };

    [TestMethod]
    public void Hinge()
    {
        double actual;
        actual = new HingeLoss([0, 1, -0.5, 0.8]).Loss([0, 1, -0.5, 0.8]);
        Assert.AreEqual(1.7, actual);

        actual = new HingeLoss([0, 1, -0.5, 0.8]).Loss([0, 1, 0.5, 0.8]);
        Assert.AreEqual(2.7, actual);

        actual = new HingeLoss([0, 1, -0.5, 0.8]).Loss([-5, 1, 0.5, 0.8]);
        Assert.AreEqual(1.7, actual);

        actual = new HingeLoss([5.4, 1, -0.5, 0.8]).Loss([-5.2, 1, 0.5, 0.8]);
        Assert.AreEqual(7.9, actual, 1e-10);

        actual = new HingeLoss(expected).Loss([1, 1, 1, 1]);
        Assert.AreEqual(2, actual);

        actual = new HingeLoss(expectedArray).Loss([0, 0, 0, 0]);
        Assert.AreEqual(4, actual);

        actual = new HingeLoss(new int[] { -1, 1, -1, -1 }).Loss([-1, -1, -1, -1]);
        Assert.AreEqual(6, actual);

        actual = new HingeLoss(expectedArray0).Loss([-1, 1, 1, 1]);
        Assert.AreEqual(4, actual);

        actual = new HingeLoss(expectedArray1).Loss([-1, -1, -1, 1]);
        Assert.AreEqual(8, actual);

        actual = new HingeLoss(new int[] { -1, -1, -1, 1 }).Loss([0, 0, 0, 1]);
        Assert.AreEqual(5, actual);

        actual = new HingeLoss(new double[] { 0, 1, 0, 0 }).Loss([0, 0, 0, 0]);
        Assert.AreEqual(4, actual);

        Assert.AreEqual(0, new HingeLoss().Loss(1, 1));
        Assert.AreEqual(2, new HingeLoss().Loss(-1, 1));
        Assert.AreEqual(2, new HingeLoss().Loss(1, -1));
        Assert.AreEqual(0, new HingeLoss().Loss(-1, -1));

        Assert.AreEqual(0, new HingeLoss().Loss(1, 5));
        Assert.AreEqual(7, new HingeLoss().Loss(-1, 6));
        Assert.AreEqual(8, new HingeLoss().Loss(1, -7));
        Assert.AreEqual(0, new HingeLoss().Loss(-1, -8));

        Assert.AreEqual(2, new HingeLoss().Loss(-1, 1));
        Assert.AreEqual(1, new HingeLoss().Loss(1, 0));
        Assert.AreEqual(1, new HingeLoss().Loss(-1, 0));

        Assert.AreEqual(0, new HingeLoss().Loss(true, 5));
        Assert.AreEqual(7, new HingeLoss().Loss(false, 6));
        Assert.AreEqual(8, new HingeLoss().Loss(true, -7));
        Assert.AreEqual(0, new HingeLoss().Loss(false, -8));

        Assert.AreEqual(2, new HingeLoss().Loss(false, 1));
        Assert.AreEqual(1, new HingeLoss().Loss(true, 0));
        Assert.AreEqual(1, new HingeLoss().Loss(false, 0));
    }

    private static readonly int[] expectedArray2 = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray3 = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray4 = new int[] { 0, 0, 0, 1 };
    private static readonly int[] expectedArray5 = new int[] { 0, 0, 0, 1 };

    [TestMethod]
    public void SmoothHinge()
    {
        double actual;
        actual = new SmoothHingeLoss([0, 1, -0.5, 0.8]).Loss([0, 1, -0.5, 0.8]);
        Assert.AreEqual(0.645, actual);

        actual = new SmoothHingeLoss([0, 1, -0.5, 0.8]).Loss([0, 1, 0.5, 0.8]);
        Assert.AreEqual(1.52, actual);

        actual = new SmoothHingeLoss([0, 1, -0.5, 0.8]).Loss([-5, 1, 0.5, 0.8]);
        Assert.AreEqual(1.02, actual);

        actual = new SmoothHingeLoss([5.4, 1, -0.5, 0.8]).Loss([-5.2, 1, 0.5, 0.8]);
        Assert.AreEqual(6.72, actual, 1e-10);

        actual = new SmoothHingeLoss(expectedArray2).Loss([1, 1, 1, 1]);
        Assert.AreEqual(1.5, actual);

        actual = new SmoothHingeLoss(expectedArray3).Loss([0, 0, 0, 0]);
        Assert.AreEqual(2, actual);

        actual = new SmoothHingeLoss(new int[] { -1, 1, -1, -1 }).Loss([-1, -1, -1, -1]);
        Assert.AreEqual(4.5, actual);

        actual = new SmoothHingeLoss(expectedArray4).Loss([-1, 1, 1, 1]);
        Assert.AreEqual(3, actual);

        actual = new SmoothHingeLoss(expectedArray5).Loss([-1, -1, -1, 1]);
        Assert.AreEqual(6, actual);

        actual = new SmoothHingeLoss(new int[] { -1, -1, -1, 1 }).Loss([0, 0, 0, 1]);
        Assert.AreEqual(3, actual);

        actual = new SmoothHingeLoss(new double[] { 0, 1, 0, 0 }).Loss([0, 0, 0, 0]);
        Assert.AreEqual(2, actual);

        Assert.AreEqual(0, new SmoothHingeLoss().Loss(1, 1));
        Assert.AreEqual(1.5, new SmoothHingeLoss().Loss(-1, 1));
        Assert.AreEqual(1.5, new SmoothHingeLoss().Loss(1, -1));
        Assert.AreEqual(0, new SmoothHingeLoss().Loss(-1, -1));

        Assert.AreEqual(0, new SmoothHingeLoss().Loss(1, 5));
        Assert.AreEqual(6.5, new SmoothHingeLoss().Loss(-1, 6));
        Assert.AreEqual(7.5, new SmoothHingeLoss().Loss(1, -7));
        Assert.AreEqual(0, new SmoothHingeLoss().Loss(-1, -8));

        Assert.AreEqual(1.5, new SmoothHingeLoss().Loss(-1, 1));
        Assert.AreEqual(0.5, new SmoothHingeLoss().Loss(1, 0));
        Assert.AreEqual(0.5, new SmoothHingeLoss().Loss(-1, 0));

        Assert.AreEqual(0, new SmoothHingeLoss().Loss(true, 5));
        Assert.AreEqual(6.5, new SmoothHingeLoss().Loss(false, 6));
        Assert.AreEqual(7.5, new SmoothHingeLoss().Loss(true, -7));
        Assert.AreEqual(0, new SmoothHingeLoss().Loss(false, -8));

        Assert.AreEqual(1.5, new SmoothHingeLoss().Loss(false, 1));
        Assert.AreEqual(0.5, new SmoothHingeLoss().Loss(true, 0));
        Assert.AreEqual(0.5, new SmoothHingeLoss().Loss(false, 0));
    }

    private static readonly int[] expectedArray6 = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray7 = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray8 = new int[] { 0, 0, 0, 1 };
    private static readonly int[] expectedArray9 = new int[] { 0, 0, 0, 1 };

    [TestMethod]
    public void SquaredHinge()
    {
        double actual;
        actual = new SquaredHingeLoss([0, 1, -0.5, 0.8]).Loss([0, 1, -0.5, 0.8]);
        Assert.AreEqual(0.645, actual);

        actual = new SquaredHingeLoss([0, 1, -0.5, 0.8]).Loss([0, 1, 0.5, 0.8]);
        Assert.AreEqual(1.645, actual);

        actual = new SquaredHingeLoss([0, 1, -0.5, 0.8]).Loss([-5, 1, 0.5, 0.8]);
        Assert.AreEqual(1.145, actual);

        actual = new SquaredHingeLoss([5.4, 1, -0.5, 0.8]).Loss([-5.2, 1, 0.5, 0.8]);
        Assert.AreEqual(20.365, actual, 1e-10);

        actual = new SquaredHingeLoss(expectedArray6).Loss([1, 1, 1, 1]);
        Assert.AreEqual(2, actual);

        actual = new SquaredHingeLoss(expectedArray7).Loss([0, 0, 0, 0]);
        Assert.AreEqual(2, actual);

        actual = new SquaredHingeLoss(new int[] { -1, 1, -1, -1 }).Loss([-1, -1, -1, -1]);
        Assert.AreEqual(6, actual);

        actual = new SquaredHingeLoss(expectedArray8).Loss([-1, 1, 1, 1]);
        Assert.AreEqual(4, actual);

        actual = new SquaredHingeLoss(expectedArray9).Loss([-1, -1, -1, 1]);
        Assert.AreEqual(8, actual);

        actual = new SquaredHingeLoss(new int[] { -1, -1, -1, 1 }).Loss([0, 0, 0, 1]);
        Assert.AreEqual(3.5, actual);

        actual = new SquaredHingeLoss(new double[] { 0, 1, 0, 0 }).Loss([0, 0, 0, 0]);
        Assert.AreEqual(2, actual);

        Assert.AreEqual(0, new SquaredHingeLoss().Loss(1, 1));
        Assert.AreEqual(2, new SquaredHingeLoss().Loss(-1, 1));
        Assert.AreEqual(2, new SquaredHingeLoss().Loss(1, -1));
        Assert.AreEqual(0, new SquaredHingeLoss().Loss(-1, -1));

        Assert.AreEqual(0, new SquaredHingeLoss().Loss(1, 5));
        Assert.AreEqual(24.5, new SquaredHingeLoss().Loss(-1, 6));
        Assert.AreEqual(32, new SquaredHingeLoss().Loss(1, -7));
        Assert.AreEqual(0, new SquaredHingeLoss().Loss(-1, -8));

        Assert.AreEqual(2, new SquaredHingeLoss().Loss(-1, 1));
        Assert.AreEqual(0.5, new SquaredHingeLoss().Loss(1, 0));
        Assert.AreEqual(0.5, new SquaredHingeLoss().Loss(-1, 0));

        Assert.AreEqual(0, new SquaredHingeLoss().Loss(true, 5));
        Assert.AreEqual(24.5, new SquaredHingeLoss().Loss(false, 6));
        Assert.AreEqual(32, new SquaredHingeLoss().Loss(true, -7));
        Assert.AreEqual(0, new SquaredHingeLoss().Loss(false, -8));

        Assert.AreEqual(2, new SquaredHingeLoss().Loss(false, 1));
        Assert.AreEqual(0.5, new SquaredHingeLoss().Loss(true, 0));
        Assert.AreEqual(0.5, new SquaredHingeLoss().Loss(false, 0));
    }

    private static readonly int[] expectedArray10 = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray11 = new int[] { 0, 1, 0, 0 };
    private static readonly int[] expectedArray12 = new int[] { 0, 0, 0, 1 };
    private static readonly int[] expectedArray13 = new int[] { 0, 0, 0, 1 };

    [TestMethod]
    public void Logistic()
    {
        double actual;
        actual = new LogisticLoss([0, 1, -0.5, 0.8]).Loss([0, 1, -0.5, 0.8]);
        Assert.AreEqual(2.8938223431265548, actual, 1e-10);

        actual = new LogisticLoss([0, 1, -0.5, 0.8]).Loss([0, 1, 0.5, 0.8]);
        Assert.AreEqual(3.2544961033487954, actual, 1e-10);

        actual = new LogisticLoss([0, 1, -0.5, 0.8]).Loss([-5, 1, 0.5, 0.8]);
        Assert.AreEqual(3.2544961033487954, actual, 1e-10);

        actual = new LogisticLoss([5.4, 1, -0.5, 0.8]).Loss([-5.2, 1, 0.5, 0.8]);
        Assert.AreEqual(42.765372851511813, actual, 1e-10);

        actual = new LogisticLoss(expectedArray10).Loss([1, 1, 1, 1]);
        Assert.AreEqual(3.250459373221156, actual, 1e-10);

        actual = new LogisticLoss(expectedArray11).Loss([0, 0, 0, 0]);
        Assert.AreEqual(4, actual);

        actual = new LogisticLoss(new int[] { -1, 1, -1, -1 }).Loss([-1, -1, -1, -1]);
        Assert.AreEqual(6.1358494549990832, actual, 1e-10);

        actual = new LogisticLoss(expectedArray12).Loss([-1, 1, 1, 1]);
        Assert.AreEqual(4.6931544141101194, actual, 1e-10);

        actual = new LogisticLoss(expectedArray13).Loss([-1, -1, -1, 1]);
        Assert.AreEqual(7.5785444958880461, actual, 1e-10);

        actual = new LogisticLoss(new int[] { -1, -1, -1, 1 }).Loss([0, 0, 0, 1]);
        Assert.AreEqual(4.8946361239720115, actual, 1e-10);

        actual = new LogisticLoss(new double[] { 0, 1, 0, 0 }).Loss([0, 0, 0, 0]);
        Assert.AreEqual(4, actual);

        Assert.AreEqual(0.31326168751822286d, new LogisticLoss().Loss(1, 1), 1e-10);
        Assert.AreEqual(1.3132616875182228d, new LogisticLoss().Loss(-1, 1), 1e-10);
        Assert.AreEqual(1.3132616875182228d, new LogisticLoss().Loss(1, -1), 1e-10);
        Assert.AreEqual(0.31326168751822286d, new LogisticLoss().Loss(-1, -1), 1e-10);

        Assert.AreEqual(0.0067153484891179669d, new LogisticLoss().Loss(1, 5), 1e-10);
        Assert.AreEqual(6.0024756851377301d, new LogisticLoss().Loss(-1, 6), 1e-10);
        Assert.AreEqual(7.0009114664537737d, new LogisticLoss().Loss(1, -7), 1e-10);
        Assert.AreEqual(0.00033540637289566238d, new LogisticLoss().Loss(-1, -8), 1e-10);

        Assert.AreEqual(1.3132616875182228d, new LogisticLoss().Loss(-1, 1), 1e-10);
        Assert.AreEqual(0.69314718055994529d, new LogisticLoss().Loss(1, 0), 1e-10);
        Assert.AreEqual(0.69314718055994529d, new LogisticLoss().Loss(-1, 0), 1e-10);

        Assert.AreEqual(0.0067153484891179669d, new LogisticLoss().Loss(true, 5), 1e-10);
        Assert.AreEqual(6.0024756851377301d, new LogisticLoss().Loss(false, 6), 1e-10);
        Assert.AreEqual(7.0009114664537737d, new LogisticLoss().Loss(true, -7), 1e-10);
        Assert.AreEqual(0.00033540637289566238d, new LogisticLoss().Loss(false, -8), 1e-10);

        Assert.AreEqual(1.3132616875182228d, new LogisticLoss().Loss(false, 1), 1e-10);
        Assert.AreEqual(0.69314718055994529d, new LogisticLoss().Loss(true, 0), 1e-10);
        Assert.AreEqual(0.69314718055994529d, new LogisticLoss().Loss(false, 0), 1e-10);
    }
}
