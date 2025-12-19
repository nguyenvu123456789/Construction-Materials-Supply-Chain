namespace Application.Constants.Messages
{
    public static class AuthMessages
    {
        // Validation & Input
        public const string REQUEST_NULL = "Dữ liệu yêu cầu không được để trống.";
        public const string EMAIL_REQUIRED = "Email là bắt buộc.";
        public const string EMAIL_INVALID = "Định dạng email không hợp lệ.";
        public const string USERNAME_REQUIRED = "Tên đăng nhập là bắt buộc.";
        public const string PASSWORD_REQUIRED = "Mật khẩu là bắt buộc.";
        public const string PURPOSE_REQUIRED = "Mục đích xác thực là bắt buộc.";
        public const string OTP_REQUIRED = "Mã OTP là bắt buộc.";

        // Business Logic
        public const string LOGIN_FAILED = "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản chưa kích hoạt.";
        public const string USER_NOT_FOUND = "Không tìm thấy người dùng.";
        public const string USER_INACTIVE = "Tài khoản người dùng đang bị khóa hoặc chưa kích hoạt.";
        public const string EMAIL_EXISTED = "Email '{0}' đã tồn tại trong hệ thống.";
        public const string OLD_PASSWORD_WRONG = "Mật khẩu cũ không chính xác.";
        public const string OTP_INVALID_OR_EXPIRED = "Mã OTP không hợp lệ hoặc đã hết hạn.";

        // Notifications
        public const string EMAIL_SUBJECT_NEW_ACCOUNT = "Thông tin tài khoản SCM VLXD";
        public const string EMAIL_SUBJECT_OTP = "Mã xác thực OTP";
    }
}