using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class MaterialPartner
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Material")]
        public int MaterialId { get; set; }

        [ForeignKey("Partner")]
        public int PartnerId { get; set; }

        [ForeignKey("Buyer")]
        public int BuyerId { get; set; }
        public virtual Material Material { get; set; } = null!;
        public virtual Partner Partner { get; set; } = null!;
        public virtual Partner Buyer { get; set; } = null!;
    }
}
