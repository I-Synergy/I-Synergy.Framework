namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics.Optimization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NonlinearConstraintTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod]
        public void ConstructorTest6()
        {
            var constraints = new[]
            {
                new NonlinearConstraint(2, x =>  1.0 - x[0] * x[0] - x[1] * x[1]),
                new NonlinearConstraint(2, x =>  1.0 - x[0] * x[0] - x[1] * x[1] >= 0),
                new NonlinearConstraint(2, x =>  -x[0] * x[0] - x[1] * x[1] >= -1.0),
                new NonlinearConstraint(2, x =>  -(-x[0] * x[0] - x[1] * x[1]) <= 1.0),
            };

            Assert.AreEqual(ConstraintType.GreaterThanOrEqualTo, constraints[0].ShouldBe);
            Assert.AreEqual(ConstraintType.GreaterThanOrEqualTo, constraints[1].ShouldBe);
            Assert.AreEqual(ConstraintType.GreaterThanOrEqualTo, constraints[2].ShouldBe);
            Assert.AreEqual(ConstraintType.LesserThanOrEqualTo, constraints[3].ShouldBe);

            Assert.AreEqual(0.0, constraints[0].Value);
            Assert.AreEqual(0.0, constraints[1].Value);
            Assert.AreEqual(-1.0, constraints[2].Value);
            Assert.AreEqual(1.0, constraints[3].Value);

            foreach (var c1 in constraints)
            {
                double v1 = c1.GetViolation(new double[] { 4, 2 });

                foreach (var c2 in constraints)
                {
                    double v2 = c2.GetViolation(new double[] { 4, 2 });

                    Assert.AreEqual(v1, v2);
                }
            }
        }

        [TestMethod]
        public void GetViolationTest1()
        {
            NonlinearConstraint[] targets =
            {
                new NonlinearConstraint(1, x => x[0] >= 1),
                new NonlinearConstraint(1, x => x[0] <= 1)
            };

            double[] expected;
            
            expected = new double[] { -0.5, 0.5 };
            for (int i = 0; i < targets.Length; i++)
            {
                double e = expected[i];
                double a = targets[i].GetViolation(new double[] { 0.5 });

                Assert.AreEqual(e, a);
            }

            expected = new double[] { 0.5, -0.5 };
            for (int i = 0; i < targets.Length; i++)
            {
                double e = expected[i];
                double a = targets[i].GetViolation(new double[] { 1.5 });

                Assert.AreEqual(e, a);
            }
        }

        [TestMethod]
        public void GetViolationTest2()
        {
            NonlinearConstraint[] targets =
            {
                new NonlinearConstraint(1, x => x[0] - 1 >= 1),
                new NonlinearConstraint(1, x => x[0] - 1 <= 1)
            };

            double[] expected;

            expected = new double[] { -1.5, 1.5 };
            for (int i = 0; i < targets.Length; i++)
            {
                double e = expected[i];
                double a = targets[i].GetViolation(new double[] { 0.5 });

                Assert.AreEqual(e, a);
            }

            expected = new double[] { 0.5, -0.5 };
            for (int i = 0; i < targets.Length; i++)
            {
                double e = expected[i];
                double a = targets[i].GetViolation(new double[] { 2.5 });

                Assert.AreEqual(e, a);
            }
        }

    }
}
