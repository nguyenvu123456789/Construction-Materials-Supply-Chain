namespace Application.Constants.Enums
{
    public enum ImportStatus
    {
        Pending = 0,
        Success = 1,
        Rejected = 2
    }

    public enum ImportDetailStatus
    {
        Pending = 0,
        Confirmed = 1
    }

    public enum ImportType
    {
        FromInvoice = 0,
        Manual = 1
    }
}
