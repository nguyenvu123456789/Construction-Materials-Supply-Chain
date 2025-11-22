namespace Application.DTOs
{
    public class OtpRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }

    public class OtpVerifyDto
    {
        public string Email { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordWithOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
