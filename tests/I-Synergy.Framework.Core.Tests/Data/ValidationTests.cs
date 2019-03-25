using ISynergy.Fixtures;
using ISynergy.Framework.Tests.Base;
using System;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace ISynergy.Data.Tests
{
    [Category("Validation")]
    public class ValidationTests : UnitTest
    {
        [Fact]
        public void ValidateIsNullTest()
        {
            var model = new ModelFixture<string>
            {
                Value = null
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (string.IsNullOrEmpty(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Value is null");
                }
            });

            Assert.False(model.Validate());
        }

        [Fact]
        public void ValidateIsNotNullTest()
        {
            var model = new ModelFixture<string>
            {
                Value = "192.168.1.0"
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (string.IsNullOrEmpty(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Value is null");
                }
            });

            Assert.True(model.Validate());
        }

        [Fact]
        public void ValidateIsNotInRangeTest()
        {
            var model = new ModelFixture<int>
            {
                Value = 9999
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<int>;

                if (!Enumerable.Range(1, 100).Contains(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Portnumber is not in range [1,100]");
                }
            });

            Assert.False(model.Validate());
        }

        [Fact]
        public void ValidateIsInRangeTest()
        {
            var model = new ModelFixture<int>
            {
                Value = 80
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<int>;

                if (!Enumerable.Range(1, 500).Contains(i.Value))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("Portnumber is not in range [1,100]");
                }
            });

            Assert.True(model.Validate());
        }

        [Fact]
        public void ValidateStringLengthIsNotInRangeTest()
        {
            var model = new ModelFixture<string>
            {
                Value = ""
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (!Enumerable.Range(1, 35).Contains(i.Value.Length))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("String should have length of 1-35 characters");
                }
            });

            Assert.False(model.Validate());
        }

        [Fact]
        public void ValidateStringLengthIsInRangeTest()
        {
            var model = new ModelFixture<string>
            {
                Value = "192.168.1.0"
            };

            model.Validator = new Action<IObservableClass>(arg =>
            {
                var i = arg as ModelFixture<string>;

                if (!Enumerable.Range(1, 35).Contains(i.Value.Length))
                {
                    i.Properties[nameof(i.Value)].Errors.Add("String should have length of 1-35 characters");
                }
            });

            Assert.True(model.Validate());
        }
    }
}
