﻿namespace API.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }         
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
