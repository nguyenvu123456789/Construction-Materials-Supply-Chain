namespace Application.Constants.Messages
{
    public static class PersonnelMessages
    {
        // Validation chung
        public const string REQUEST_NULL = "Dữ liệu yêu cầu không được để trống.";
        public const string TYPE_INVALID = "Loại đối tượng không hợp lệ (chỉ chấp nhận: driver, porter, vehicle).";

        // Validation trường dữ liệu
        public const string NAME_REQUIRED = "Tên hoặc mã không được để trống.";
        public const string PHONE_INVALID = "Số điện thoại không đúng định dạng (yêu cầu 10-11 số).";
        public const string PLATE_REQUIRED = "Biển số xe là bắt buộc.";

        // Check trùng lặp
        public const string PHONE_EXISTED = "Số điện thoại '{0}' đã được đăng ký cho nhân sự khác.";
        public const string PLATE_EXISTED = "Biển số xe '{0}' đã tồn tại.";
        public const string CODE_EXISTED = "Mã xe '{0}' đã tồn tại.";

        // Không tìm thấy (Not Found)
        public const string DRIVER_NOT_FOUND = "Không tìm thấy tài xế với ID {0}.";
        public const string PORTER_NOT_FOUND = "Không tìm thấy bốc xếp với ID {0}.";
        public const string VEHICLE_NOT_FOUND = "Không tìm thấy phương tiện với ID {0}.";
    }
}