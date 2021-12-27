using ISynergy.Framework.Synchronization.Core.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISynergy.Framework.Synchronization.Core.Tests.Base
{
    public abstract class BaseDataContext<TContext> : DbContext
        where TContext : DbContext
    {
        protected string _connectionString;

        public bool UseSchema { get; protected set; } = false;
        public bool UseSeeding { get; protected set; } = true;

        protected BaseDataContext()
            : base()
        {
        }

        protected BaseDataContext(DbContextOptions<TContext> options)
            : base(options)
        {
        }

        protected BaseDataContext(string connectionString, bool useSeeding)
            : this()
        {
            _connectionString = connectionString;

            UseSeeding = useSeeding;
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddress { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAddress> EmployeeAddress { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductModel> ProductModel { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
        public virtual DbSet<SalesOrderHeader> SalesOrderHeader { get; set; }
        public virtual DbSet<Posts> Posts { get; set; }
        public virtual DbSet<PostTag> PostTag { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<PriceList> PricesList { get; set; }
        public virtual DbSet<PriceListDetail> PricesListDetail { get; set; }
        public virtual DbSet<PriceListCategory> PricesListCategory { get; set; }

        /// <summary>
        /// Need to specify all default values
        /// See https://github.com/aspnet/EntityFrameworkCore/issues/13206 for current issue
        /// </summary>
        protected void OnSeeding(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().HasData(
                new Address { AddressId = 1, AddressLine1 = "8713 Yosemite Ct.", AddressLine2 = "Appt 1", City = "Bothell", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98011" },
                new Address { AddressId = 2, AddressLine1 = "1318 Lasalle Street", AddressLine2 = "Appt 2", City = "Bothell", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98011" },
                new Address { AddressId = 3, AddressLine1 = "9178 Jumping St.", AddressLine2 = "Appt 3", City = "Dallas", StateProvince = "Texas", CountryRegion = "United States", PostalCode = "75201" },
                new Address { AddressId = 4, AddressLine1 = "9228 Via Del Sol", AddressLine2 = "Appt 4", City = "Phoenix", StateProvince = "Arizona", CountryRegion = "United States", PostalCode = "85004" },
                new Address { AddressId = 5, AddressLine1 = "26910 Indela Road", AddressLine2 = "Appt 5", City = "Montreal", StateProvince = "Quebec", CountryRegion = "Canada", PostalCode = "H1Y 2H5" },
                new Address { AddressId = 6, AddressLine1 = "2681 Eagle Peak", AddressLine2 = "Appt 6", City = "Bellevue", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98004" },
                new Address { AddressId = 7, AddressLine1 = "7943 Walnut Ave", AddressLine2 = "Appt 7", City = "Renton", StateProvince = "Washington", CountryRegion = "United States", PostalCode = "98055" },
                new Address { AddressId = 8, AddressLine1 = "6388 Lake City Way", AddressLine2 = "Appt 8", City = "Burnaby", StateProvince = "British Columbia", CountryRegion = "Canada", PostalCode = "V5A 3A6" },
                new Address { AddressId = 9, AddressLine1 = "52560 Free Street", AddressLine2 = "Appt 9", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V7" },
                new Address { AddressId = 10, AddressLine1 = "22580 Free Street", AddressLine2 = "Appt 10", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V7" },
                new Address { AddressId = 11, AddressLine1 = "2575 Bloor Street East", AddressLine2 = "Appt 11", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V6" },
                new Address { AddressId = 12, AddressLine1 = "Station E", AddressLine2 = "Appt 12", City = "Chalk Riber", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "K0J 1J0" },
                new Address { AddressId = 13, AddressLine1 = "575 Rue St Amable", AddressLine2 = "Appt 13", City = "Quebec", StateProvince = "Quebec", CountryRegion = "Canada", PostalCode = "G1R" },
                new Address { AddressId = 14, AddressLine1 = "2512-4th Ave Sw", AddressLine2 = "Appt 14", City = "Calgary", StateProvince = "Alberta", CountryRegion = "Canada", PostalCode = "T2P 2G8" },
                new Address { AddressId = 15, AddressLine1 = "55 Lakeshore Blvd East", AddressLine2 = "Appt 15", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M4B 1V6" },
                new Address { AddressId = 16, AddressLine1 = "6333 Cote Vertu", AddressLine2 = "Appt 16", City = "Montreal", StateProvince = "Quebec", CountryRegion = "Canada", PostalCode = "H1Y 2H5" },
                new Address { AddressId = 17, AddressLine1 = "3255 Front Street West", AddressLine2 = "Appt 17", City = "Toronto", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "H1Y 2H5" },
                new Address { AddressId = 18, AddressLine1 = "2550 Signet Drive", AddressLine2 = "Appt 18", City = "Weston", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "H1Y 2H7" },
                new Address { AddressId = 19, AddressLine1 = "6777 Kingsway", AddressLine2 = "Appt 19", City = "Burnaby", StateProvince = "British Columbia", CountryRegion = "Canada", PostalCode = "H1Y 2H8" },
                new Address { AddressId = 20, AddressLine1 = "5250-505 Burning St", AddressLine2 = "Appt 20", City = "Vancouver", StateProvince = "British Columbia", CountryRegion = "Canada", PostalCode = "H1Y 2H9" },
                new Address { AddressId = 21, AddressLine1 = "600 Slater Street", AddressLine2 = "Appt 21", City = "Ottawa", StateProvince = "Ontario", CountryRegion = "Canada", PostalCode = "M9V 4W3" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, FirstName = "Pamela", LastName = "Orson" },
                new Employee { EmployeeId = 2, FirstName = "David", LastName = "Kandle" },
                new Employee { EmployeeId = 3, FirstName = "Jillian", LastName = "Jon" }
            );

            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            var customerId3 = Guid.NewGuid();
            var customerId4 = Guid.NewGuid();

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = customerId1, EmployeeId = 1, NameStyle = false, Title = "Mr.", FirstName = "Orlando", MiddleName = "N.", LastName = "Gee", CompanyName = "A Bike Store", SalesPerson = @"adventure-works\pamela0", EmailAddress = "orlando0@adventure-works.com", Phone = "245-555-0173", PasswordHash = "L/Rlwxzp4w7RWmEgXX+/A7cXaePEPcp+KwQhl2fJL7w=", PasswordSalt = "1KjXYs4=" },
                new Customer { CustomerId = customerId2, EmployeeId = 1, NameStyle = false, Title = "Mr.", FirstName = "Keith", MiddleName = "N.", LastName = "Harris", CompanyName = "Progressive Sports", SalesPerson = @"adventure-works\david8", EmailAddress = "keith0@adventure-works.com", Phone = "170-555-0127", PasswordHash = "YPdtRdvqeAhj6wyxEsFdshBDNXxkCXn+CRgbvJItknw=", PasswordSalt = "fs1ZGhY=" },
                new Customer { CustomerId = customerId3, EmployeeId = 2, NameStyle = false, Title = "Ms.", FirstName = "Donna", MiddleName = "F.", LastName = "Carreras", CompanyName = "Advanced Bike Components", SalesPerson = @"adventure-works\jillian0", EmailAddress = "donna0@adventure-works.com", Phone = "279-555-0130", PasswordHash = "LNoK27abGQo48gGue3EBV/UrlYSToV0/s87dCRV7uJk=", PasswordSalt = "YTNH5Rw=" },
                new Customer { CustomerId = customerId4, EmployeeId = 3, NameStyle = false, Title = "Ms.", FirstName = "Janet", MiddleName = "M.", LastName = "Gates", CompanyName = "Modular Cycle Systems", SalesPerson = @"adventure-works\jillian0", EmailAddress = "janet1@adventure-works.com", Phone = "710-555-0173", PasswordHash = "ElzTpSNbUW1Ut+L5cWlfR7MF6nBZia8WpmGaQPjLOJA=", PasswordSalt = "nm7D5e4=" }
            );

            modelBuilder.Entity<EmployeeAddress>().HasData(
                new EmployeeAddress { EmployeeId = 1, AddressId = 6, AddressType = "Home" },
                new EmployeeAddress { EmployeeId = 2, AddressId = 7, AddressType = "Home" },
                new EmployeeAddress { EmployeeId = 3, AddressId = 8, AddressType = "Home" }
            );

            modelBuilder.Entity<CustomerAddress>().HasData(
                new CustomerAddress { CustomerId = customerId1, AddressId = 4, AddressType = "Main Office" },
                new CustomerAddress { CustomerId = customerId1, AddressId = 5, AddressType = "Office Depot" },
                new CustomerAddress { CustomerId = customerId2, AddressId = 3, AddressType = "Main Office" },
                new CustomerAddress { CustomerId = customerId3, AddressId = 2, AddressType = "Main Office" },
                new CustomerAddress { CustomerId = customerId4, AddressId = 1, AddressType = "Main Office" }
            );

            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { ProductCategoryId = "_BIKES", Name = "Bikes" },
                new ProductCategory { ProductCategoryId = "_COMPT", Name = "Components" },
                new ProductCategory { ProductCategoryId = "_CLOTHE", Name = "Clothing" },
                new ProductCategory { ProductCategoryId = "_ACCESS", Name = "Accessories" },
                new ProductCategory { ProductCategoryId = "MOUNTB", Name = "Mountain Bikes" },
                new ProductCategory { ProductCategoryId = "ROADB", Name = "Road Bikes" },
                new ProductCategory { ProductCategoryId = "ROADFR", Name = "Road Frames" },
                new ProductCategory { ProductCategoryId = "TOURB", Name = "Touring Bikes" },
                new ProductCategory { ProductCategoryId = "HANDLB", Name = "Handlebars" },
                new ProductCategory { ProductCategoryId = "BRACK", Name = "Bottom Brackets" },
                new ProductCategory { ProductCategoryId = "BRAKES", Name = "Brakes" }

            );

            modelBuilder.Entity<ProductModel>().HasData(
                new ProductModel { ProductModelId = 6, Name = "HL Road Frame" },
                new ProductModel { ProductModelId = 19, Name = "Mountain-100", CatalogDescription = @"
                        <?xml-stylesheet href=""ProductDescription.xsl"" type=""text/xsl""?><p1:ProductDescription xmlns:p1=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelDescription"" xmlns:wm=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain"" xmlns:wf=""http://www.adventure-works.com/schemas/OtherFeatures"" xmlns:html=""http://www.w3.org/1999/xhtml"" ProductModelID=""19"" ProductModelName=""Mountain 100""><p1:Summary><html:p>Our top-of-the-line competition mountain bike. 
                        Performance-enhancing options include the innovative HL Frame,
                        super-smooth front suspension, and traction for all terrain.
                        </html:p></p1:Summary><p1:Manufacturer><p1:Name>AdventureWorks</p1:Name><p1:Copyright>2002</p1:Copyright><p1:ProductURL>HTTP://www.Adventure-works.com</p1:ProductURL></p1:Manufacturer><p1:Features>These are the product highlights. 
                        <wm:Warranty><wm:WarrantyPeriod>3 years</wm:WarrantyPeriod><wm:Description>parts and labor</wm:Description></wm:Warranty><wm:Maintenance><wm:NoOfYears>10 years</wm:NoOfYears><wm:Description>maintenance contract available through your dealer or any AdventureWorks retail store.</wm:Description></wm:Maintenance><wf:wheel>High performance wheels.</wf:wheel><wf:saddle><html:i>Anatomic design</html:i> and made from durable leather for a full-day of riding in comfort.</wf:saddle><wf:pedal><html:b>Top-of-the-line</html:b> clipless pedals with adjustable tension.</wf:pedal><wf:BikeFrame>Each frame is hand-crafted in our Bothell facility to the optimum diameter
                        and wall-thickness required of a premium mountain frame.
                        The heat-treated welded aluminum frame has a larger diameter tube that absorbs the bumps.</wf:BikeFrame><wf:crankset> Triple crankset; alumunim crank arm; flawless shifting. </wf:crankset></p1:Features><!-- add one or more of these elements...one for each specific product in this product model --><p1:Picture><p1:Angle>front</p1:Angle><p1:Size>small</p1:Size><p1:ProductPhotoID>118</p1:ProductPhotoID></p1:Picture><!-- add any tags in <specifications> --><p1:Specifications> These are the product specifications.
                        <Material>Almuminum Alloy</Material><Color>Available in most colors</Color><ProductLine>Mountain bike</ProductLine><Style>Unisex</Style><RiderExperience>Advanced to Professional riders</RiderExperience></p1:Specifications></p1:ProductDescription>
                " },
                new ProductModel { ProductModelId = 20, Name = "Mountain-200" },
                new ProductModel { ProductModelId = 21, Name = "Mountain-300" },
                new ProductModel { ProductModelId = 25, Name = "Road-150", CatalogDescription = @"
                        <?xml-stylesheet href=""ProductDescription.xsl"" type=""text/xsl""?><p1:ProductDescription xmlns:p1=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelDescription"" xmlns:wm=""http://schemas.microsoft.com/sqlserver/2004/07/adventure-works/ProductModelWarrAndMain"" xmlns:wf=""http://www.adventure-works.com/schemas/OtherFeatures"" xmlns:html=""http://www.w3.org/1999/xhtml"" ProductModelID=""25"" ProductModelName=""Road-150""><p1:Summary><html:p>This bike is ridden by race winners. Developed with the 
                        Adventure Works Cycles professional race team, it has a extremely light
                        heat-treated aluminum frame, and steering that allows precision control.
                        </html:p></p1:Summary><p1:Manufacturer><p1:Name>AdventureWorks</p1:Name><p1:Copyright>2002</p1:Copyright><p1:ProductURL>HTTP://www.Adventure-works.com</p1:ProductURL></p1:Manufacturer><p1:Features>These are the product highlights. 
                        <wm:Warranty><wm:WarrantyPeriod>4 years</wm:WarrantyPeriod><wm:Description>parts and labor</wm:Description></wm:Warranty><wm:Maintenance><wm:NoOfYears>7 years</wm:NoOfYears><wm:Description>maintenance contact available through dealer or any Adventure Works Cycles retail store.</wm:Description></wm:Maintenance><wf:handlebar>Designed for racers; high-end anatomically shaped bar from aluminum alloy.</wf:handlebar><wf:wheel>Strong wheels with double-walled rims.</wf:wheel><wf:saddle><html:i>Lightweight</html:i> kevlar racing saddle.</wf:saddle><wf:pedal><html:b>Top-of-the-line</html:b> clipless pedals with adjustable tension.</wf:pedal><wf:BikeFrame><html:i>Our lightest and best quality</html:i> aluminum frame made from the newest alloy;
                        it is welded and heat-treated for strength.
                        Our innovative design results in maximum comfort and performance.</wf:BikeFrame></p1:Features><!-- add one or more of these elements...one for each specific product in this product model --><p1:Picture><p1:Angle>front</p1:Angle><p1:Size>small</p1:Size><p1:ProductPhotoID>126</p1:ProductPhotoID></p1:Picture><!-- add any tags in <specifications> --><p1:Specifications> These are the product specifications.
                        <Material>Aluminum</Material><Color>Available in all colors.</Color><ProductLine>Road bike</ProductLine><Style>Unisex</Style><RiderExperience>Intermediate to Professional riders</RiderExperience></p1:Specifications></p1:ProductDescription>
                " },
                new ProductModel { ProductModelId = 30, Name = "Road-650" },
                new ProductModel { ProductModelId = 52, Name = "LL Mountain Handlebars" },
                new ProductModel { ProductModelId = 54, Name = "ML Mountain Handlebars" },
                new ProductModel { ProductModelId = 55, Name = "HL Mountain Handlebars" }
            );


            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();

            var products = new List<Product>();
            //for (var i = 0; i < 2000; i++)
            //{
            //    products.Add(
            //        new Product
            //        {
            //            ProductId = Guid.NewGuid(),
            //            Name = $"Generated N° {i.ToString()}",
            //            ProductNumber = $"FR-{i.ToString()}",
            //            Color = "Black",
            //            StandardCost = 1059.3100M,
            //            ListPrice = 1431.5000M,
            //            Size = "58",
            //            Weight = 1016.04M,
            //            ProductCategoryId = "ROADFR",
            //            ProductModelId = 6
            //        }
            //    );
            //}

            products.AddRange(new[] {
                new Product { ProductId = Guid.NewGuid(), Name = "HL Road Frame - Black, 58", ProductNumber = "FR-R92B-58", Color = "Black", StandardCost = 1059.3100M, ListPrice = 1431.5000M, Size = "58", Weight = 1016.04M, ProductCategoryId = "ROADFR", ProductModelId = 6 },
                new Product { ProductId = p1, Name = "HL Road Frame - Red, 58", ProductNumber = "FR-R92R-58", Color = "Red", StandardCost = 1059.3100M, ListPrice = 1431.5000M, Size = "58", Weight = 1016.04M, ProductCategoryId = "ROADFR", ProductModelId = 6 },
                new Product { ProductId = p2, Name = "Road-150 Red, 62", ProductNumber = "BK-R93R-62", Color = "Red", StandardCost = 2171.2942M, ListPrice = 3578.2700M, Size = "62", Weight = 6803.85M, ProductCategoryId = "ROADB", ProductModelId = 25 },
                new Product { ProductId = Guid.NewGuid(), Name = "Road-650 Black, 58", ProductNumber = "BK-R50B-58", Color = "Black", StandardCost = 486.7066M, ListPrice = 782.9900M, Size = "58", Weight = 8976.55M, ProductCategoryId = "ROADB", ProductModelId = 30 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-100 Silver, 38", ProductNumber = "BK-M82S-38", Color = "Silver", StandardCost = 1912.1544M, ListPrice = 3399.9900M, Size = "38", Weight = 9230.56M, ProductCategoryId = "MOUNTB", ProductModelId = 19 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-100 Black, 38", ProductNumber = "BK-M82B-38", Color = "Black", StandardCost = 1898.0944M, ListPrice = 3374.9900M, Size = "38", Weight = 9230.56M, ProductCategoryId = "MOUNTB", ProductModelId = 19 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Silver, 38", ProductNumber = "BK-M68S-38", Color = "Silver", StandardCost = 1265.6195M, ListPrice = 2319.9900M, Size = "38", Weight = 10591.33M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Black, 38", ProductNumber = "BK-M68B-38", Color = "Black", StandardCost = 1251.9813M, ListPrice = 2294.9900M, Size = "38", Weight = 10591.33M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Black, 42", ProductNumber = "BK-M68B-42", Color = "Black", StandardCost = 1251.9813M, ListPrice = 2294.9900M, Size = "42", Weight = 10781.83M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-200 Black, 46", ProductNumber = "BK-M68B-46", Color = "Black", StandardCost = 1251.9813M, ListPrice = 2294.9900M, Size = "46", Weight = 10945.13M, ProductCategoryId = "MOUNTB", ProductModelId = 20 },
                new Product { ProductId = Guid.NewGuid(), Name = "Mountain-300 Black, 38", ProductNumber = "BK-M47B-38", Color = "Black", StandardCost = 598.4354M, ListPrice = 1079.9900M, Size = "38", Weight = 11498.51M, ProductCategoryId = "MOUNTB", ProductModelId = 21 },
                new Product { ProductId = p3, Name = "LL Mountain Handlebars", ProductNumber = "HB-M243", StandardCost = 19.7758M, ListPrice = 44.5400M, ProductCategoryId = "HANDLB", ProductModelId = 52 },
                new Product { ProductId = Guid.NewGuid(), Name = "ML Mountain Handlebars", ProductNumber = "HB-M763", StandardCost = 27.4925M, ListPrice = 61.9200M, ProductCategoryId = "HANDLB", ProductModelId = 54 },
                new Product { ProductId = Guid.NewGuid(), Name = "HL Mountain Handlebars", ProductNumber = "HB-M918", StandardCost = 53.3999M, ListPrice = 120.2700M, ProductCategoryId = "HANDLB", ProductModelId = 55 }
            });


            modelBuilder.Entity<Product>()
                .HasData(products);

            modelBuilder.Entity<SalesOrderHeader>().HasData(
                new SalesOrderHeader
                {
                    SalesOrderId = 1000,
                    SalesOrderNumber = "SO-1000",
                    RevisionNumber = 1,
                    Status = 5,
                    OnlineOrderFlag = true,
                    PurchaseOrderNumber = "PO348186287",
                    AccountNumber = "10-4020-000609",
                    CustomerId = customerId1,
                    ShipToAddressId = 4,
                    BillToAddressId = 5,
                    ShipMethod = "CAR TRANSPORTATION",
                    SubTotal = 6530.35M,
                    TaxAmt = 70.4279M,
                    Freight = 22.0087M,
                    TotalDue = 6530.35M + 70.4279M + 22.0087M
                }
            );

            modelBuilder.Entity<SalesOrderDetail>().HasData(
                new SalesOrderDetail { SalesOrderId = 1000, SalesOrderDetailId = 110562, OrderQty = 1, ProductId = p2, UnitPrice = 3578.2700M },
                new SalesOrderDetail { SalesOrderId = 1000, SalesOrderDetailId = 110563, OrderQty = 2, ProductId = p3, UnitPrice = 44.5400M },
                new SalesOrderDetail { SalesOrderId = 1000, SalesOrderDetailId = 110564, OrderQty = 2, ProductId = p1, UnitPrice = 1431.5000M }
            );

            modelBuilder.Entity<Posts>().HasData(
                new Posts { PostId = 1, Title = "Best Boutiques on the Eastside" },
                new Posts { PostId = 2, Title = "Avoiding over-priced helmets" },
                new Posts { PostId = 3, Title = "Where to buy Mars Bars" }
            );

            modelBuilder.Entity<Tags>().HasData(
                new Tags { TagId = 1, Text = "Golden" },
                new Tags { TagId = 2, Text = "Pineapple" },
                new Tags { TagId = 3, Text = "Girlscout" },
                new Tags { TagId = 4, Text = "Cookies" }
            );

            modelBuilder.Entity<PostTag>().HasData(
                new PostTag { PostId = 1, TagId = 1 },
                new PostTag { PostId = 1, TagId = 2 },
                new PostTag { PostId = 1, TagId = 3 },
                new PostTag { PostId = 2, TagId = 1 },
                new PostTag { PostId = 2, TagId = 4 },
                new PostTag { PostId = 3, TagId = 3 },
                new PostTag { PostId = 3, TagId = 4 }
            );

            var hollydayPriceListId = new Guid("944563b4-1f40-4218-b896-7fcb71674f43");
            var dalyPriceListId = new Guid("de60f9fb-7d4f-489a-9aae-2a7f7e4a5f0a");
            decimal[] discountlist = { 5, 10, 30, 50 };

            modelBuilder.Entity<PriceList>(entity =>
            {
                entity.HasData(
                    new PriceList() { PriceListId = dalyPriceListId, Description = "Daly price list" },
                    new PriceList() { PriceListId = hollydayPriceListId, Description = "Hollyday price list" }
                    );

            });

            modelBuilder.Entity<PriceListCategory>()
                .HasData(new PriceListCategory() { PriceListId = hollydayPriceListId, PriceCategoryId = "_BIKES" }
                    , new PriceListCategory() { PriceListId = hollydayPriceListId, PriceCategoryId = "_CLOTHE", }
                    , new PriceListCategory() { PriceListId = dalyPriceListId, PriceCategoryId = "_BIKES", }
                    , new PriceListCategory() { PriceListId = dalyPriceListId, PriceCategoryId = "_CLOTHE", }
                    , new PriceListCategory() { PriceListId = dalyPriceListId, PriceCategoryId = "_COMPT", }
                    );


            var dettails = new List<PriceListDetail>();
            var generator = new Random((int)DateTime.Now.Ticks);
            //Add hollyday price list
            dettails.AddRange(products
                .Where(p => p.ProductCategoryId == "MOUNTB")
                .Select(item => new PriceListDetail()
                {
                    PriceListId = hollydayPriceListId,
                    PriceCategoryId = "_BIKES",
                    PriceListDettailId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductDescription = $"{item.Name}(Easter {DateTime.Now.Year})",
                    MinQuantity = generator.Next(0, 5),
                    Amount = item.ListPrice.HasValue ? item.ListPrice.Value : 0,
                    Discount = discountlist[generator.Next(0, discountlist.Length - 1)],
                }));

            dettails.AddRange(products
                .Where(p => p.ProductCategoryId == "_CLOTHE")
                .Select(item => new PriceListDetail()
                {
                    PriceListId = hollydayPriceListId,
                    PriceCategoryId = "_CLOTHE",
                    PriceListDettailId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductDescription = $"{item.Name}(Easter {DateTime.Now.Year})",
                    MinQuantity = generator.Next(0, 5),
                    Amount = item.ListPrice.HasValue ? item.ListPrice.Value : 0,
                    Discount = discountlist[generator.Next(0, discountlist.Length - 1)],
                }));

            //Add standard price list
            dettails.AddRange(products
                .Where(p => p.ProductCategoryId == "MOUNTB")
                .Select(item => new PriceListDetail()
                {
                    PriceListId = dalyPriceListId,
                    PriceCategoryId = "_BIKES",
                    PriceListDettailId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductDescription = item.Name,
                    MinQuantity = generator.Next(0, 5),
                    Amount = item.ListPrice.HasValue ? item.ListPrice.Value : 0,
                }));

            dettails.AddRange(products
                .Where(p => p.ProductCategoryId == "_CLOTHE")
                .Select(item => new PriceListDetail()
                {
                    PriceListId = dalyPriceListId,
                    PriceCategoryId = "_CLOTHE",
                    PriceListDettailId = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductDescription = item.Name,
                    MinQuantity = generator.Next(0, 5),
                    Amount = item.ListPrice.HasValue ? item.ListPrice.Value : 0,
                }));

            modelBuilder.Entity<PriceListDetail>().HasData(dettails.ToArray());

        }

    }

#if NET6_0 

    public class MyModelCacheKeyFactory<TContext> : IModelCacheKeyFactory
        where TContext : BaseDataContext<DbContext>
    {
        public object Create(TContext context, bool designTime)
        {
            var adventureWorksContext = context;
            var useSchema = adventureWorksContext.UseSchema;
            var useSeeding = adventureWorksContext.UseSeeding;

            var hashCode = base.GetHashCode() * 397;
            hashCode ^= useSchema.GetHashCode();
            hashCode ^= useSeeding.GetHashCode();

            return (hashCode, designTime);
        }
    }
#else
    internal class MyModelCacheKeyFactory<TContext> : IModelCacheKeyFactory
    {
        public object Create(TContext context)
            => new MyModelCacheKey(context);
    }

#endif

    internal class MyModelCacheKey<TContext> : ModelCacheKey
        where TContext : BaseDataContext<DbContext>
    {
        private readonly bool useSchema;
        private readonly bool useSeeding;

        public MyModelCacheKey(TContext context)
            : base(context)
        {

            var adventureWorksContext = context;
            useSchema = adventureWorksContext.UseSchema;
            useSeeding = adventureWorksContext.UseSeeding;
        }

        protected override bool Equals(ModelCacheKey other)
        {
            var otherModel = other as MyModelCacheKey<TContext>;
            var isequal = base.Equals(other)
                && otherModel?.useSchema == useSchema
                && otherModel?.useSeeding == useSeeding;

            return isequal;
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode() * 397;
            hashCode ^= useSchema.GetHashCode();
            hashCode ^= useSeeding.GetHashCode();

            return hashCode;
        }
    }
}
