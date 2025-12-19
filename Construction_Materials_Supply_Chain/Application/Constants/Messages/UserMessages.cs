namespace Application.Constants.Messages
{
    public static class UserMessages
    {
        // Thông báo lỗi chung
        public const string REQUEST_NULL = "Dữ liệu yêu cầu không được để trống.";
        public const string STATUS_INVALID = "Trạng thái không hợp lệ (chỉ chấp nhận 'Active' hoặc 'Inactive').";

        // Thông báo liên quan đến User
        public const string USER_NOT_FOUND = "Không tìm thấy người dùng.";
        public const string USER_DELETED = "Người dùng này đã bị xóa.";
        public const string CANNOT_UPDATE_DELETED = "Không thể cập nhật thông tin người dùng đã bị xóa.";

        // Thông báo Validate dữ liệu (Trùng lặp, định dạng)
        public const string EMAIL_EXISTED = "Email '{0}' đã tồn tại trong hệ thống.";
        public const string USERNAME_EXISTED = "Tên đăng nhập '{0}' đã tồn tại.";
        public const string PHONE_INVALID = "Số điện thoại không đúng định dạng (yêu cầu 10-11 số).";

        // Thông báo liên quan đến File/Ảnh
        public const string AVATAR_INVALID = "File ảnh đại diện không hợp lệ hoặc bị lỗi.";
    }
}