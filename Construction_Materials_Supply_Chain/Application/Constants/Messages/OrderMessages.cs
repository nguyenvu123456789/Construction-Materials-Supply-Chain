namespace Application.Constants.Messages
{
    public static class OrderMessages
    {
        public const string INVALID_ORDER_DATA = "Dữ liệu đơn hàng không hợp lệ.";
        public const string BUYER_NOT_FOUND = "Người mua không tồn tại.";
        public const string SUPPLIER_NOT_FOUND = "Nhà cung cấp không tồn tại.";
        public const string BUYER_NO_PARTNER = "Người mua chưa thuộc đối tác nào.";
        public const string INVALID_PARTNER_LEVEL = "Không thể mua từ đối tác có cùng hoặc cấp cao hơn.";
        public const string REGION_MISMATCH = "Người mua và nhà cung cấp không cùng vùng, không thể tạo đơn hàng.";
        public const string ORDER_NOT_FOUND = "Không tìm thấy đơn hàng.";
        public const string HANDLER_NOT_FOUND = "Người xử lý không tồn tại.";
        public const string FETCH_ORDER_SUCCESS = "Lấy thông tin đơn hàng thành công.";
        public const string CREATE_ORDER_SUCCESS = "Tạo đơn hàng thành công.";
        public const string HANDLE_ORDER_SUCCESS = "Xử lý đơn hàng thành công.";
        public const string FETCH_PURCHASE_ORDERS_SUCCESS = "Lấy danh sách đơn mua thành công.";
        public const string FETCH_SALES_ORDERS_SUCCESS = "Lấy danh sách đơn bán thành công.";
        public const string ORDER_CREATE_SUCCESS = "Tạo đơn hàng thành công";
        public const string ORDER_HANDLE_SUCCESS = "Xử lý đơn hàng thành công";
    }
}
