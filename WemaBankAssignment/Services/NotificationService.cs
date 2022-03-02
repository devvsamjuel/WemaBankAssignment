using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WemaBankAssignment.Data;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Models;
using WemaBankAssignment.Models.Configurations;

namespace WemaBankAssignment.Services
{
    public class NotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly JwtSettings _jwtSettings;
        private readonly AppSettings _appSettings;

        public NotificationService(IEmailSender emailSender, 
            IOptions<JwtSettings> jwtSettings,
            IOptions<AppSettings> appSettings
            )
        {
            _emailSender = emailSender;
            _jwtSettings = jwtSettings.Value;
            _appSettings = appSettings.Value;

        }
        public async Task SendOtp(ApplicationUser user)
        {
            var email = new Email
            {
                To = user.Email,
                Body = $"Please use this Otp  to confirm your email: {user.Otp}" +
                        $"It expires in {_appSettings.OtpLifespan}.",
                Subject = "Confirm Email Address"
            };


            await _emailSender.SendEmail(email);
        }
    }
}
