using ISynergy.Framework.Synchronization.Core.Providers;
using ISynergy.Framework.Synchronization.Core.Tests.Models;
using ISynergy.Framework.Synchronization.SqlServer.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using ISynergy.Framework.Synchronization.Core.Tests.Base;
using ISynergy.Framework.Synchronization.Core.Enumerations;

namespace ISynergy.Framework.Synchronization.SqlServer.Tests.Context
{
    public class DataContext : BaseDataContext<DataContext>
    {
        private readonly ProviderType _providerType;

        public DataContext()
        {
        }

        public DataContext((string DatabaseName, CoreProvider provider) t, bool fallbackUseSchema = true, bool useSeeding = false)
            : this(new DatabaseHelper().GetConnectionString(t.DatabaseName), useSeeding)
        {
            UseSchema = fallbackUseSchema;
        }

        public DataContext((string DatabaseName, ProviderType ProviderType, CoreProvider provider) t, bool fallbackUseSchema = true, bool useSeeding = false)
            : this(new DatabaseHelper().GetConnectionString(t.DatabaseName), useSeeding)
        {
            _providerType = t.ProviderType;

            UseSchema = fallbackUseSchema;
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DataContext(string connectionString, bool useSeeding)
            : base(connectionString, useSeeding)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasIndex(e => e.StateProvince);

                entity.HasIndex(e => new { e.City, e.StateProvince, e.PostalCode, e.CountryRegion });

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.AddressLine1)
                    .IsRequired();

                entity.Property(e => e.City)
                    .HasMaxLength(30);

                entity.Property(e => e.CountryRegion)
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PostalCode)
                    .IsUnicode()
                    .HasMaxLength(15);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.StateProvince)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.EmailAddress);

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID");

                entity.Property(e => e.CustomerId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CompanyName).HasMaxLength(128);

                entity.Property(e => e.EmailAddress).HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                // Creating a column with space in it
                entity.Property(e => e.AttributeWithSpace)
                    .HasColumnName("Attribute With Space");

                entity.Property(e => e.MiddleName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");


                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(25);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SalesPerson).HasMaxLength(256);

                entity.Property(e => e.Suffix).HasMaxLength(10);

                entity.Property(e => e.Title).HasMaxLength(8);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");


            });

            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.AddressId });

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.AddressType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerAddress)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EmployeeAddress>(entity =>
            {
                entity.HasKey(e => new { e.EmployeeId, e.AddressId });

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID");

                entity.Property(e => e.AddressId).HasColumnName("AddressID");

                entity.Property(e => e.AddressType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.EmployeeAddress)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAddress)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Oid);

                entity.Property(e => e.Oid).ValueGeneratedNever();

                entity.Property(e => e.ErrorDescription).HasMaxLength(50);

                entity.Property(e => e.Gcrecord).HasColumnName("GCRecord");

                entity.Property(e => e.Operation).HasMaxLength(50);

                entity.Property(e => e.TimeStampDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                if (UseSchema)
                    entity.ToTable("Product", "SalesLT");

                entity.HasKey(e => e.ProductId);

                entity.HasIndex(e => e.Name)
#if NET5_0 || NET6_0 || NETCOREAPP3_1
                    .HasDatabaseName("AK_Product_Name")
#elif NETCOREAPP2_1
                    .HasDatabaseName("AK_Product_Name")
#endif
                    .IsUnique();

                entity.HasIndex(e => e.ProductNumber)
#if NET5_0 || NET6_0 || NETCOREAPP3_1
                    .HasDatabaseName("AK_Product_ProductNumber")
#elif NETCOREAPP2_1
                    .HasDatabaseName("AK_Product_ProductNumber")
#endif
                    .IsUnique();

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Color).HasMaxLength(15);

                entity.Property(e => e.DiscontinuedDate).HasColumnType("datetime");

                entity.Property(e => e.ListPrice).HasColumnType("money");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");


                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("ProductCategoryID")
                    .HasMaxLength(12);

                entity.Property(e => e.ProductModelId).HasColumnName("ProductModelID");

                entity.Property(e => e.ProductNumber)
                    .HasMaxLength(25);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SellEndDate).HasColumnType("datetime");

                entity.Property(e => e.SellStartDate).HasColumnType("datetime");

                entity.Property(e => e.Size).HasMaxLength(5);

                entity.Property(e => e.StandardCost).HasColumnType("money");

                entity.Property(e => e.ThumbnailPhotoFileName).HasMaxLength(50);

                entity.Property(e => e.Weight).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductCategoryId);

                entity.HasOne(d => d.ProductModel)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductModelId);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                if (UseSchema)
                    entity.ToTable("ProductCategory", "SalesLT");

                entity.HasKey(e => e.ProductCategoryId);


                entity.HasIndex(e => e.Name)
#if NET5_0 || NET6_0 || NETCOREAPP3_1
                    .HasDatabaseName("AK_ProductCategory_Name")
#elif NETCOREAPP2_1
                    .HasDatabaseName("AK_ProductCategory_Name")
#endif
                    .IsUnique();

                entity.Property(e => e.ProductCategoryId)
                    .HasColumnName("ProductCategoryID")
                    .HasMaxLength(12)
                    .ValueGeneratedNever();

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("Name")
                    .HasMaxLength(50);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                // Creating a column with space in it, and a schema on the table
                entity.Property(e => e.AttributeWithSpace)
                    .HasColumnName("Attribute With Space");


                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                if (UseSchema)
                    entity.ToTable("ProductModel", "SalesLT");

                entity.HasIndex(e => e.Name)
