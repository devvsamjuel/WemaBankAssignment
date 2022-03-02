﻿using System.ComponentModel.DataAnnotations;

namespace WemaBankAssignment.Models
{
    public class RegistrationRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StateOfResidence { get; set; }
        [Required]
        public string Lga { get; set; }

    }
}
