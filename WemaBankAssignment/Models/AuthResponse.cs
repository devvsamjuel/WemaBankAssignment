﻿using System;

namespace WemaBankAssignment.Models
{
    public class AuthResponse
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime ExpiresIn { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string RefreshToken { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }

    }
}
