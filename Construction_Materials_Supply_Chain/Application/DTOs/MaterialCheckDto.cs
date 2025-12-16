using Application.DTOs.Application.DTOs;

namespace Application.DTOs
{
    public class MaterialCheckDto
    {
        public int CheckId { get; set; }
        public int MaterialId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckDate { get; set; }
        public string? Result { get; set; }
        public int QuantityChecked { get; set; }
        public string? Notes { get; set; }
    }

    public class MaterialCheckHandleDto
    {
        public int CheckId { get; set; }            // ID phiếu kiểm kho
        public int HandledBy { get; set; }          // User duyệt/từ chối
        public string Action { get; set; }          // "Approve" hoặc "Reject"
        public string? Note { get; set; }           // Ghi chú nếu có
    }
    public class MaterialCheckCreateDto
    {
        public int WarehouseId { get; set; }
        public int UserId { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }

        public List<MaterialCheckDetailDto> Details { get; set; } = new List<MaterialCheckDetailDto>();
    }

    public class MaterialCheckDetailDto
    {
        public int MaterialId { get; set; }
        public decimal SystemQty { get; set; }
        public decimal ActualQty { get; set; }
        public string? Reason { get; set; }
    }

    public class MaterialCheckResponseDto
    {
        public int CheckId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CheckDate { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }

        public List<MaterialCheckDetailResponseDto> Details { get; set; } = new List<MaterialCheckDetailResponseDto>();

        public MaterialCheckHandleResponseDto? LatestHandle { get; set; } // thêm đây
    }


    public class MaterialCheckDetailResponseDto
    {
        public int MaterialId { get; set; }
        public string MaterialCode { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public decimal SystemQty { get; set; }
        public decimal ActualQty { get; set; }
        public string? Reason { get; set; }
    }
    namespace Application.DTOs
    {
        public class MaterialCheckHandleResponseDto
        {
            public int CheckId { get; set; }
            public string Status { get; set; }          // "Approved" hoặc "Rejected"
            public DateTime HandledAt { get; set; }
            public string? Note { get; set; }
        }
    }
    public class MaterialCheckResponseWithHandleDto
    {
        public int CheckId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CheckDate { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }

        // Chi tiết vật tư
        public List<MaterialCheckDetailResponseDto> Details { get; set; } = new List<MaterialCheckDetailResponseDto>();

        // Thông tin duyệt/từ chối mới nhất
        public MaterialCheckHandleResponseDto? LatestHandle { get; set; }
    }
}
