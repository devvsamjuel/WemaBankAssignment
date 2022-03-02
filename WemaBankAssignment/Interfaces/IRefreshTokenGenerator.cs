using System.Threading.Tasks;

namespace WemaBankAssignment.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        Task<string> GenerateToken();
    }
}
