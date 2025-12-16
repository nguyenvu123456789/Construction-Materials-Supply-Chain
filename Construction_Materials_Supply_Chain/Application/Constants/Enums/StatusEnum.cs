namespace Application.Constants.Enums
{
    public enum StatusEnum
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        InProgress = 4,
        Completed = 5,
        Canceled = 6,
        Draft = 7,
        Success = 8,
        Invoiced = 9,
        Export = 10,
        Import = 11,
        Deleted = 12,
        Active = 13,
        InActive = 14,
        ExportReport = 15,
        ImportReport = 16,
        Viewed = 17,
        Order = 18,
    }

    public static class StatusEnumExtensions
    {
        public static string ToStatusString(this StatusEnum status)
        {
            return status.ToString();
        }
    }
}
