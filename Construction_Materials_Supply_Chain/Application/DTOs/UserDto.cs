namespace Application.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AvatarBase64 { get; set; }
        public int? PartnerId { get; set; }
        public string? ZaloUserId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserCreateDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarBase64 { get; set; }
        public int? PartnerId { get; set; }
        public string? ZaloUserId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    public class UserUpdateDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<int> RoleIds { get; set; } = new();
        public string? Status { get; set; }
        public string? AvatarBase64 { get; set; }
        public int? PartnerId { get; set; }
        public string? ZaloUserId { get; set; }
    }
}
