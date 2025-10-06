namespace Domain.Models;

public partial class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? FullName { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public virtual ICollection<Invoice> InvoiceCreatedByNavigations { get; set; } = new List<Invoice>();
    public virtual ICollection<Invoice> InvoiceCustomers { get; set; } = new List<Invoice>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
    //Import
    public virtual ICollection<Import> Imports { get; set; } = new List<Import>();
    public virtual ICollection<ImportReport> ImportReportsCreated { get; set; } = new List<ImportReport>();
    public virtual ICollection<ImportReport> ImportReportsReviewed { get; set; } = new List<ImportReport>();
    //Export

    public virtual ICollection<Export> Exports { get; set; } = new List<Export>();
    public virtual ICollection<ExportReport> ExportReportsReported { get; set; } = new List<ExportReport>();
    public virtual ICollection<ExportReport> ExportReportsDecided { get; set; } = new List<ExportReport>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
