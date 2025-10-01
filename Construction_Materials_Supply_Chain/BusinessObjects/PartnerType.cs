using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public partial class PartnerType
    {
        public int PartnerTypeId { get; set; }
        public string TypeName { get; set; } = null!;  // Supplier / Distributor / Agent

        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}
