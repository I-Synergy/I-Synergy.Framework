using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Synchronization.Migrations
{
    public partial class _0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    City = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StateProvince = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountryRegion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 275, DateTimeKind.Local).AddTicks(6290))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressID);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    NameStyle = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: ""),
                    CompanyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SalesPerson = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    PasswordSalt = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 275, DateTimeKind.Local).AddTicks(8235))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.CustomerID);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    ProductCategoryID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 276, DateTimeKind.Local).AddTicks(9704))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.ProductCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "ProductModel",
                columns: table => new
                {
                    ProductModelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CatalogDescription = table.Column<string>(type: "xml", nullable: true),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 277, DateTimeKind.Local).AddTicks(978))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModel", x => x.ProductModelID);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddress",
                columns: table => new
                {
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddressID = table.Column<int>(type: "int", nullable: false),
                    AddressType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 276, DateTimeKind.Local).AddTicks(1237))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddress", x => new { x.CustomerID, x.AddressID });
                    table.ForeignKey(
                        name: "FK_CustomerAddress_Address_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Address",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_CustomerAddress_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderHeader",
                columns: table => new
                {
                    SalesOrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RevisionNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ShipDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "((1))"),
                    OnlineOrderFlag = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    SalesOrderNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false, defaultValueSql: "(('SO-XXXX'))"),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CustomerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipToAddressID = table.Column<int>(type: "int", nullable: false),
                    BillToAddressID = table.Column<int>(type: "int", nullable: false),
                    ShipMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreditCardApprovalCode = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    SubTotal = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))"),
                    TaxAmt = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))"),
                    Freight = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))"),
                    TotalDue = table.Column<decimal>(type: "money", nullable: false, defaultValueSql: "((0.00))"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 277, DateTimeKind.Local).AddTicks(7540))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderHeader", x => x.SalesOrderID);
                    table.ForeignKey(
                        name: "FK_SalesOrderHeader_Address_BillTo_AddressID",
                        column: x => x.BillToAddressID,
                        principalTable: "Address",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_SalesOrderHeader_Address_ShipTo_AddressID",
                        column: x => x.ShipToAddressID,
                        principalTable: "Address",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_SalesOrderHeader_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    StandardCost = table.Column<decimal>(type: "money", nullable: false),
                    ListPrice = table.Column<decimal>(type: "money", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    ProductCategoryID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ProductModelID = table.Column<int>(type: "int", nullable: false),
                    SellStartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SellEndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DiscontinuedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ThumbNailPhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ThumbnailPhotoFileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 276, DateTimeKind.Local).AddTicks(5637))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory_ProductCategoryID",
                        column: x => x.ProductCategoryID,
                        principalTable: "ProductCategory",
                        principalColumn: "ProductCategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductModel_ProductModelID",
                        column: x => x.ProductModelID,
                        principalTable: "ProductModel",
                        principalColumn: "ProductModelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderDetail",
                columns: table => new
                {
                    SalesOrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesOrderID = table.Column<int>(type: "int", nullable: false),
                    OrderQty = table.Column<short>(type: "smallint", nullable: false),
                    ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false),
                    UnitPriceDiscount = table.Column<decimal>(type: "money", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(38,6)", nullable: true),
                    rowguid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, defaultValueSql: "(newid())"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValue: new DateTime(2021, 12, 5, 22, 10, 11, 277, DateTimeKind.Local).AddTicks(2348))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderDetail", x => x.SalesOrderDetailID);
                    table.ForeignKey(
                        name: "FK_SalesOrderDetail_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK_SalesOrderDetail_SalesOrderHeader_SalesOrderID",
                        column: x => x.SalesOrderID,
                        principalTable: "SalesOrderHeader",
                        principalColumn: "SalesOrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Address",
                columns: new[] { "AddressID", "AddressLine1", "City", "CountryRegion", "PostalCode", "StateProvince" },
                values: new object[,]
                {
                    { 1, "8713 Yosemite Ct.", "Bothell", "United States", "98011", "Washington" },
                    { 2, "1318 Lasalle Street", "Bothell", "United States", "98011", "Washington" },
                    { 3, "9178 Jumping St.", "Dallas", "United States", "75201", "Texas" },
                    { 4, "9228 Via Del Sol", "Phoenix", "United States", "85004", "Arizona" },
                    { 5, "26910 Indela Road", "Montreal", "Canada", "H1Y 2H5", "Quebec" },
                    { 6, "2681 Eagle Peak", "Bellevue", "United States", "98004", "Washington" },
                    { 7, "7943 Walnut Ave", "Renton", "United States", "98055", "Washington" },
                    { 8, "6388 Lake City Way", "Burnaby", "Canada", "V5A 3A6", "British Columbia" },
                    { 9, "52560 Free Street", "Toronto", "Canada", "M4B 1V7", "Ontario" },
                    { 10, "22580 Free Street", "Toronto", "Canada", "M4B 1V7", "Ontario" },
                    { 11, "2575 Bloor Street East", "Toronto", "Canada", "M4B 1V6", "Ontario" },
                    { 12, "Station E", "Chalk Riber", "Canada", "K0J 1J0", "Ontario" },
                    { 13, "575 Rue St Amable", "Quebec", "Canada", "G1R", "Quebec" },
                    { 14, "2512-4th Ave Sw", "Calgary", "Canada", "T2P 2G8", "Alberta" },
                    { 15, "55 Lakeshore Blvd East", "Toronto", "Canada", "M4B 1V6", "Ontario" },
                    { 16, "6333 Cote Vertu", "Montreal", "Canada", "H1Y 2H5", "Quebec" },
                    { 17, "3255 Front Street West", "Toronto", "Canada", "H1Y 2H5", "Ontario" },
                    { 18, "2550 Signet Drive", "Weston", "Canada", "H1Y 2H7", "Ontario" },
                    { 19, "6777 Kingsway", "Burnaby", "Canada", "H1Y 2H8", "British Columbia" },
                    { 20, "5250-505 Burning St", "Vancouver", "Canada", "H1Y 2H9", "British Columbia" },
                    { 21, "600 Slater Street", "Ottawa", "Canada", "M9V 4W3", "Ontario" }
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "CustomerID", "CompanyName", "EmailAddress", "EmployeeID", "FirstName", "LastName", "MiddleName", "NameStyle", "PasswordHash", "PasswordSalt", "Phone", "SalesPerson", "Title" },
                values: new object[,]
                {
                    { new Guid("370b78fe-c755-476e-8cff-06a99650af94"), "Progressive Sports", "keith0@adventure-works.com", 1, "Keith", "Harris", "N.", false, "YPdtRdvqeAhj6wyxEsFdshBDNXxkCXn+CRgbvJItknw=", "fs1ZGhY=", "170-555-0127", "adventure-works\\david8", "Mr." },
                    { new Guid("45672e0d-0456-4e9a-9492-d334f859e928"), "Advanced Bike Components", "donna0@adventure-works.com", 2, "Donna", "Carreras", "F.", false, "LNoK27abGQo48gGue3EBV/UrlYSToV0/s87dCRV7uJk=", "YTNH5Rw=", "279-555-0130", "adventure-works\\jillian0", "Ms." },
                    { new Guid("737e9052-54f5-464e-9f2e-f27ca5122ecf"), "A Bike Store", "orlando0@adventure-works.com", 1, "Orlando", "Gee", "N.", false, "L/Rlwxzp4w7RWmEgXX+/A7cXaePEPcp+KwQhl2fJL7w=", "1KjXYs4=", "245-555-0173", "adventure-works\\pamela0", "Mr." },
                    { new Guid("e4b1d888-53ec-4f47-9866-04c027204c47"), "Modular Cycle Systems", "janet1@adventure-works.com", 3, "Janet", "Gates", "M.", false, "ElzTpSNbUW1Ut+L5cWlfR7MF6nBZia8WpmGaQPjLOJA=", "nm7D5e4=", "710-555-0173", "adventure-works\\jillian0", "Ms." }
                });

            migrationBuilder.InsertData(
                table: "ProductCategory",
                columns: new[] { "ProductCategoryID", "Name" },
                values: new object[,]
                {
                    { "ACCESS", "Accessories" },
                    { "BIKES", "Bikes" },
                    { "BRACK", "Bottom Brackets" },
                    { "BRAKES", "Brakes" },
                    { "CLOTHE", "Clothing" },
                    { "COMPT", "Components" },
                    { "HANDLB", "Handlebars" },
                    { "MOUNTB", "Mountain Bikes" },
                    { "ROADB", "Road Bikes" },
                    { "ROADFR", "Road Frames" },
                    { "TOURB", "Touring Bikes" }
                });

            migrationBuilder.InsertData(
                table: "ProductModel",
                columns: new[] { "ProductModelID", "CatalogDescription", "Name" },
                values: new object[,]
                {
                    { 6, null, "HL Road Frame" },
                    { 19, "\n                        <?xml-stylesheet href=\"ProductDescription.xsl\" type=\"text/xsl\"?><p1:ProductDescription xmlns:p1=\"http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelDescription\" xmlns:wm=\"http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain\" xmlns:wf=\"http://www.adventure-works.com/schemas/OtherFeatures\" xmlns:html=\"http://www.w3.org/1999/xhtml\" ProductModelID=\"19\" ProductModelName=\"Mountain 100\"><p1:Summary><html:p>Our top-of-the-line competition mountain bike. \n                        Performance-enhancing options include the innovative HL Frame,\n                        super-smooth front suspension, and traction for all terrain.\n                        </html:p></p1:Summary><p1:Manufacturer><p1:Name>AdventureWorks</p1:Name><p1:Copyright>2002</p1:Copyright><p1:ProductURL>HTTP://www.Adventure-works.com</p1:ProductURL></p1:Manufacturer><p1:Features>These are the product highlights. \n                        <wm:Warranty><wm:WarrantyPeriod>3 years</wm:WarrantyPeriod><wm:Description>parts and labor</wm:Description></wm:Warranty><wm:Maintenance><wm:NoOfYears>10 years</wm:NoOfYears><wm:Description>maintenance contract available through your dealer or any AdventureWorks retail store.</wm:Description></wm:Maintenance><wf:wheel>High performance wheels.</wf:wheel><wf:saddle><html:i>Anatomic design</html:i> and made from durable leather for a full-day of riding in comfort.</wf:saddle><wf:pedal><html:b>Top-of-the-line</html:b> clipless pedals with adjustable tension.</wf:pedal><wf:BikeFrame>Each frame is hand-crafted in our Bothell facility to the optimum diameter\n                        and wall-thickness required of a premium mountain frame.\n                        The heat-treated welded aluminum frame has a larger diameter tube that absorbs the bumps.</wf:BikeFrame><wf:crankset> Triple crankset; alumunim crank arm; flawless shifting. </wf:crankset></p1:Features><!-- add one or more of these elements...one for each specific product in this product model --><p1:Picture><p1:Angle>front</p1:Angle><p1:Size>small</p1:Size><p1:ProductPhotoID>118</p1:ProductPhotoID></p1:Picture><!-- add any tags in <specifications> --><p1:Specifications> These are the product specifications.\n                        <Material>Almuminum Alloy</Material><Color>Available in most colors</Color><ProductLine>Mountain bike</ProductLine><Style>Unisex</Style><RiderExperience>Advanced to Professional riders</RiderExperience></p1:Specifications></p1:ProductDescription>\n                ", "Mountain-100" },
                    { 20, null, "Mountain-200" },
                    { 21, null, "Mountain-300" },
                    { 25, "\n                        <?xml-stylesheet href=\"ProductDescription.xsl\" type=\"text/xsl\"?><p1:ProductDescription xmlns:p1=\"http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelDescription\" xmlns:wm=\"http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain\" xmlns:wf=\"http://www.adventure-works.com/schemas/OtherFeatures\" xmlns:html=\"http://www.w3.org/1999/xhtml\" ProductModelID=\"25\" ProductModelName=\"Road-150\"><p1:Summary><html:p>This bike is ridden by race winners. Developed with the \n                        Adventure Works Cycles professional race team, it has a extremely light\n                        heat-treated aluminum frame, and steering that allows precision control.\n                        </html:p></p1:Summary><p1:Manufacturer><p1:Name>AdventureWorks</p1:Name><p1:Copyright>2002</p1:Copyright><p1:ProductURL>HTTP://www.Adventure-works.com</p1:ProductURL></p1:Manufacturer><p1:Features>These are the product highlights. \n                        <wm:Warranty><wm:WarrantyPeriod>4 years</wm:WarrantyPeriod><wm:Description>parts and labor</wm:Description></wm:Warranty><wm:Maintenance><wm:NoOfYears>7 years</wm:NoOfYears><wm:Description>maintenance contact available through dealer or any Adventure Works Cycles retail store.</wm:Description></wm:Maintenance><wf:handlebar>Designed for racers; high-end anatomically shaped bar from aluminum alloy.</wf:handlebar><wf:wheel>Strong wheels with double-walled rims.</wf:wheel><wf:saddle><html:i>Lightweight</html:i> kevlar racing saddle.</wf:saddle><wf:pedal><html:b>Top-of-the-line</html:b> clipless pedals with adjustable tension.</wf:pedal><wf:BikeFrame><html:i>Our lightest and best quality</html:i> aluminum frame made from the newest alloy;\n                        it is welded and heat-treated for strength.\n                        Our innovative design results in maximum comfort and performance.</wf:BikeFrame></p1:Features><!-- add one or more of these elements...one for each specific product in this product model --><p1:Picture><p1:Angle>front</p1:Angle><p1:Size>small</p1:Size><p1:ProductPhotoID>126</p1:ProductPhotoID></p1:Picture><!-- add any tags in <specifications> --><p1:Specifications> These are the product specifications.\n                        <Material>Aluminum</Material><Color>Available in all colors.</Color><ProductLine>Road bike</ProductLine><Style>Unisex</Style><RiderExperience>Intermediate to Professional riders</RiderExperience></p1:Specifications></p1:ProductDescription>\n                ", "Road-150" },
                    { 30, null, "Road-650" }
                });

            migrationBuilder.InsertData(
                table: "ProductModel",
                columns: new[] { "ProductModelID", "CatalogDescription", "Name" },
                values: new object[] { 52, null, "LL Mountain Handlebars" });

            migrationBuilder.InsertData(
                table: "ProductModel",
                columns: new[] { "ProductModelID", "CatalogDescription", "Name" },
                values: new object[] { 54, null, "ML Mountain Handlebars" });

            migrationBuilder.InsertData(
                table: "ProductModel",
                columns: new[] { "ProductModelID", "CatalogDescription", "Name" },
                values: new object[] { 55, null, "HL Mountain Handlebars" });

            migrationBuilder.InsertData(
                table: "CustomerAddress",
                columns: new[] { "AddressID", "CustomerID", "AddressType" },
                values: new object[,]
                {
                    { 3, new Guid("370b78fe-c755-476e-8cff-06a99650af94"), "Main Office" },
                    { 2, new Guid("45672e0d-0456-4e9a-9492-d334f859e928"), "Main Office" },
                    { 4, new Guid("737e9052-54f5-464e-9f2e-f27ca5122ecf"), "Main Office" },
                    { 5, new Guid("737e9052-54f5-464e-9f2e-f27ca5122ecf"), "Office Depot" },
                    { 1, new Guid("e4b1d888-53ec-4f47-9866-04c027204c47"), "Main Office" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductID", "Color", "DiscontinuedDate", "ListPrice", "Name", "ProductCategoryID", "ProductModelID", "ProductNumber", "SellEndDate", "SellStartDate", "Size", "StandardCost", "ThumbNailPhoto", "ThumbnailPhotoFileName", "Weight" },
                values: new object[,]
                {
                    { new Guid("1a783a33-2e55-4443-bd46-784fcd74e29c"), "Black", null, 2294.9900m, "Mountain-200 Black, 42", "MOUNTB", 20, "BK-M68B-42", null, null, "42", 1251.9813m, null, null, 10781.83m },
                    { new Guid("31946b9e-84ab-47f1-aa61-cd57c331b5cf"), "Black", null, 1079.9900m, "Mountain-300 Black, 38", "MOUNTB", 21, "BK-M47B-38", null, null, "38", 598.4354m, null, null, 11498.51m },
                    { new Guid("3f19b196-de78-40ff-84f8-43e93e083dc5"), "Silver", null, 3399.9900m, "Mountain-100 Silver, 38", "MOUNTB", 19, "BK-M82S-38", null, null, "38", 1912.1544m, null, null, 9230.56m },
                    { new Guid("4bf7022b-b33e-41ca-82fe-3fd23a7db1ad"), "Black", null, 782.9900m, "Road-650 Black, 58", "ROADB", 30, "BK-R50B-58", null, null, "58", 486.7066m, null, null, 8976.55m },
                    { new Guid("4ed503b7-9a33-4f5d-b04d-ff92a4e27ce2"), "Black", null, 2294.9900m, "Mountain-200 Black, 46", "MOUNTB", 20, "BK-M68B-46", null, null, "46", 1251.9813m, null, null, 10945.13m },
                    { new Guid("5a684535-7e87-4d89-9662-61e17dc0580b"), "Silver", null, 2319.9900m, "Mountain-200 Silver, 38", "MOUNTB", 20, "BK-M68S-38", null, null, "38", 1265.6195m, null, null, 10591.33m },
                    { new Guid("5f6fdd7b-d2ab-4557-9c4a-604ae994731f"), null, null, 44.5400m, "LL Mountain Handlebars", "HANDLB", 52, "HB-M243", null, null, null, 19.7758m, null, null, null },
                    { new Guid("713f6ba4-cec9-4060-94fc-4bff058887dc"), null, null, 61.9200m, "ML Mountain Handlebars", "HANDLB", 54, "HB-M763", null, null, null, 27.4925m, null, null, null },
                    { new Guid("82e9194d-ea3e-46e7-8c7d-144f12f3addc"), "Black", null, 3374.9900m, "Mountain-100 Black, 38", "MOUNTB", 19, "BK-M82B-38", null, null, "38", 1898.0944m, null, null, 9230.56m },
                    { new Guid("9e2a8668-bedb-42d5-b9c5-7637f632efad"), "Black", null, 1431.5000m, "HL Road Frame - Black, 58", "ROADFR", 6, "FR-R92B-58", null, null, "58", 1059.3100m, null, null, 1016.04m },
                    { new Guid("b25a6a50-f7bd-45f0-af93-2da697eed822"), "Red", null, 3578.2700m, "Road-150 Red, 62", "ROADB", 25, "BK-R93R-62", null, null, "62", 2171.2942m, null, null, 6803.85m },
                    { new Guid("b775a4de-c00e-4fa4-ba21-7cd084cfc382"), null, null, 120.2700m, "HL Mountain Handlebars", "HANDLB", 55, "HB-M918", null, null, null, 53.3999m, null, null, null },
                    { new Guid("bc148123-c7d1-4a39-9710-e4e9802902b9"), "Black", null, 2294.9900m, "Mountain-200 Black, 38", "MOUNTB", 20, "BK-M68B-38", null, null, "38", 1251.9813m, null, null, 10591.33m },
                    { new Guid("d060e823-0fe4-41fc-81e1-a823182a513e"), "Red", null, 1431.5000m, "HL Road Frame - Red, 58", "ROADFR", 6, "FR-R92R-58", null, null, "58", 1059.3100m, null, null, 1016.04m }
                });

            migrationBuilder.InsertData(
                table: "SalesOrderHeader",
                columns: new[] { "SalesOrderID", "AccountNumber", "BillToAddressID", "Comment", "CreditCardApprovalCode", "CustomerID", "DueDate", "Freight", "OnlineOrderFlag", "PurchaseOrderNumber", "RevisionNumber", "SalesOrderNumber", "ShipDate", "ShipMethod", "ShipToAddressID", "Status", "SubTotal", "TaxAmt", "TotalDue" },
                values: new object[] { 1000, "10-4020-000609", 5, null, null, new Guid("737e9052-54f5-464e-9f2e-f27ca5122ecf"), null, 22.0087m, true, "PO348186287", (byte)1, "SO-1000", null, "CAR TRANSPORTATION", 4, (byte)5, 6530.35m, 70.4279m, 6622.7866m });

            migrationBuilder.InsertData(
                table: "SalesOrderDetail",
                columns: new[] { "SalesOrderDetailID", "LineTotal", "OrderQty", "ProductID", "SalesOrderID", "UnitPrice", "UnitPriceDiscount" },
                values: new object[] { 110562, null, (short)1, new Guid("b25a6a50-f7bd-45f0-af93-2da697eed822"), 1000, 3578.2700m, 0m });

            migrationBuilder.InsertData(
                table: "SalesOrderDetail",
                columns: new[] { "SalesOrderDetailID", "LineTotal", "OrderQty", "ProductID", "SalesOrderID", "UnitPrice", "UnitPriceDiscount" },
                values: new object[] { 110563, null, (short)2, new Guid("5f6fdd7b-d2ab-4557-9c4a-604ae994731f"), 1000, 44.5400m, 0m });

            migrationBuilder.InsertData(
                table: "SalesOrderDetail",
                columns: new[] { "SalesOrderDetailID", "LineTotal", "OrderQty", "ProductID", "SalesOrderID", "UnitPrice", "UnitPriceDiscount" },
                values: new object[] { 110564, null, (short)2, new Guid("d060e823-0fe4-41fc-81e1-a823182a513e"), 1000, 1431.5000m, 0m });

            migrationBuilder.CreateIndex(
                name: "IX_Address_City_StateProvince_PostalCode_CountryRegion",
                table: "Address",
                columns: new[] { "City", "StateProvince", "PostalCode", "CountryRegion" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateProvince",
                table: "Address",
                column: "StateProvince");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_EmailAddress",
                table: "Customer",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddress_AddressID",
                table: "CustomerAddress",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "AK_Product_Name",
                table: "Product",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AK_Product_ProductNumber",
                table: "Product",
                column: "ProductNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCategoryID",
                table: "Product",
                column: "ProductCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductModelID",
                table: "Product",
                column: "ProductModelID");

            migrationBuilder.CreateIndex(
                name: "AK_ProductCategory_Name",
                table: "ProductCategory",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "AK_ProductModel_Name",
                table: "ProductModel",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetail_ProductID",
                table: "SalesOrderDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderDetail_SalesOrderID",
                table: "SalesOrderDetail",
                column: "SalesOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderHeader_BillToAddressID",
                table: "SalesOrderHeader",
                column: "BillToAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderHeader_CustomerID",
                table: "SalesOrderHeader",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderHeader_ShipToAddressID",
                table: "SalesOrderHeader",
                column: "ShipToAddressID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerAddress");

            migrationBuilder.DropTable(
                name: "SalesOrderDetail");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "SalesOrderHeader");

            migrationBuilder.DropTable(
                name: "ProductCategory");

            migrationBuilder.DropTable(
                name: "ProductModel");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
