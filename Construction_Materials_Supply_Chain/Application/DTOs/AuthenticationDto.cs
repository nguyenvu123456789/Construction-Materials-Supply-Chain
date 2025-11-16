namespace Application.DTOs
{
    public class AdminCreateUserRequestDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class LoginRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();

        public int? PartnerId { get; set; }
        public string? PartnerName { get; set; }
        public string? PartnerType { get; set; }
        public bool MustChangePassword { get; set; }
    }

    public class ChangePasswordRequestDto
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class BulkCreateUsersByEmailRequestDto
    {
        public List<string> Emails { get; set; } = new();
    }
}
