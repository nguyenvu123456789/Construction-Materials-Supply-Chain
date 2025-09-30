namespace API.DTOs
{
    public class VendorDto
    {
        public int VendorId { get; set; }          // VendorID
        public string VendorName { get; set; }
        public string Status { get; set; }   // Approved / Pending / Rejected
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
