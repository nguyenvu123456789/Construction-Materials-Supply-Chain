namespace BusinessObjects;

public partial class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
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
    public virtual ICollection<ImportRequest> ImportRequestCreatedNavigations { get; set; } = new List<ImportRequest>();
    public virtual ICollection<ExportRequest> ExportRequestCreatedNavigations { get; set; } = new List<ExportRequest>();

}
