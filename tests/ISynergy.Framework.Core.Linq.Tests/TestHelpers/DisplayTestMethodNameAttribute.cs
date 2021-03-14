using System;
using System.Reflection;
using Xunit.Sdk;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.TestHelpers
{
    /// <summary>
    /// Class DisplayTestMethodNameAttribute.
    /// Implements the <see cref="Xunit.Sdk.BeforeAfterTestAttribute" />
    /// </summary>
    /// <seealso cref="Xunit.Sdk.BeforeAfterTestAttribute" />
    class DisplayTestMethodNameAttribute : BeforeAfterTestAttribute
    {
        /// <summary>
        /// This method is called before the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public override void Before(MethodInfo methodUnderTest)
        {
            Console.WriteLine("Setup for test '{0}.'", methodUnderTest.Name);
        }

        /// <summary>
        /// Afters the specified method under test.
        /// </summary>
        /// <param name="methodUnderTest">The method under test.</param>
        public override void After(MethodInfo methodUnderTest)
        {
            Console.WriteLine("TearDown for test '{0}.'", methodUnderTest.Name);
        }
    }
}
