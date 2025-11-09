namespace Domain.Models
{
    public enum AlertRecipientMode { Manager = 1, Roles = 2, Users = 3 }

    public partial class InventoryAlertRule
    {
        public int InventoryAlertRuleId { get; set; }
        public int PartnerId { get; set; }
        public int? WarehouseId { get; set; }
        public int MaterialId { get; set; }
        public decimal MinQuantity { get; set; }
        public decimal? CriticalMinQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public bool SendZalo { get; set; } = true;
        public AlertRecipientMode RecipientMode { get; set; } = AlertRecipientMode.Manager;
        public virtual Partner Partner { get; set; } = null!;
        public virtual Warehouse? Warehouse { get; set; }
        public virtual Material Material { get; set; } = null!;
        public virtual ICollection<InventoryAlertRuleRole> Roles { get; set; } = new List<InventoryAlertRuleRole>();
        public virtual ICollection<InventoryAlertRuleUser> Users { get; set; } = new List<InventoryAlertRuleUser>();
    }

    public partial class InventoryAlertRuleRole
    {
        public int InventoryAlertRuleRoleId { get; set; }
        public int InventoryAlertRuleId { get; set; }
        public int RoleId { get; set; }
        public virtual InventoryAlertRule Rule { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }

    public partial class InventoryAlertRuleUser
    {
        public int InventoryAlertRuleUserId { get; set; }
        public int InventoryAlertRuleId { get; set; }
        public int UserId { get; set; }
        public virtual InventoryAlertRule Rule { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
