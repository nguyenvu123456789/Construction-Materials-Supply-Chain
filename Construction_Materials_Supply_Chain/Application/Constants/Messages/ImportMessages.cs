namespace Application.Constants.Messages
{
    public static class ImportMessages
    {
        public const string MSG_INVALID_REQUEST_DATA = "Dữ liệu request không hợp lệ.";
        public const string MSG_WAREHOUSE_REQUIRED_FOR_INVOICE = "WarehouseId là bắt buộc khi nhập kho từ hóa đơn.";
        public const string MSG_IMPORT_NOT_FOUND = "Phiếu nhập không tồn tại.";

        public const string MSG_MISSING_INVOICE_OR_IMPORT = "Bạn phải cung cấp ít nhất một mã: invoiceCode hoặc importCode.";
        public const string MSG_INVOICE_NOT_FOUND = "Hóa đơn không tồn tại.";
        public const string MSG_INVOICE_ALREADY_IMPORTED = "Hóa đơn đã được nhập.";
        public const string MSG_IMPORT_PENDING_NOT_FOUND = "Phiếu nhập đang chờ không tồn tại.";
        public const string MSG_IMPORT_DETAIL_NOT_FOUND = "Không tìm thấy chi tiết cho phiếu nhập này.";
        public const string MSG_REQUIRE_AT_LEAST_ONE_MATERIAL = "Cần ít nhất một vật tư.";
        public const string MSG_MATERIAL_NOT_FOUND = "MaterialId {0} không tồn tại.";
        public const string MSG_INVOICE_CODE_REQUIRED = "InvoiceCode là bắt buộc.";
        public const string MSG_IMPORT_REPORT_CREATED = "Báo cáo nhập kho được tạo thành công.";
        public const string MSG_FAILED_LOAD_REPORT = "Không thể tải báo cáo vừa tạo.";
        public const string MSG_IMPORT_REPORT_NOT_FOUND = "Không tìm thấy báo cáo nhập kho.";
        public const string MSG_NOT_YET_REVIEWED = "Chưa duyệt";
        public const string MSG_UNKNOWN_CREATOR = "Không rõ";
        public const string MSG_ONLY_PENDING_CAN_BE_REJECTED = "Chỉ có thể từ chối phiếu nhập đang chờ.";
        public const string MSG_UNKNOWN_USER = "Không rõ";
        public const string MSG_INVALID_REPORT_DATA = "Dữ liệu báo cáo không hợp lệ.";
        public const string MSG_IMPORT_CREATED_SUCCESS = "Báo cáo nhập kho tạo thành công.";
        public const string MSG_IMPORT_REVIEWED_SUCCESS = "Báo cáo nhập kho đã được duyệt/từ chối.";
        public const string MSG_IMPORT_DETAIL_MISSING = "Thiếu chi tiết báo cáo nhập kho.";
        public const string MSG_NOT_ENOUGH_STOCK = "Không đủ tồn kho cho vật tư {0}.";
        public const string MSG_ONLY_REJECTED_REPORT_CAN_RETURN ="Chỉ có thể tạo phiếu nhập lại khi báo cáo nhập kho bị từ chối.";
        public const string MSG_INVOICE_REQUIRED_FOR_RETURN ="Không tìm thấy hóa đơn liên quan để tạo phiếu nhập lại.";
        public const string MSG_SELLER_WAREHOUSE_NOT_FOUND ="Không xác định được kho của bên bán.";
        public const string MSG_RETURN_IMPORT_NOTE ="Nhập lại vật tư không bị hư hỏng từ báo cáo nhập kho {0}.";
        public const string MSG_ONLY_PENDING_CAN_BE_REVIEWED = "Chỉ báo cáo nhập kho ở trạng thái Pending mới được duyệt.";
        public const string MSG_MATERIAL_NOT_FOUND_BY_ID = "MaterialId {0} không tồn tại.";

    }

}