#if NET5_0 || NET6_0 || NETCOREAPP3_1
                    .HasDatabaseName("AK_ProductModel_Name")
#elif NETCOREAPP2_1
                    .HasDatabaseName("AK_ProductModel_Name")
#endif
                    .IsUnique();

                entity.Property(e => e.ProductModelId).HasColumnName("ProductModelID");

                //if (this.ProviderType == ProviderType.Sql)
                //    entity.Property(e => e.CatalogDescription).HasColumnType("xml");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<SalesOrderDetail>(entity =>
            {
                if (UseSchema)
                    entity.ToTable("SalesOrderDetail", "SalesLT");

                entity.HasKey(e => new { e.SalesOrderDetailId });

                entity.HasIndex(e => e.ProductId);

                entity.Property(e => e.SalesOrderId)
                    .HasColumnName("SalesOrderID")
                    .ValueGeneratedNever();

                entity.Property(e => e.SalesOrderDetailId)
                    .HasColumnName("SalesOrderDetailID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.LineTotal).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ProductId)
                    .HasColumnName("ProductID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UnitPriceDiscount).HasColumnType("money");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SalesOrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SalesOrder)
                    .WithMany(p => p.SalesOrderDetail)
                    .HasForeignKey(d => d.SalesOrderId);
            });

            modelBuilder.Entity<SalesOrderHeader>(entity =>
            {
                if (UseSchema)
                    entity.ToTable("SalesOrderHeader", "SalesLT");

                entity.HasKey(e => e.SalesOrderId);

                entity.HasIndex(e => e.CustomerId);

                entity.Property(e => e.SalesOrderId).HasColumnName("SalesOrderID");

                entity.Property(e => e.AccountNumber)
                    .HasColumnName("AccountNumber")
                    .HasMaxLength(15);

                entity.Property(e => e.BillToAddressId).HasColumnName("BillToAddressID");

                entity.Property(e => e.CreditCardApprovalCode)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Freight).HasColumnType("money");

                entity.Property(e => e.Freight).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.OnlineOrderFlag)
                    .IsRequired()
                    .HasColumnType("bit");

                entity.Property(e => e.OnlineOrderFlag).HasDefaultValueSql("((1))");

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PurchaseOrderNumber)
                    .HasMaxLength(25);

                entity.Property(e => e.Rowguid)
                    .HasColumnName("rowguid");

                entity.Property(e => e.Rowguid).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SalesOrderNumber)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.SalesOrderNumber).HasDefaultValueSql("(('SO-XXXX'))");


                entity.Property(e => e.ShipDate).HasColumnType("datetime");

                entity.Property(e => e.ShipMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShipToAddressId).HasColumnName("ShipToAddressID");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.SubTotal).HasColumnType("money");

                entity.Property(e => e.SubTotal).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.TaxAmt).HasColumnType("money");

                entity.Property(e => e.TaxAmt).HasDefaultValueSql("((0.00))");

                entity.Property(e => e.TotalDue).HasColumnType("money");

                entity.Property(e => e.TotalDue).HasDefaultValueSql("((0.00))");

                entity.HasOne(d => d.BillToAddress)
                    .WithMany(p => p.SalesOrderHeaderBillToAddress)
                    .HasForeignKey(d => d.BillToAddressId)
                    .HasConstraintName("FK_SalesOrderHeader_Address_BillTo_AddressID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.SalesOrderHeader)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ShipToAddress)
                    .WithMany(p => p.SalesOrderHeaderShipToAddress)
                    .HasForeignKey(d => d.ShipToAddressId)
                    .HasConstraintName("FK_SalesOrderHeader_Address_ShipTo_AddressID");
            });

            modelBuilder.Entity<Posts>(entity =>
            {
                entity.HasKey(e => e.PostId);
            });

            modelBuilder.Entity<PostTag>(entity =>
            {
                entity.HasKey(e => new { e.PostId, e.TagId });

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostTag)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTag_Posts");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.PostTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostTag_Tags");
            });

            modelBuilder.Entity<Tags>(entity =>
            {
                entity.HasKey(e => e.TagId);
            });

            modelBuilder.Entity<PriceListDetail>(entity =>
            {
                entity.HasKey(d => new
                {
                    d.PriceListId,
                    d.PriceCategoryId,
                    d.PriceListDettailId,
                });

                entity.HasOne(d => d.Category)
                    .WithMany(c => c.Details);

                entity.Property(d => d.ProductId)
                    .IsRequired();

                entity.Property(d => d.ProductDescription)
                    .HasMaxLength(50)
                    .IsUnicode()
                    .IsRequired();
            });

            modelBuilder.Entity<PriceListCategory>(entity =>
            {
                entity.HasKey(c => new { c.PriceListId, c.PriceCategoryId });

                entity.HasOne(c => c.PriceList)
                    .WithMany(p => p.Categories);
            });

            modelBuilder.Entity<PriceList>(entity =>
            {
                entity.HasKey(p => p.PriceListId);

                entity.Property(p => p.Description)
                    .IsRequired()
                    .IsUnicode()
                    .HasMaxLength(50);
            });

            if (UseSeeding)
                this.OnSeeding(modelBuilder);
        }
    }
}
