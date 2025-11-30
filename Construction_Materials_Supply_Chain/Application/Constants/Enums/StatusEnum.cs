using System;

namespace Application.Constants.Enums
{
    public enum StatusEnum
    {
        Pending = 1,        // Chờ xử lý
        Approved = 2,       // Đã duyệt
        Rejected = 3,       // Từ chối
        InProgress = 4,     // Đang xử lý
        Completed = 5,      // Hoàn thành
        Canceled = 6,       // Hủy
        Draft = 7,          // Nháp
        Success = 8         // Thành công
    }

    public static class StatusEnumExtensions
    {
        public static string ToStatusString(this StatusEnum status)
        {
            return status.ToString();
        }
    }
}
