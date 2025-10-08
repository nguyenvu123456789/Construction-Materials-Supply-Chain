using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

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
    public virtual DbSet<Category> Categories { get; set; }
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
    public virtual DbSet<Material> Materials { get; set; }
    public virtual DbSet<MaterialCheck> MaterialChecks { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<ShippingLog> ShippingLogs { get; set; }
    public virtual DbSet<Partner> Partners { get; set; }
    public virtual DbSet<PartnerType> PartnerTypes { get; set; }
    public virtual DbSet<Transport> Transports { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
    public virtual DbSet<Warehouse> Warehouses { get; set; }
    public virtual DbSet<Import> Imports { get; set; }
    public virtual DbSet<ImportDetail> ImportDetails { get; set; }
    public virtual DbSet<ImportReport> ImportReports { get; set; }
    public virtual DbSet<ImportReportDetail> ImportReportDetails { get; set; }
    public virtual DbSet<Export> Exports { get; set; }
    public virtual DbSet<ExportDetail> ExportDetails { get; set; }
    public virtual DbSet<ExportReport> ExportReports { get; set; }
    public virtual DbSet<ExportReportDetail> ExportReportDetails { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Import>(entity =>
        {
            entity.HasKey(e => e.ImportId);
            entity.ToTable("Import");

            entity.Property(e => e.ImportId).HasColumnName("ImportID");
            entity.Property(e => e.ImportDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Warehouse)
                  .WithMany(p => p.Imports)
                  .HasForeignKey(d => d.WarehouseId);

            entity.HasOne(d => d.ImportedByNavigation)
                  .WithMany(p => p.Imports)
                  .HasForeignKey(d => d.CreatedBy);
        });

        modelBuilder.Entity<ImportDetail>(entity =>
        {
            entity.HasKey(e => e.ImportDetailId);
            entity.ToTable("ImportDetail");

            entity.Property(e => e.ImportDetailId).HasColumnName("ImportDetailID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Import)
                  .WithMany(p => p.ImportDetails)
                  .HasForeignKey(d => d.ImportId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ImportReport>(entity =>
        {
            entity.HasKey(e => e.ImportReportId);
            entity.ToTable("ImportReport");

            entity.Property(e => e.ImportReportId).HasColumnName("ImportReportID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.RejectReason).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Import)
                  .WithMany(i => i.ImportReports)
                  .HasForeignKey(e => e.ImportId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByNavigation)
                  .WithMany(u => u.ImportReportsCreated)
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReviewedByNavigation)
                  .WithMany(u => u.ImportReportsReviewed)
                  .HasForeignKey(e => e.ReviewedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ImportReportDetail>(entity =>
        {
            entity.HasKey(e => e.ImportReportDetailId);
            entity.ToTable("ImportReportDetail");

            entity.Property(e => e.ImportReportDetailId).HasColumnName("ImportReportDetailID");

            entity.HasOne(e => e.ImportReport)
                  .WithMany(ir => ir.ImportReportDetails)
                  .HasForeignKey(e => e.ImportReportId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Material)
                  .WithMany(m => m.ImportReportDetails)
                  .HasForeignKey(e => e.MaterialId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Export>(entity =>
        {
            entity.HasKey(e => e.ExportId);
            entity.ToTable("Export");

            entity.Property(e => e.ExportId).HasColumnName("ExportID");
            entity.Property(e => e.ExportDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Warehouse)
                  .WithMany(p => p.Exports)
                  .HasForeignKey(d => d.WarehouseId);

            entity.HasOne(d => d.ExportedByNavigation)
                  .WithMany(p => p.Exports)
                  .HasForeignKey(d => d.CreatedBy);
        });

        modelBuilder.Entity<ExportDetail>(entity =>
        {
            entity.HasKey(e => e.ExportDetailId);
            entity.ToTable("ExportDetail");

            entity.Property(e => e.ExportDetailId).HasColumnName("ExportDetailID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Export)
                  .WithMany(p => p.ExportDetails)
                  .HasForeignKey(d => d.ExportId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ExportReport>(entity =>
   {
       entity.HasKey(e => e.ExportReportId);
       entity.ToTable("ExportReport");

       entity.Property(e => e.ExportReportId).HasColumnName("ExportReportID");
       entity.Property(e => e.ReportDate).HasColumnType("datetime").HasDefaultValueSql("(getdate())");
       entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
       entity.Property(e => e.Notes).HasMaxLength(500);

       entity.HasOne(e => e.Export)
             .WithMany(e => e.ExportReports)
             .HasForeignKey(e => e.ExportId)
             .OnDelete(DeleteBehavior.Cascade);

       entity.HasOne(e => e.ReportedByNavigation)
             .WithMany(u => u.ExportReportsReported)
             .HasForeignKey(e => e.ReportedBy)
             .OnDelete(DeleteBehavior.Restrict);

       entity.HasOne(e => e.DecidedByNavigation)
             .WithMany(u => u.ExportReportsDecided)
             .HasForeignKey(e => e.DecidedBy)
             .OnDelete(DeleteBehavior.Restrict);
   });

        modelBuilder.Entity<ExportReportDetail>(entity =>
        {
            entity.HasKey(e => e.ExportReportDetailId);
            entity.ToTable("ExportReportDetail");

            entity.Property(e => e.ExportReportDetailId).HasColumnName("ExportReportDetailID");
            entity.Property(e => e.Reason).HasMaxLength(500);

            entity.HasOne(e => e.ExportReport)
                  .WithMany(er => er.ExportReportDetails)
                  .HasForeignKey(e => e.ExportReportId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Material)
                  .WithMany(m => m.ExportReportDetails)
                  .HasForeignKey(e => e.MaterialId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

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

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18, 2)")  
                .HasDefaultValueSql("0");          

            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Material).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Mater__5AEE82B9");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Wareh__59FA5E80");
        });


        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);
            entity.ToTable("AuditLog");

            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.Changes).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("(getdate())")
                  .HasColumnType("datetime");

            entity.HasOne(e => e.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId);
            entity.ToTable("Invoice");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.InvoiceType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation)
                  .WithMany(p => p.InvoiceCreatedByNavigations)
                  .HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Partner)
                  .WithMany(p => p.Invoices)
                  .HasForeignKey(d => d.PartnerId);
        });



        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F1578F1246305C4");
            entity.ToTable("InvoiceDetail");

            entity.Property(e => e.InvoiceDetailId).HasColumnName("InvoiceDetailID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.LineTotal)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InvoiceDe__Invoi__6FE99F9F");

            entity.HasOne(d => d.Material).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InvoiceDe__Mater__70DDC3D8");
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
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__628FA481");

            entity.HasOne(d => d.Material).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Mater__6383C8BA");
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
        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__B40CC6ED9105ABE6");

            entity.ToTable("Material");

            entity.HasIndex(e => e.MaterialCode, "UQ__Material__CA1ECF0DFE149FC4").IsUnique();

            entity.Property(e => e.MaterialId)
                .HasColumnName("MaterialID");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MaterialName)
                .HasMaxLength(100);

            entity.Property(e => e.MaterialCode)
                .HasMaxLength(50)
                .HasColumnName("MaterialCode");

            entity.Property(e => e.Unit)
                .HasMaxLength(20);

            entity.Property(e => e.CategoryId)
                .HasColumnName("CategoryID");

            entity.Property(e => e.PartnerId)
                .HasColumnName("PartnerID");

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Materials)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Material__CategoryID__5BE2A6F2");

            entity.HasOne(d => d.Partner)
                .WithMany(p => p.Materials)
                .HasForeignKey(d => d.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Material__PartnerID__5CE5B6AF");
        });

        modelBuilder.Entity<MaterialCheck>(entity =>
        {
            entity.HasKey(e => e.CheckId);
            entity.ToTable("MaterialCheck");

            entity.Property(e => e.CheckId).HasColumnName("CheckID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.CheckDate).HasColumnType("datetime");
            entity.Property(e => e.Result).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(255);

            entity.HasOne(d => d.Material)
                .WithMany(p => p.MaterialChecks)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialCheck__MaterialID");
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

        modelBuilder.Entity<PartnerType>(entity =>
        {
            entity.HasKey(e => e.PartnerTypeId);
            entity.ToTable("PartnerType");

            entity.Property(e => e.PartnerTypeId).HasColumnName("PartnerTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.PartnerId);
            entity.ToTable("Partner");

            entity.Property(e => e.PartnerId).HasColumnName("PartnerID");
            entity.Property(e => e.PartnerName).HasMaxLength(100);
            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.PartnerTypeId).HasColumnName("PartnerTypeID");

            entity.HasOne(d => d.PartnerType)
                .WithMany(p => p.Partners)
                .HasForeignKey(d => d.PartnerTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
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

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BBF6C7E2F");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId)
                .HasColumnName("CategoryID");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}