using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mathematics;

namespace ISynergy.Framework.Mathematics.Common
{
    /// <summary>
    ///   Common mathematical constants.
    /// </summary>
    ///  
    /// <remarks>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Cephes Math Library, http://www.netlib.org/cephes/ </description></item>
    ///     <item><description>
    ///       http://www.johndcook.com/cpp_expm1.html </description></item>
    ///   </list>
    /// </remarks>
    /// 
    public static class Constants
    {
        /// <summary>
        ///   The number one (1).
        /// </summary>
        /// 
        public static T One<T>()
        {
            return 1.To<T>();
        }

        /// <summary>
        ///   The number zero (0).
        /// </summary>
        /// 
        public static T Zero<T>()
        {
            return 0.To<T>();
        }

        /// <summary>
        ///   Euler-Mascheroni constant.
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant is defined as 0.5772156649015328606065120.
        /// </remarks>
        /// 
        public const double EulerGamma = 0.5772156649015328606065120;

        /// <summary>
        ///   Double-precision machine round-off error.
        /// </summary>
        /// 
        /// <remarks>
        ///   This value is actually different from Double.Epsilon. It
        ///   is defined as 1.11022302462515654042E-16.
        /// </remarks>
        /// 
        public const double DoubleEpsilon = 1.11022302462515654042e-16;

        /// <summary>
        ///   Double-precision machine round-off error.
        /// </summary>
        /// 
        /// <remarks>
        ///   This value is actually different from Double.Epsilon. It
        ///   is defined as 1.11022302462515654042E-16.
        /// </remarks>
        /// 
        public const decimal DecimalEpsilon = 0.0000000000000000000000000001M;

        /// <summary>
        ///   Single-precision machine round-off error.
        /// </summary>
        /// 
        /// <remarks>
        ///   This value is actually different from Single.Epsilon. It
        ///   is defined as 1.1920929E-07f.
        /// </remarks>
        /// 
        public const float SingleEpsilon = 1.1920929E-07f;

        /// <summary>
        ///   Double-precision small value.
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant is defined as 1.493221789605150e-300.
        /// </remarks>
        /// 
        public const double DoubleSmall = 1.493221789605150e-300;

        /// <summary>
        ///   Single-precision small value.
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant is defined as 1.493221789605150e-40f.
        /// </remarks>
        /// 
        public const float SingleSmall = 1.493221789605150e-40f;

        /// <summary>
        ///   Fixed-precision small value.
        /// </summary>
        /// 
        public const decimal DecimalSmall = decimal.MinValue;

        /// <summary>
        ///   Maximum log on the machine.
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant is defined as 7.09782712893383996732E2.
        /// </remarks>
        /// 
        public const double LogMax = 7.09782712893383996732E2;

        /// <summary>
        ///   Minimum log on the machine.
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant is defined as -7.451332191019412076235E2.
        /// </remarks>
        /// 
        public const double LogMin = -7.451332191019412076235E2;

        /// <summary>
        ///   Catalan's constant. 
        /// </summary>
        /// 
        public const double Catalan = 0.915965594177219015054603514;

        /// <summary>
        ///   Log of number pi: log(pi).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 1.14472988584940017414.
        /// </remarks>
        /// 
        public const double LogPI = 1.14472988584940017414;

        /// <summary>
        ///   Log of two: log(2).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 0.69314718055994530941.
        /// </remarks>
        /// 
        public const double Log2 = 0.69314718055994530941;

        /// <summary>
        ///   Log of three: log(3).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 1.098612288668109691395.
        /// </remarks>
        /// 
        public const double Log3 = 1.098612288668109691395;

        /// <summary>
        ///   Log of square root of twice number pi: sqrt(log(2*π).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 0.91893853320467274178032973640562.
        /// </remarks>
        /// 
        public const double LogSqrt2PI = 0.91893853320467274178032973640562;

        /// <summary>
        ///   Log of twice number pi: log(2*pi).
        /// </summary>
        /// 
        /// 
        /// <remarks>
        ///   This constant has the value 1.837877066409345483556.
        /// </remarks>
        /// 
        public const double Log2PI = 1.837877066409345483556;

        /// <summary>
        ///   Square root of twice number pi: sqrt(2*π).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 2.50662827463100050242E0.
        /// </remarks>
        /// 
        public const double Sqrt2PI = 2.50662827463100050242E0;

        /// <summary>
        ///   Square root of half number π: sqrt(π/2).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 1.25331413731550025121E0.
        /// </remarks>
        /// 
        public const double SqrtHalfPI = 1.25331413731550025121E0;

        /// <summary>
        ///   Square root of 2: sqrt(2).
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 1.4142135623730950488016887.
        /// </remarks>
        /// 
        public const double Sqrt2 = 1.4142135623730950488016887;

        /// <summary>
        ///   Half square root of 2: sqrt(2)/2.
        /// </summary>
        /// 
        /// <remarks>
        ///   This constant has the value 7.07106781186547524401E-1.
        /// </remarks>
        /// 
        public const double Sqrt2H = 7.07106781186547524401E-1;

    }
}
