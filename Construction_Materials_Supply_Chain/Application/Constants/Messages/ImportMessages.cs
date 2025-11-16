namespace Application.Constants.Messages
{
    public static class ImportMessages
    {
        public const string MSG_MISSING_INVOICE_OR_IMPORT = "Bạn phải cung cấp ít nhất một mã: invoiceCode hoặc importCode.";
        public const string MSG_INVOICE_NOT_FOUND = "Hóa đơn không tồn tại.";
        public const string MSG_INVOICE_ALREADY_IMPORTED = "Hóa đơn đã được nhập.";
        public const string MSG_IMPORT_PENDING_NOT_FOUND = "Phiếu nhập đang chờ không tồn tại.";
        public const string MSG_IMPORT_DETAIL_NOT_FOUND = "Không tìm thấy chi tiết cho phiếu nhập này.";
        public const string MSG_REQUIRE_AT_LEAST_ONE_MATERIAL = "Cần ít nhất một vật tư.";
        public const string MSG_MATERIAL_NOT_FOUND = "MaterialId {0} không tồn tại.";
        public const string MSG_ONLY_PENDING_CAN_BE_REJECTED = "Chỉ có thể từ chối phiếu nhập đang chờ.";
    }

}
