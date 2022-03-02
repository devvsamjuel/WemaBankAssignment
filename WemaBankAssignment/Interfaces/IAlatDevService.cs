using System.Threading.Tasks;
using WemaBankAssignment.Integrations.AlatTechTest;

namespace WemaBankAssignment.Interfaces
{
    public interface IAlatDevService
    {
        Task<GetBanksResponse> GetBanks();
    }
}