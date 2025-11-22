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
    public virtual DbSet<HandleRequest> HandleRequests { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Material> Materials { get; set; }
    public virtual DbSet<MaterialCheck> MaterialChecks { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RolePermission> RolePermissions { get; set; }
    public virtual DbSet<ShippingLog> ShippingLogs { get; set; }
    public virtual DbSet<Partner> Partners { get; set; }
    public virtual DbSet<PartnerType> PartnerTypes { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<PartnerRegion> PartnerRegions { get; set; }

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
    public virtual DbSet<MaterialPartner> MaterialPartners { get; set; } = null!;

    public virtual DbSet<GlAccount> GlAccounts { get; set; }
    public virtual DbSet<JournalEntry> JournalEntries { get; set; }
    public virtual DbSet<JournalLine> JournalLines { get; set; }
    public virtual DbSet<MoneyAccount> MoneyAccounts { get; set; }
    public virtual DbSet<BankStatement> BankStatements { get; set; }
    public virtual DbSet<BankStatementLine> BankStatementLines { get; set; }
    public virtual DbSet<Receipt> Receipts { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<PostingPolicy> PostingPolicies { get; set; }
    public virtual DbSet<SubLedgerEntry> SubLedgerEntries { get; set; }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Porter> Porters { get; set; }

    public DbSet<Transport> Transports { get; set; }
    public DbSet<TransportStop> TransportStops { get; set; }
    public DbSet<TransportInvoice> TransportInvoices { get; set; }
    public DbSet<TransportPorter> TransportPorters { get; set; }

    public DbSet<PriceMaterialPartner> PriceMaterialPartners { get; set; }
    public DbSet<NotificationRecipient> NotificationRecipients { get; set; }
    public DbSet<NotificationRecipientRole> NotificationRecipientRoles { get; set; }
    public DbSet<NotificationReply> NotificationReplies { get; set; }

    public virtual DbSet<InventoryAlertRule> InventoryAlertRules { get; set; } = null!;
    public virtual DbSet<InventoryAlertRuleRole> InventoryAlertRuleRoles { get; set; } = null!;
    public virtual DbSet<InventoryAlertRuleUser> InventoryAlertRuleUsers { get; set; } = null!;
    public virtual DbSet<EventNotificationSetting> EventNotificationSettings { get; set; } = null!;
    public virtual DbSet<EventNotificationSettingRole> EventNotificationSettingRoles { get; set; } = null!;
    public virtual DbSet<UserOtp> UserOtps { get; set; } = null!;

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
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Import)
                  .WithMany(i => i.ImportReports)
                  .HasForeignKey(e => e.ImportId)
                  .OnDelete(DeleteBehavior.Cascade);
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
            entity.Property(e => e.ImportStatus)
                     .HasMaxLength(50)
                     .HasDefaultValue("Pending");
            entity.Property(e => e.ExportStatus)
                  .HasMaxLength(50)
                  .HasDefaultValue("Pending");

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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAF0731C5D2");
            entity.ToTable("Order");
            entity.HasIndex(e => e.OrderCode, "UQ__Order__CAC5E743BD8032BF").IsUnique();
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.OrderCode).HasMaxLength(50);
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

        modelBuilder.Entity<HandleRequest>(entity =>
        {
            entity.HasKey(e => e.HandleRequestId);

            entity.HasOne(d => d.HandledByNavigation)
                .WithMany()
                .HasForeignKey(d => d.HandledBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_HandleRequests_User_HandledByNavigationUserId");
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

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Materials)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Material__CategoryID__5BE2A6F2");

        });

        modelBuilder.Entity<MaterialPartner>(entity =>
        {
            entity.ToTable("MaterialPartner");

            entity.HasKey(e => new { e.MaterialId, e.PartnerId });

            entity.HasOne(e => e.Material)
                .WithMany(m => m.MaterialPartners)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialPartner_Material");

            entity.HasOne(e => e.Partner)
                .WithMany(p => p.MaterialPartners)
                .HasForeignKey(e => e.PartnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialPartner_Partner");
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

            entity.Property(p => p.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

        });

        modelBuilder.Entity<Region>()
        .HasIndex(r => r.RegionName)
        .IsUnique();

        modelBuilder.Entity<PartnerRegion>()
            .HasOne(pr => pr.Partner)
            .WithMany(p => p.PartnerRegions)
            .HasForeignKey(pr => pr.PartnerId);

        modelBuilder.Entity<PartnerRegion>()
            .HasOne(pr => pr.Region)
            .WithMany(r => r.PartnerRegions)
            .HasForeignKey(pr => pr.RegionId);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACDB2E31D3");
            entity.ToTable("User");
            entity.HasIndex(e => e.UserName, "UQ__User__C9F28456BAF9FD7E").IsUnique();
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(u => u.Partner)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(u => u.PartnerId);
            entity.Property(u => u.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.Property(u => u.ZaloUserId).HasMaxLength(64);
            entity.Property(u => u.MustChangePassword)
                .HasDefaultValue(false);
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

        modelBuilder.Entity<GlAccount>(e =>
        {
            e.ToTable("GlAccount");
            e.HasKey(x => x.AccountId);
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(255).IsRequired();
            e.Property(x => x.Type).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.PartnerId, x.Code }).IsUnique();
            e.HasOne(x => x.Parent).WithMany(p => p.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<JournalEntry>(e =>
        {
            e.ToTable("JournalEntry");
            e.HasKey(x => x.JournalEntryId);
            e.Property(x => x.PartnerId).IsRequired();
            e.Property(x => x.PostingDate).HasColumnType("datetime");
            e.HasIndex(x => new { x.PartnerId, x.SourceType, x.SourceId }).IsUnique();
        });

        modelBuilder.Entity<JournalLine>(e =>
        {
            e.ToTable("JournalLine");
            e.HasKey(x => x.JournalLineId);
            e.Property(x => x.Debit).HasColumnType("decimal(18,2)");
            e.Property(x => x.Credit).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.JournalEntry).WithMany(j => j.Lines).HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.PartnerId, x.AccountId });
        });

        modelBuilder.Entity<PostingPolicy>(e =>
        {
            e.ToTable("PostingPolicy");
            e.HasKey(x => x.PostingPolicyId);
            e.Property(x => x.PartnerId).IsRequired();
            e.Property(x => x.DocumentType).HasMaxLength(50).IsRequired();
            e.Property(x => x.RuleKey).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.PartnerId, x.DocumentType, x.RuleKey }).IsUnique();
        });

        modelBuilder.Entity<SubLedgerEntry>(e =>
        {
            e.ToTable("SubLedgerEntry");
            e.HasKey(x => x.SubLedgerEntryId);
            e.Property(x => x.SubLedgerType).HasMaxLength(5).IsRequired();
            e.Property(x => x.Debit).HasColumnType("decimal(18,2)");
            e.Property(x => x.Credit).HasColumnType("decimal(18,2)");
            e.HasIndex(x => new { x.PartnerId, x.SubLedgerType, x.Date });
        });

        modelBuilder.Entity<Receipt>(e =>
        {
            e.ToTable("Receipt");
            e.HasKey(x => x.ReceiptId);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Method).HasMaxLength(50);
            e.HasIndex(x => new { x.Date, x.PartnerId });
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("Payment");
            e.HasKey(x => x.PaymentId);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Method).HasMaxLength(50);
            e.HasIndex(x => new { x.Date, x.PartnerId });
        });

        modelBuilder.Entity<MoneyAccount>(e =>
        {
            e.ToTable("MoneyAccount");
            e.HasKey(x => x.MoneyAccountId);
            e.Property(x => x.PartnerId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(255).IsRequired();
            e.Property(x => x.Type).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.PartnerId, x.Name }).IsUnique();
        });

        modelBuilder.Entity<BankStatement>(e =>
        {
            e.ToTable("BankStatement");
            e.HasKey(x => x.BankStatementId);
            e.Property(x => x.PartnerId).IsRequired();
            e.HasOne(x => x.MoneyAccount).WithMany().HasForeignKey(x => x.MoneyAccountId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.PartnerId, x.MoneyAccountId, x.From, x.To });
        });

        modelBuilder.Entity<BankStatementLine>(e =>
        {
            e.ToTable("BankStatementLine");
            e.HasKey(x => x.BankStatementLineId);
            e.HasOne(x => x.BankStatement).WithMany(b => b.Lines).HasForeignKey(x => x.BankStatementId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(e =>
        {
            e.HasKey(x => x.AddressId);
            e.ToTable("Address");
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasKey(x => x.VehicleId);
            e.ToTable("Vehicle");
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.PlateNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.VehicleClass).HasMaxLength(50);
            e.HasOne(x => x.Partner).WithMany(p => p.Vehicles).HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Driver>(e =>
        {
            e.HasKey(x => x.DriverId);
            e.ToTable("Driver");
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20);
            e.HasOne(x => x.Partner).WithMany(p => p.Drivers).HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Porter>(e =>
        {
            e.HasKey(x => x.PorterId);
            e.ToTable("Porter");
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20);
            e.HasOne(x => x.Partner).WithMany(p => p.Porters).HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transport>(e =>
        {
            e.HasKey(x => x.TransportId);
            e.HasOne(x => x.Depot).WithMany().HasForeignKey(x => x.DepotId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ProviderPartner).WithMany().HasForeignKey(x => x.ProviderPartnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Vehicle).WithMany(v => v.Transports).HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Driver).WithMany(d => d.Transports).HasForeignKey(x => x.DriverId).OnDelete(DeleteBehavior.SetNull);
            e.HasMany(x => x.TransportPorters).WithOne(tp => tp.Transport).HasForeignKey(tp => tp.TransportId);
        });

        modelBuilder.Entity<TransportStop>(e =>
        {
            e.HasKey(x => x.TransportStopId);
            e.ToTable("TransportStop");
            e.Property(x => x.StopType).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.ProofImageBase64);
            e.HasOne(x => x.Transport).WithMany(t => t.Stops).HasForeignKey(x => x.TransportId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.TransportInvoices).WithOne(ti => ti.TransportStop).HasForeignKey(ti => ti.TransportStopId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TransportInvoice>(e =>
        {
            e.ToTable("TransportInvoice");
            e.HasKey(x => new { x.TransportStopId, x.InvoiceId });
            e.HasOne(x => x.TransportStop).WithMany(s => s.TransportInvoices).HasForeignKey(x => x.TransportStopId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Invoice).WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.InvoiceId).IsUnique();
        });

        modelBuilder.Entity<TransportPorter>(e =>
        {
            e.HasKey(x => new { x.TransportId, x.PorterId });
            e.ToTable("TransportPorter");
            e.Property(x => x.Role).HasMaxLength(20);
            e.HasOne(x => x.Transport).WithMany(t => t.TransportPorters).HasForeignKey(x => x.TransportId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShippingLog>(e =>
        {
            e.HasKey(x => x.ShippingLogId);
            e.ToTable("ShippingLog");

            e.Property(x => x.Status).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedAt)
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasOne(x => x.Transport)
                .WithMany()
                .HasForeignKey(x => x.TransportId)
                .OnDelete(DeleteBehavior.NoAction);

            e.HasOne(x => x.TransportStop)
                .WithMany()
                .HasForeignKey(x => x.TransportStopId)
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(x => x.Invoice)
                .WithMany()
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PriceMaterialPartner>(e =>
        {
            e.HasKey(x => x.PriceMaterialPartnerId);
            e.Property(x => x.BuyPrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            e.Property(x => x.SellPrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            e.HasOne(x => x.Partner).WithMany().HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Content).HasMaxLength(4000);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PartnerId).IsRequired();
            entity.HasOne(e => e.Partner).WithMany().HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationRecipient>(entity =>
        {
            entity.HasKey(e => e.NotificationRecipientId);
            entity.HasIndex(e => new { e.NotificationId, e.UserId }).IsUnique();
            entity.Property(e => e.PartnerId).IsRequired();
            entity.HasOne(e => e.Notification).WithMany(n => n.NotificationRecipients).HasForeignKey(e => e.NotificationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Partner).WithMany().HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationRecipientRole>(entity =>
        {
            entity.HasKey(e => e.NotificationRecipientRoleId);
            entity.HasIndex(e => new { e.NotificationId, e.RoleId }).IsUnique();
            entity.Property(e => e.PartnerId).IsRequired();
            entity.HasOne(e => e.Notification).WithMany(n => n.NotificationRecipientRoles).HasForeignKey(e => e.NotificationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Partner).WithMany().HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationReply>(entity =>
        {
            entity.HasKey(e => e.NotificationReplyId);
            entity.Property(e => e.Message).HasMaxLength(4000);
            entity.Property(e => e.PartnerId).IsRequired();
            entity.HasOne(e => e.Notification).WithMany(n => n.NotificationReplies).HasForeignKey(e => e.NotificationId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Partner).WithMany().HasForeignKey(e => e.PartnerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<NotificationReply>().WithMany().HasForeignKey(e => e.ParentReplyId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryAlertRule>(e =>
        {
            e.HasKey(x => x.InventoryAlertRuleId);
            e.Property(x => x.PartnerId).IsRequired();
            e.Property(x => x.MinQuantity).HasPrecision(18, 2);
            e.Property(x => x.CriticalMinQuantity).HasPrecision(18, 2);
            e.HasOne(x => x.Partner).WithMany().HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.SendEmail).HasDefaultValue(true);
            e.Property(x => x.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<InventoryAlertRuleRole>(e =>
        {
            e.HasKey(x => x.InventoryAlertRuleRoleId);
            e.HasOne(x => x.Rule).WithMany(r => r.Roles).HasForeignKey(x => x.InventoryAlertRuleId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.InventoryAlertRuleId, x.RoleId }).IsUnique();
        });

        modelBuilder.Entity<InventoryAlertRuleUser>(e =>
        {
            e.HasKey(x => x.InventoryAlertRuleUserId);
            e.HasOne(x => x.Rule).WithMany(r => r.Users).HasForeignKey(x => x.InventoryAlertRuleId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.InventoryAlertRuleId, x.UserId }).IsUnique();
        });

        modelBuilder.Entity<EventNotificationSetting>(e =>
        {
            e.HasKey(x => x.EventNotificationSettingId);
            e.Property(x => x.PartnerId).IsRequired();
            e.Property(x => x.EventType).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.Partner).WithMany().HasForeignKey(x => x.PartnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.PartnerId, x.EventType }).IsUnique();
            e.Property(x => x.SendEmail).HasDefaultValue(true);
            e.Property(x => x.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<EventNotificationSettingRole>(e =>
        {
            e.HasKey(x => x.EventNotificationSettingRoleId);
            e.HasOne(x => x.Setting).WithMany(s => s.Roles).HasForeignKey(x => x.EventNotificationSettingId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.EventNotificationSettingId, x.RoleId }).IsUnique();
        });

        modelBuilder.Entity<UserOtp>(entity =>
        {
            entity.HasKey(e => e.UserOtpId);
            entity.ToTable("UserOtp");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Purpose).HasMaxLength(50);
            entity.Property(e => e.CreatedAt);
            entity.Property(e => e.ExpiresAt);
            entity.Property(e => e.IsUsed).HasDefaultValue(false);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}