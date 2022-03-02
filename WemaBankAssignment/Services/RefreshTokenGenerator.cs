using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WemaBankAssignment.Interfaces
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public async Task<string> GenerateToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return await Task.FromResult(Convert.ToBase64String(randomNumber));
            }
        }
    }
}
