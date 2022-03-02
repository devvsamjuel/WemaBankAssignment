using System.ComponentModel.DataAnnotations;

namespace WemaBankAssignment.Models
{
    public class VerifyOtpRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Otp { get; set; }
    }
}
