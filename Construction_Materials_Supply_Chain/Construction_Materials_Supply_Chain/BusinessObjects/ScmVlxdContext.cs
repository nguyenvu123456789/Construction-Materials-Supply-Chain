using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects;

public partial class ScmVlxdContext : DbContext
{
    public ScmVlxdContext()
    {
    }

    public ScmVlxdContext(DbContextOptions<ScmVlxdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<DebtReport> DebtReports { get; set; }

    public virtual DbSet<FinancialReport> FinancialReports { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<Kpi> Kpis { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<ShippingLog> ShippingLogs { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Transport> Transports { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Activity__5E5499A894C0E299");

            entity.ToTable("ActivityLog");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EntityId).HasColumnName("EntityID");
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ActivityL__UserI__7C4F7684");
        });

        modelBuilder.Entity<DebtReport>(entity =>
        {
            entity.HasKey(e => e.DebtReportId).HasName("PK__DebtRepo__4221A738FA82C6CD");

            entity.ToTable("DebtReport");

            entity.Property(e => e.DebtReportId).HasColumnName("DebtReportID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<FinancialReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Financia__D5BD48E5A4B815DC");

            entity.ToTable("FinancialReport");

            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReportType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__F5FDE6D3731CCD3C");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("InventoryID");
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasDefaultValue(0);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__5AEE82B9");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__59FA5E80");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAD5C6273FC0");

            entity.ToTable("Invoice");

            entity.HasIndex(e => e.InvoiceNumber, "UQ__Invoice__D776E98185E86C00").IsUnique();

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
            entity.Property(e => e.InvoiceType).HasMaxLength(50);
            entity.Property(e => e.IssueDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RelatedOrderId).HasColumnName("RelatedOrderID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InvoiceCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Invoice__Created__6D0D32F4");

            entity.HasOne(d => d.Customer).WithMany(p => p.InvoiceCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Invoice__Custome__6B24EA82");

            entity.HasOne(d => d.RelatedOrder).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.RelatedOrderId)
                .HasConstraintName("FK__Invoice__Related__6A30C649");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__Invoice__Supplie__6C190EBB");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F1578F1246305C4");

            entity.ToTable("InvoiceDetail");

            entity.Property(e => e.InvoiceDetailId).HasColumnName("InvoiceDetailID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InvoiceDe__Invoi__6FE99F9F");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InvoiceDe__Produ__70DDC3D8");
        });

        modelBuilder.Entity<Kpi>(entity =>
        {
            entity.HasKey(e => e.Kpiid).HasName("PK__KPI__72E6928111C3E191");

            entity.ToTable("KPI");

            entity.Property(e => e.Kpiid).HasColumnName("KPIID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Kpiname)
                .HasMaxLength(100)
                .HasColumnName("KPIName");
            entity.Property(e => e.TargetValue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32681B6EAC");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__787EE5A0");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAF0731C5D2");

            entity.ToTable("Order");

            entity.HasIndex(e => e.OrderNumber, "UQ__Order__CAC5E743BD8032BF").IsUnique();

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Order__CreatedBy__5FB337D6");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C3B014C4D");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__628FA481");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__6383C8BA");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB0F3117DF6B");

            entity.ToTable("Permission");

            entity.HasIndex(e => e.PermissionName, "UQ__Permissi__0FFDA357954640BC").IsUnique();

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PermissionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED9105ABE6");

            entity.ToTable("Product");

            entity.HasIndex(e => e.Sku, "UQ__Product__CA1ECF0DFE149FC4").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A47D5EE45");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B61602BEC257F").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK__Role_Per__120F469A85ACD2C4");

            entity.ToTable("Role_Permission");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ__Role_Per__6400A18BF5DF240B").IsUnique();

            entity.Property(e => e.RolePermissionId).HasColumnName("RolePermissionID");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Role_Perm__Permi__49C3F6B7");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Role_Perm__RoleI__48CFD27E");
        });

        modelBuilder.Entity<ShippingLog>(entity =>
        {
            entity.HasKey(e => e.ShippingLogId).HasName("PK__Shipping__2A7E450D5152A772");

            entity.ToTable("ShippingLog");

            entity.Property(e => e.ShippingLogId).HasColumnName("ShippingLogID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TransportId).HasColumnName("TransportID");

            entity.HasOne(d => d.Order).WithMany(p => p.ShippingLogs)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__ShippingL__Order__07C12930");

            entity.HasOne(d => d.Transport).WithMany(p => p.ShippingLogs)
                .HasForeignKey(d => d.TransportId)
                .HasConstraintName("FK__ShippingL__Trans__08B54D69");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE66694260B829E");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        modelBuilder.Entity<Transport>(entity =>
        {
            entity.HasKey(e => e.TransportId).HasName("PK__Transpor__19E9A17D730EE80D");

            entity.ToTable("Transport");

            entity.Property(e => e.TransportId).HasColumnName("TransportID");
            entity.Property(e => e.Driver).HasMaxLength(100);
            entity.Property(e => e.Porter).HasMaxLength(100);
            entity.Property(e => e.Route).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Vehicle).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACDB2E31D3");

            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ__User__C9F28456BAF9FD7E").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__User_Rol__3D978A5554942671");

            entity.ToTable("User_Role");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ__User_Rol__AF27604EEE534ECC").IsUnique();

            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Role__RoleI__440B1D61");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Role__UserI__4316F928");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("PK__Vendor__FC8618D3CCF2E9A2");

            entity.ToTable("Vendor");

            entity.Property(e => e.VendorId).HasColumnName("VendorID");
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.VendorName).HasMaxLength(100);
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFD9BEBFF0D6");

            entity.ToTable("Warehouse");

            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.WarehouseName).HasMaxLength(100);

            entity.HasOne(d => d.Manager).WithMany(p => p.Warehouses)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Warehouse__Manag__5165187F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
