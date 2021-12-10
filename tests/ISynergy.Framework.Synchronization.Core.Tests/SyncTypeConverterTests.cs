using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Text;

namespace ISynergy.Framework.Synchronization.Core.Tests
{
    [TestClass]
    public class SyncTypeConverterTests
    {
        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToInt16(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<short>(input);
            Assert.IsInstanceOfType(o1, typeof(short));
            Assert.AreEqual(12, o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToInt32(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<int>(input);
            Assert.IsInstanceOfType(o1, typeof(int));
            Assert.AreEqual(12, o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToInt64(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<long>(input);
            Assert.IsInstanceOfType(o1, typeof(long));
            Assert.AreEqual(12, o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToUInt16(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<ushort>(input);
            Assert.IsInstanceOfType(o1, typeof(ushort));
            Assert.AreEqual(12, o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToUInt32(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<uint>(input);
            Assert.IsInstanceOfType(o1, typeof(uint));
            Assert.AreEqual((uint)12, o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToUInt64(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<ulong>(input);
            Assert.IsInstanceOfType(o1, typeof(ulong));
            Assert.AreEqual((ulong)12, o1);
        }


        [DataTestMethod]
        [DataRow("10/02/2020")]
        [DataRow("2020/02/10")]
        [DataRow("2020-02-10")]
        [DataRow("10-02-2020")]
        [DataRow(637168896000000000)]
        public void Convert_ToDateTime(object input)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
            var o1 = SyncTypeConverter.TryConvertTo<DateTime>(input, cultureInfo);
            Assert.IsInstanceOfType(o1, typeof(DateTime));
            Assert.AreEqual(new DateTime(2020, 02, 10), o1);
        }

        [DataTestMethod]
        [DataRow("10/02/2020")]
        [DataRow("2020/02/10")]
        [DataRow("2020-02-10")]
        [DataRow("10-02-2020")]
        [DataRow(637168896000000000)]
        public void Convert_ToDateTimeOffset(object input)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
            var ti = new DateTimeOffset(new DateTime(2020, 02, 10)).Ticks;

            var o1 = SyncTypeConverter.TryConvertTo<DateTimeOffset>(input, cultureInfo);
            Assert.IsInstanceOfType(o1, typeof(DateTimeOffset));
            Assert.AreEqual(new DateTimeOffset(new DateTime(2020, 02, 10)), o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToByte(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<byte>(input);
            Assert.IsInstanceOfType(o1, typeof(byte));
            Assert.AreEqual(12, o1);
        }

        [DataTestMethod]
        [DataRow("12")]
        [DataRow(12)]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToSByte(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<sbyte>(input);
            Assert.IsInstanceOfType(o1, typeof(sbyte));
            Assert.AreEqual(12, o1);
        }

        [DataTestMethod]
        [DataRow("true")]
        [DataRow("TRUE")]
        [DataRow("True")]
        [DataRow("tRue")]
        [DataRow("1")]
        [DataRow(1)]
        [DataRow((float)1.0)]
        [DataRow(1.0)]
        [DataRow(1)]
        [DataRow(true)]
        public void Convert_ToBoolean_True(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<bool>(input);
            Assert.IsInstanceOfType(o1, typeof(bool));
            Assert.IsTrue(o1);
        }


        [DataTestMethod]
        [DataRow("false")]
        [DataRow("FALSE")]
        [DataRow("False")]
        [DataRow("faLse")]
        [DataRow("0")]
        [DataRow(0)]
        [DataRow(0.0)]
        [DataRow((float)0.0)]
        [DataRow(0)]
        [DataRow(false)]
        public void Convert_ToBoolean_False(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<bool>(input);
            Assert.IsInstanceOfType(o1, typeof(bool));
            Assert.IsFalse(o1);
        }


        [DataTestMethod]
        [DataRow("DDB67AC3-89DF-430E-AD65-CBE691D237D8")]
        [DataRow("ddb67ac3-89df-430e-ad65-cbe691d237d8")]
        public void Convert_ToGuid(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<Guid>(input);
            Assert.IsInstanceOfType(o1, typeof(Guid));
            Assert.AreEqual(new Guid("ddb67ac3-89df-430e-ad65-cbe691d237d8"), o1);
        }

        [TestMethod]
        public void Convert_Byte_ArrayToGuid()
        {
            var bytearray = new Guid("ddb67ac3-89df-430e-ad65-cbe691d237d8").ToByteArray();

            var o1 = SyncTypeConverter.TryConvertTo<Guid>(bytearray);
            Assert.IsInstanceOfType(o1, typeof(Guid));
            Assert.AreEqual(new Guid("ddb67ac3-89df-430e-ad65-cbe691d237d8"), o1);
        }

        [DataTestMethod]
        [DataRow("a")]
        [DataRow('a')]
        public void Convert_ToChar(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<char>(input);
            Assert.IsInstanceOfType(o1, typeof(char));
            Assert.AreEqual('a', o1);
        }

        [DataTestMethod]
        [DataRow("12.177")]
        [DataRow(12.177)]
        [DataRow((float)12.177)]
        public void Convert_ToDecimal_Invariant(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<decimal>(input);
            Assert.IsInstanceOfType(o1, typeof(decimal));
            Assert.AreEqual((decimal)12.177, o1);
        }

        [DataTestMethod]
        [DataRow("12.177")]
        public void Convert_ToDecimal_OtherCulture(object input)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");

            var o1 = SyncTypeConverter.TryConvertTo<decimal>(input, cultureInfo);
            Assert.IsInstanceOfType(o1, typeof(decimal));
            Assert.AreEqual((decimal)12.177, o1);

            SyncGlobalization.DataSourceNumberDecimalSeparator = ",";
            var o2 = SyncTypeConverter.TryConvertTo<decimal>(input);
            Assert.IsInstanceOfType(o2, typeof(decimal));
            Assert.AreEqual((decimal)12.177, o2);
            SyncGlobalization.DataSourceNumberDecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
        }

        [DataTestMethod]
        [DataRow("12.177")]
        [DataRow(12.177)]
        public void Convert_ToDouble(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<double>(input);
            Assert.IsInstanceOfType(o1, typeof(double));
            Assert.AreEqual((double)12.177, o1);
        }

        [DataTestMethod]
        [DataRow("12,177")]
        public void Convert_ToDouble_WithNfi(object input)
        {

            var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");

            var o1 = SyncTypeConverter.TryConvertTo<double>(input, cultureInfo);
            Assert.IsInstanceOfType(o1, typeof(double));
            Assert.AreEqual((double)12.177, o1);

            var o2 = SyncTypeConverter.TryConvertTo<double>(input, cultureInfo);
            Assert.IsInstanceOfType(o2, typeof(double));
            Assert.AreEqual((double)12.177, o2);
        }
        [DataTestMethod]
        [DataRow("12.177")]
        [DataRow(12.177d)]
        [DataRow((float)12.177)]
        public void Convert_ToFloat(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<float>(input);
            Assert.IsInstanceOfType(o1, typeof(float));
            Assert.AreEqual((float)12.177, o1);
        }

        [DataTestMethod]
        [DataRow("12.177")]
        public void Convert_ToFloat_OtherCulture(object input)
        {

            var cultureInfo = CultureInfo.GetCultureInfo("en-US");

            var o1 = SyncTypeConverter.TryConvertTo<float>(input, cultureInfo);
            Assert.IsInstanceOfType(o1, typeof(float));
            Assert.AreEqual((float)12.177, o1);

            SyncGlobalization.DataSourceNumberDecimalSeparator = ",";
            var o2 = SyncTypeConverter.TryConvertTo<float>(input);
            Assert.IsInstanceOfType(o2, typeof(float));
            Assert.AreEqual((float)12.177, o2);
            SyncGlobalization.DataSourceNumberDecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
        }

        [DataTestMethod]
        [DataRow("00:00:00.0100000")]
        [DataRow(100000)]
        public void Convert_ToTimeSpan(object input)
        {
            var ts = new TimeSpan(100000);

            var o1 = SyncTypeConverter.TryConvertTo<TimeSpan>(input);
            Assert.IsInstanceOfType(o1, typeof(TimeSpan));
            Assert.AreEqual(ts, o1);
        }

        [TestMethod]
        public void Convert_Base64String_ToByteArray()
        {
            var s = "I'm a drummer";
            var sByt = Encoding.UTF8.GetBytes(s);
            var sBase64 = Convert.ToBase64String(sByt);

            var o1 = SyncTypeConverter.TryConvertTo<byte[]>(sBase64);
            Assert.IsInstanceOfType(o1, typeof(byte[]));
            CollectionAssert.AreEqual(sByt, o1);

            var b = Encoding.UTF8.GetString(o1);
            Assert.AreEqual(s, b);
        }

        [DataTestMethod]
        [DataRow(100000)]
        [DataRow(true)]
        [DataRow(false)]
        [DataRow(12.43)]
        [DataRow((float)12.43)]
        public void Convert_ToByteArray(object input)
        {
            var o1 = SyncTypeConverter.TryConvertTo<byte[]>(input);
            var expected = BitConverter.GetBytes((dynamic)input);
            CollectionAssert.AreEqual(expected, o1);
        }
    }
}
