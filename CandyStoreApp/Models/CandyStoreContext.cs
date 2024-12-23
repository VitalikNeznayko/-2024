using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CandyStoreApp.Models;

public partial class CandyStoreContext : DbContext
{
    public CandyStoreContext()
    {
    }

    public CandyStoreContext(DbContextOptions<CandyStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }
    public DbSet<CategorySalesByRating> CategorySalesByRating { get; set; }
    public DbSet<ProductSalesByRating> ProductSalesByRating { get; set; }
    public DbSet<TopSoldProductByCategory> TopSoldProductByCategory { get; set; }
    public DbSet<DetailedRevenueByDateRange> DetailedRevenueByDateRange { get; set; }
    public DbSet<ClientTotalIncome> ClientTotalIncome { get; set; }
    public DbSet<OrderWithHighestAverageCheck> OrderWithHighestAverageCheck { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=VITALIK\\MSSQLSERVER01;Database=candy_store;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("PK__Categori__E548B673BC00E2F5");

            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient).HasName("PK__Clients__6EC2B6C086A70002");

            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.AddressClient)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("address_client");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Email)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("postal_code");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("PK__Orders__DD5B8F3FFB3FCFC5");

            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("order_status");
            entity.Property(e => e.TotalCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_cost");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("FK__Orders__id_clien__3B75D760");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItem).HasName("PK__Order_It__2453F0126E8668AD");

            entity.ToTable("Order_Items", tb =>
                {
                    tb.HasTrigger("CalculateOrderTotal");
                    tb.HasTrigger("CheckProductStockBeforeOrder");
                    tb.HasTrigger("UpdateProductStockAfterPurchase");
                });

            entity.Property(e => e.IdOrderItem).HasColumnName("id_order_item");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdOrder)
                .HasConstraintName("FK__Order_Ite__total__4D94879B");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK__Order_Ite__id_pr__4E88ABD4");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__BA39E84FC03FCDD3");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.IdSupplier).HasColumnName("id_supplier");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductDescription)
                .HasColumnType("text")
                .HasColumnName("product_description");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdCategory)
                .HasConstraintName("FK__Products__produc__45F365D3");

            entity.HasOne(d => d.IdSupplierNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdSupplier)
                .HasConstraintName("FK__Products__id_sup__46E78A0C");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Reviews__2F79F8C7C4CB1EE6");

            entity.Property(e => e.IdReview).HasColumnName("id_review");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate)
                .HasColumnType("datetime")
                .HasColumnName("review_date");
            entity.Property(e => e.ReviewText)
                .HasColumnType("text")
                .HasColumnName("review_text");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.IdClient)
                .HasConstraintName("FK__Reviews__id_clie__49C3F6B7");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK__Reviews__id_prod__4AB81AF0");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.IdShipment).HasName("PK__Shipment__C7B896422296DC22");

            entity.Property(e => e.IdShipment).HasColumnName("id_shipment");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.ShipmentDate)
                .HasColumnType("datetime")
                .HasColumnName("shipment_date");
            entity.Property(e => e.ShipmentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shipment_method");
            entity.Property(e => e.TrackingNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("tracking_number");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.IdOrder)
                .HasConstraintName("FK__Shipments__id_or__3E52440B");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.IdSupplier).HasName("PK__Supplier__F6C576E67992EF49");

            entity.Property(e => e.IdSupplier).HasColumnName("id_supplier");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contact_person");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.SupplierAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("supplier_address");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("supplier_name");
        });
        modelBuilder.Entity<CategorySalesByRating>().HasNoKey();
        modelBuilder.Entity<ProductSalesByRating>().HasNoKey();
        modelBuilder.Entity<TopSoldProductByCategory>().HasNoKey();
        modelBuilder.Entity<DetailedRevenueByDateRange>().HasNoKey();
        modelBuilder.Entity<ClientTotalIncome>().HasNoKey();
        modelBuilder.Entity<OrderWithHighestAverageCheck>().HasNoKey();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
