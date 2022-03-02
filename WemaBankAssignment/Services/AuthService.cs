using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Data;
using WemaBankAssignment.Entities;
using WemaBankAssignment.Exceptions;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Models;
using WemaBankAssignment.Models.Configurations;

namespace WemaBankAssignment.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly NotificationService _notify;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public AuthService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            NotificationService notify,
             IRefreshTokenGenerator refreshTokenGenerator,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _notify = notify;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<AuthResponse> Login(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new BadRequestException($"User with {request.Email} not found.");
            }

            if (user.Status != StatusType.Active.ToString())
            {
                throw new BadRequestException("Your account is not active");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (signInResult.IsLockedOut)
            {
                throw new BadRequestException("You have been temporarily locked out");
            }

            if (!signInResult.Succeeded)
            {
                throw new BadRequestException($"Credentials for '{request.Email} aren't valid', you have {3 - user.AccessFailedCount} attempt left.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifespan),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            if (user.PhoneNumberConfirmed)
            {
                var response = new AuthResponse()
                {
                    PhoneNumber = user.PhoneNumber,
                    Status = user.Status,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = user.Role,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    TokenType = "Bearer",
                    ExpiresIn = tokenDescriptor.Expires.Value,
                    RefreshToken = await _refreshTokenGenerator.GenerateToken(),
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsPhoneNumberConfirmed = user.PhoneNumberConfirmed
                };

                user.AccessFailedCount = 0;
                user.JwtRefreshToken = response.RefreshToken;
                await _userManager.UpdateAsync(user);

                return response;
            }
            else
            {
                //send Otp
                await _notify.SendOtp(user);

                throw new BadRequestException($"Please verify the otp sent to your email: {request.Email}.");
            }

            throw new BadRequestException($"User with {request.Email} not found.");
        }
        public async Task<RefreshTokenResponse> Authenticate(string email, Claim[] claims)
        {
            var token = GenerateTokenString(email, DateTime.UtcNow, claims);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user != null)
            {
                var refreshToken = await _refreshTokenGenerator.GenerateToken();

                user.JwtRefreshToken = refreshToken;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var response = new RefreshTokenResponse
                {
                    JwtToken = token,
                    RefreshToken = refreshToken
                };

                return response;
            }
            else
            {
                throw new BadRequestException("Invalid user credentials");
            }
        }
        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email || x.PhoneNumber == request.PhoneNumber);
            if (existingUser != null)
            {
                throw new BadRequestException("User already exist, kindly Login with your email and password");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                State = request.StateOfResidence,
                UserName = request.Email
            };

            //Generate Otp
            user.Otp = new Random().Next(10000, 99999).ToString();
            user.DateCreated = DateTime.Now;
            user.Status = StatusType.Active.ToString();
            user.OtpLifeSpan = DateTime.Now;
            user.DateLastModified = DateTime.Now;
            user.SaltProperty = CryptoServices.CreateRandomSalt();
            user.Role = UserRole.Customer.ToString();

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());

                //send Otp to User to confirm email
                await _notify.SendOtp(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Email)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifespan),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                var registrationResponse = new RegistrationResponse
                {
                    PhoneNumber = user.PhoneNumber,
                    Status = user.Status,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = user.Role,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    TokenType = "Bearer",
                    ExpiresIn = tokenDescriptor.Expires.Value,
                    RefreshToken = await _refreshTokenGenerator.GenerateToken(),
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsPhoneNumberConfirmed = user.PhoneNumberConfirmed
                };

                return registrationResponse;
            }
            else
            {
                throw new BadRequestException($"{result.Errors}");
            }
        }
        public async Task<VerifyOtpResponse> VerifyOtp(VerifyOtpRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                throw new BadRequestException("User Not found");
            }

            if (user.Otp != model.Otp)
            {
                throw new BadRequestException("Invalid Otp");
            }

            var otpLifeSpan = (DateTime.Now - user.OtpLifeSpan.Value).TotalMinutes;

            if (otpLifeSpan <= _jwtSettings.TokenLifespan)
            {
                //Update user Otp confirmation
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                var response = new VerifyOtpResponse()
                {
                    PhoneNumber = user.PhoneNumber,
                    Status = user.Status,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Role = user.Role,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsPhoneNumberConfirmed =user.PhoneNumberConfirmed
                };
                return response;
            }
            else
            {
                throw new Exception("Expired Token", null);
            }
        }
      
        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            var pricipal = tokenHandler.ValidateToken(request.JwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                }, out validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new BadRequestException("Invalid Token");
            }

            var userName = pricipal.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.JwtRefreshToken == request.RefreshToken);
            if (user == null || request.RefreshToken != user.JwtRefreshToken)
            {
                throw new BadRequestException("Invalid token passed!");
            }

            var result = await Authenticate(userName, pricipal.Claims.ToArray());
            var response = JsonConvert.DeserializeObject<RefreshTokenResponse>(JsonConvert.SerializeObject(result));

            return await Task.FromResult(response);
        }



        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifespan),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        private async Task<SecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            //implement jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifespan),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(token);
        }
        private string GenerateTokenString(string username, DateTime expires, Claim[] claims = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                 claims ?? new Claim[]
                 {
                    new Claim(ClaimTypes.Name, username)
                 }),
                //NotBefore = expires,
                Expires = expires.AddMinutes(_jwtSettings.TokenLifespan),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
