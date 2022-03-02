using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace WemaBankAssignment.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string Lga { get; set; }
        public string JwtRefreshToken { get; set; }
        public string Otp { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? OtpLifeSpan { get; set; }
        public DateTime? DateLastModified { get;  set; }
        public int SaltProperty { get; set; }
        public string Role { get; set; }
    }
}
