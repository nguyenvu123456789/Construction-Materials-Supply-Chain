namespace Application.Constants.Messages
{
    public static class MaterialCheckMessages
    {
        // ===== Not found =====
        public const string CHECK_NOT_FOUND = "Phiếu kiểm kho {0} không tồn tại.";
        public const string WAREHOUSE_NOT_FOUND = "Kho {0} không tồn tại.";
        public const string USER_NOT_FOUND = "User {0} không tồn tại.";
        public const string MATERIAL_NOT_FOUND = "Material {0} không tồn tại.";

        // ===== Invalid data =====
        public const string INVALID_CHECK_DETAILS = "Chi tiết kiểm kho không được để trống.";
        public const string INVALID_ACTION = "Action phải là 'Approved' hoặc 'Rejected'.";

        // ===== Success =====
        public const string CREATE_SUCCESS = "Tạo phiếu kiểm kho thành công.";
        public const string HANDLE_SUCCESS = "Phiếu kiểm kho đã được {0}.";
    }
}
