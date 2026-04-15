using Microsoft.EntityFrameworkCore;
using SistemaVentas.Persistence.Entities.Database;

namespace SistemaVentas.Persistence.Context
{
    public class SistemaVentasDbContext : DbContext
    {
        public SistemaVentasDbContext(DbContextOptions<SistemaVentasDbContext> options) : base(options) { }


        public DbSet<CountryDb> Countries => Set<CountryDb>();
        public DbSet<CityDb> Cities => Set<CityDb>();
        public DbSet<CategoryDb> Categories => Set<CategoryDb>();
        public DbSet<OrderStatusDb> OrderStatus => Set<OrderStatusDb>();
        public DbSet<CustomerDb> Customers => Set<CustomerDb>();
        public DbSet<ProductDb> Products => Set<ProductDb>();
        public DbSet<OrderDb> Orders => Set<OrderDb>();
        public DbSet<OrderDetailsDb> OrderDetails => Set<OrderDetailsDb>();


        // Configuración de tablas, llaves y relaciones
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configuración de Countries
            modelBuilder.Entity<CountryDb>(entity =>
            {
                entity.ToTable("Countries");
                entity.HasKey(e => e.CountryID);

                entity.Property(e => e.Country)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(e => e.Country)
                      .IsUnique();
            });


            // Configuración de Cities
            modelBuilder.Entity<CityDb>(entity =>
            {
                entity.ToTable("Cities");
                entity.HasKey(e => e.CityID);

                entity.Property(e => e.City)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(e => new { e.City, e.CountryID })
                      .IsUnique();

                entity.HasOne(e => e.CountryNavigation)
                      .WithMany(c => c.Cities)
                      .HasForeignKey(e => e.CountryID);
            });


            // Configuración de Categories
            modelBuilder.Entity<CategoryDb>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.CategoryID);

                entity.Property(e => e.Category)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(e => e.Category)
                      .IsUnique();
            });


            // Configuración de OrderStatus
            modelBuilder.Entity<OrderStatusDb>(entity =>
            {
                entity.ToTable("OrderStatus");
                entity.HasKey(e => e.StatusID);

                entity.Property(e => e.Status)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasIndex(e => e.Status)
                      .IsUnique();
            });


            // Configuración de Customers
            modelBuilder.Entity<CustomerDb>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(e => e.CustomerID);

                entity.Property(e => e.FirstName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.LastName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Email)
                      .HasMaxLength(255);

                entity.Property(e => e.Phone)
                      .HasMaxLength(50);

                entity.HasOne(e => e.CityNavigation)
                      .WithMany(c => c.Customers)
                      .HasForeignKey(e => e.CityID)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // Configuración de Products
            modelBuilder.Entity<ProductDb>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.ProductID);

                entity.Property(e => e.ProductName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.Price)
                      .HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.CategoryNavigation)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryID)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // Configuración de Orders
            modelBuilder.Entity<OrderDb>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(e => e.OrderID);

                entity.Property(e => e.OrderDate)
                      .HasColumnType("date");

                entity.HasOne(e => e.CustomerNavigation)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(e => e.CustomerID);

                entity.HasOne(e => e.StatusNavigation)
                      .WithMany(s => s.Orders)
                      .HasForeignKey(e => e.StatusID);
            });


            // Configuración de OrderDetails
            modelBuilder.Entity<OrderDetailsDb>(entity =>
            {
                entity.ToTable("OrderDetails");
                entity.HasKey(e => e.OrderDetailsID);

                entity.Property(e => e.UnitPrice)
                      .HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.OrderNavigation)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(e => e.OrderID);

                entity.HasOne(e => e.ProductNavigation)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(e => e.ProductID);
            });
        }
    }
}