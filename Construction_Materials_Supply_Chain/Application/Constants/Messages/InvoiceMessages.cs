namespace Application.Constants.Messages
{
    public static class InvoiceMessages
    {
        public const string INVOICE_CODE_EXISTS = "Mã hóa đơn đã tồn tại.";
        public const string INVOICE_NOT_FOUND = "Hóa đơn không tồn tại.";
        public const string MATERIAL_NOT_FOUND = "Vật tư {0} không tồn tại.";
        public const string INVALID_REQUEST = "Dữ liệu yêu cầu không hợp lệ.";
        public const string ORDER_NOT_FOUND = "Đơn hàng không tồn tại.";
        public const string ORDER_NOT_APPROVED = "Đơn hàng phải được duyệt trước khi tạo hóa đơn.";
        public const string PARTNER_NOT_FOUND = "PartnerId (người mua hoặc người bán) không tìm thấy cho đơn hàng này.";
        public const string NO_MATERIAL_PROVIDED = "Cần ít nhất một vật tư để tạo hóa đơn.";
        public const string NO_MATCHING_MATERIALS = "Không tìm thấy vật tư phù hợp trong đơn hàng.";
        public const string ONLY_PENDING_CAN_BE_REJECTED = "Chỉ có thể từ chối hóa đơn đang chờ.";
        public const string INVOICE_REJECTED_SUCCESS = "Hóa đơn đã được từ chối thành công.";
        public const string INVOICE_CREATED_SUCCESS = "{0} hóa đơn được tạo thành công.";
        public const string DELIVERED_QTY_EXCEEDS_ORDER = "Số lượng xuất cho vật tư {0} vượt quá số lượng trong đơn hàng.";

    }
}
