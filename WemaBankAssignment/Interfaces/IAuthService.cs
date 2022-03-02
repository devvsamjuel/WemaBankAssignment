using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Models;

namespace WemaBankAssignment.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
        Task<VerifyOtpResponse> VerifyOtp(VerifyOtpRequest model);
        Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest model);
        Task<RefreshTokenResponse> Authenticate(string email, Claim[] claims);

    }
}
