using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public partial class HandleRequest
    {
        public int HandleRequestId { get; set; }

        public string RequestType { get; set; } = null!; // "Order", "Import", "Export", ...
        public int RequestId { get; set; }               // ID của bản ghi cần xử lý

        public int HandledBy { get; set; }              // UserId của người thực hiện
        public string ActionType { get; set; } = null!; // "Approved", "Rejected", "Cancelled", "Reopened"
        public string? Note { get; set; }
        public DateTime HandledAt { get; set; } = DateTime.Now;


        [ForeignKey(nameof(HandledBy))]
        public virtual User HandledByNavigation { get; set; } = null!;

    }
}
