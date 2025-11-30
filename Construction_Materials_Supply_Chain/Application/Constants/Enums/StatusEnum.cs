using System;

namespace Application.Constants.Enums
{
    public enum StatusEnum
    {
        Pending = 1,        // Chờ xử lý
        Approved = 2,       // Đã duyệt
        Rejected = 3,       
        InProgress = 4,     // Đang xử lý
        Completed = 5,      
        Canceled = 6,       
        Draft = 7,         
        Success = 8,
        Invoiced = 9,
        Export = 10,
        Import = 11
    }

    public static class StatusEnumExtensions
    {
        public static string ToStatusString(this StatusEnum status)
        {
            return status.ToString();
        }
    }
}
