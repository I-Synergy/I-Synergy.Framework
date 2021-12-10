using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Helpers;
using Moq;
using System;
using System.Data.SqlClient;

namespace ISynergy.Framework.Synchronization.SqlServer.Tests.Base
{
    public abstract class BaseTest
    {
        internal readonly DatabaseHelper _databaseHelper;
        protected readonly IVersionService _versionService;

        public string[] Tables => new string[]
        {
            "SalesLT.ProductCategory", "SalesLT.ProductModel", "SalesLT.Product", "Employee", "Customer", "Address", "CustomerAddress", "EmployeeAddress",
            "SalesLT.SalesOrderHeader", "SalesLT.SalesOrderDetail",  "Posts", "Tags", "PostTag",
            "PricesList", "PricesListCategory", "PricesListDetail"
        };

        public BaseTest()
        {
            _databaseHelper = new DatabaseHelper();

            var versionServiceMock = new Mock<IVersionService>();
            versionServiceMock.Setup(x => x.ProductVersion).Returns(new Version(1, 0, 0));
            _versionService = versionServiceMock.Object;

            // Since we are creating a lot of databases
            // each database will have its own pool
            // Droping database will not clear the pool associated
            // So clear the pools on every start of a new test
            SqlConnection.ClearAllPools();
        }
    }
}
