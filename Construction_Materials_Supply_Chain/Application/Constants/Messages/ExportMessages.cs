namespace Application.Constants.Messages
{
    public static class ExportMessages
    {
        public const string MSG_REQUIRE_AT_LEAST_ONE_MATERIAL = "Cần ít nhất một vật tư.";
        public const string MSG_MATERIAL_NOT_FOUND_IN_WAREHOUSE = "Vật tư {0} không tồn tại trong kho {1}.";
        public const string MSG_NOT_ENOUGH_STOCK = "Không đủ tồn kho cho vật tư {0}. Tồn: {1}, Yêu cầu: {2}.";
        public const string MSG_MATERIAL_NOT_FOUND = "MaterialId {0} không tồn tại.";
        public const string MSG_PENDING_EXPORT_NOT_FOUND = "Không tìm thấy phiếu xuất đang chờ.";
        public const string MSG_EXPORT_DETAIL_NOT_FOUND = "Không tìm thấy chi tiết phiếu xuất.";
        public const string MSG_NOT_ENOUGH_STOCK_WHEN_CONFIRM = "Không đủ số lượng trong kho cho vật tư {0}.";
        public const string MSG_ONLY_PENDING_CAN_BE_REJECTED = "Chỉ được từ chối phiếu xuất đang ở trạng thái Pending.";
        public const string MSG_INVOICE_NOT_FOUND = "Hóa đơn không tồn tại.";
        public const string MSG_INVOICE_HAS_NO_DETAILS = "Hóa đơn không có chi tiết.";
        public const string MSG_MATERIAL_NOT_IN_WAREHOUSE = "Vật tư {0} không tồn tại trong kho.";
        public const string INVALID_REQUEST = "Dữ liệu yêu cầu không hợp lệ.";
        public const string EXPORT_NOT_FOUND = "Phiếu xuất không tồn tại.";
        public const string INTERNAL_ERROR = "Đã xảy ra lỗi hệ thống.";
    }
}
