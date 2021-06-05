using ISynergy.Framework.Core.Linq.Abstractions;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Entities;
using ISynergy.Framework.Core.Linq.Parsers;
using Moq;
using NFluent;
using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests.Parser
{
    /// <summary>
    /// Class TypeFinderTests.
    /// </summary>
    [TestClass]
    public class TypeFinderTests
    {
        /// <summary>
        /// The parsing configuration
        /// </summary>
        private readonly ParsingConfig _parsingConfig = new ParsingConfig();
        /// <summary>
        /// The keywords helper mock
        /// </summary>
        private readonly Mock<IKeywordsHelper> _keywordsHelperMock;
        /// <summary>
        /// The dynamic type provider mock
        /// </summary>
        private readonly Mock<IDynamicLinkCustomTypeProvider> _dynamicTypeProviderMock;

        /// <summary>
        /// The sut
        /// </summary>
        private readonly TypeFinder _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFinderTests"/> class.
        /// </summary>
        public TypeFinderTests()
        {
            _dynamicTypeProviderMock = new Mock<IDynamicLinkCustomTypeProvider>();
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(BaseEmployee).FullName)).Returns(typeof(BaseEmployee));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Boss).FullName)).Returns(typeof(Boss));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Worker).FullName)).Returns(typeof(Worker));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("Boss")).Returns(typeof(Boss));

            _parsingConfig = new ParsingConfig
            {
                CustomTypeProvider = _dynamicTypeProviderMock.Object
            };

            _keywordsHelperMock = new Mock<IKeywordsHelper>();

            _sut = new TypeFinder(_parsingConfig, _keywordsHelperMock.Object);
        }

        /// <summary>
        /// Defines the test method TypeFinder_FindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_equals_false.
        /// </summary>
        [TestMethod]
        public void TypeFinder_FindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_equals_false()
        {
            // Assign
            _parsingConfig.ResolveTypesBySimpleName = true;

            // Act
            Type result = _sut.FindTypeByName("Boss", null, forceUseCustomTypeProvider: false);

            // Assert
            Check.That(result).IsNull();
        }

        /// <summary>
        /// Defines the test method TypeFinder_FindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_equals_true.
        /// </summary>
        [TestMethod]
        public void TypeFinder_FindTypeByName_With_SimpleTypeName_forceUseCustomTypeProvider_equals_true()
        {
            // Assign
            _parsingConfig.ResolveTypesBySimpleName = true;

            // Act
            Type result = _sut.FindTypeByName("Boss", null, forceUseCustomTypeProvider: true);

            // Assert
            Check.That(result).Equals(typeof(Boss));
        }

        /// <summary>
        /// Defines the test method TypeFinder_FindTypeByName_With_SimpleTypeName_basedon_it.
        /// </summary>
        [TestMethod]
        public void TypeFinder_FindTypeByName_With_SimpleTypeName_basedon_it()
        {
            // Assign
            _parsingConfig.ResolveTypesBySimpleName = true;
            var expressions = new[] { Expression.Parameter(typeof(BaseEmployee)) };

            // Act
            Type result = _sut.FindTypeByName("Boss", expressions, forceUseCustomTypeProvider: false);

            // Assert
            Check.That(result).Equals(typeof(Boss));
        }
    }
}
