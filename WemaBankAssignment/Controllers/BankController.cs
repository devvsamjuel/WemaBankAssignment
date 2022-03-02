using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WemaBankAssignment.Integrations.AlatTechTest;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Middleware;

namespace WemaBank.Assignment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IAlatDevService _alatDevService;

        public BankController(IAlatDevService alatDevService)
        {
            _alatDevService = alatDevService;
        }


        /// <summary>
        /// This end point fetches list of banks
        /// </summary>
        /// <param></param>
        /// <returns>GetBanksResponse model</returns>
        /// <remarks>
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("GetBanks")]
        [Produces(typeof(GetBanksResponse))]
        [ProducesErrorResponseType(typeof(ErrorDetails))]
        public async Task<IActionResult> GetBanks()
        {
            return Ok(await _alatDevService.GetBanks());
        }
    }
}
