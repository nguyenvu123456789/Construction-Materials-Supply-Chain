using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserOtp
    {
        public int UserOtpId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = null!;
        public string Purpose { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
