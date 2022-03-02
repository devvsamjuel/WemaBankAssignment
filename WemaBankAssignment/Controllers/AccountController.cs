using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Middleware;
using WemaBankAssignment.Models;

namespace WemaBankAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authenticationService;
        public AccountController(IAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// This endpoint is used for signin up a customer, the rolecode specifies the role of the user.
        /// </summary>
        /// <param name="request">This is the request payload: RegistrationRequest model</param>
        /// <returns>RegistrationResponse model</returns>
        /// <remarks>
        ///  
        /// The request body to Login
        /// {  
        ///"firstName": user's first name,
        ///"lastName": user's last name,
        ///"phoneNumber": user's phone number,
        ///"emailAddress": user's email address,
        ///"password": user's password,
        ///"stateOfResidence":user's state of residence,
        ///"lga":"user's local government area" 
        /// }  
        /// 
        /// Example request body
        ///{
        ///"firstName": "Samuel",
        ///"lastName": "Babatunde",
        ///"phoneNumber": "2348135920222",
        ///"emailAddress": "samuel@gmail.com",
        ///"password": "samuel@gig123",
        ///"stateOfResidence":"Lagos",
        ///"lga":"Mainland"
        ///}
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("register")]
        [Produces(typeof(RegistrationResponse))]
        [ProducesErrorResponseType(typeof(ErrorDetails))]
        public async Task<ActionResult<RegistrationResponse>> Register([FromBody] RegistrationRequest request)
        {
            return Ok(await _authenticationService.Register(request));
        }

        /// <summary>
        /// This end point enables user Login by either using Email and password
        /// </summary>
        /// <param name="request">This is the request payload: AuthRequest model</param>
        /// <returns>AuthResponse model</returns> 
        /// <remarks>
        /// 
        /// The request body to Login
        /// {  
        /// email: user's email address,  
        /// password: user's password,  
        /// }  
        /// 
        /// Example request body
        /// {
        ///      "email": "samuel@gmail.com",
        ///      "password": "samuel@gig123"
        /// }
        /// </remarks>
        [HttpPost("login")]
        [Produces(typeof(AuthResponse))]
        [ProducesErrorResponseType(typeof(ErrorDetails))]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            return Ok(await _authenticationService.Login(request));
        }

        /// <summary>
        /// This endpoint takes the customer's email, and the sent Otp, it confirms the user has verified his account. 
        /// </summary>
        /// <param name="request">This is the request payload: VerifyOtpRequest model</param>
        /// <returns>VerifyOtpResponse model</returns>
        /// <remarks>
        /// The request body for verifying otp
        /// {  
        /// email: user's email address,  
        /// otp: One time passcode sent to user,  
        /// }  
        /// 
        /// Example request body
        /// {
        ///  "email": "samuel@gmail.com",
        ///  "otp": "97954"
        /// }
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("verifyOtp")]
        [Produces(typeof(VerifyOtpResponse))]
        [ProducesErrorResponseType(typeof(ErrorDetails))]
        public async Task<ActionResult<VerifyOtpResponse>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            return Ok(await _authenticationService.VerifyOtp(request));
        }

        /// <summary>
        /// This end point revalidates and refreshes the token after token has expired
        /// </summary>
        /// <param name="request">This is the request payload: RefreshTokenRequest model</param>
        /// <returns>RefreshTokenResponse model</returns>
        /// <remarks>
        /// The request body for refreshing access token
        /// {  
        /// email: user's email address,  
        /// jwtToken: expired access token,  
        /// refreshtoken: user's refresh token
        /// }  
        /// 
        /// Example request body
        /// {
        ///  "email": "samuel@gmail.com",
        ///  "jwtToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InNhbXVlbG9sYXdhbGViYWJhdHVuZGVAZ21haWwuY29tIiwibmJmIjoxNjQ2MTI3MDE4LCJleHAiOjE2NDYxMjgyMTgsImlhdCI6MTY0NjEyNzAxOH0.05V2uLvZ-WJEQmcua3PyR9Yxr8i5r-JPXBuaE7SFXTc"
        ///  "refreshtoken": "9gEikFcSnPONIHAytBiezq+MaetcRpy3w6s4TxTRa/o="
        /// }
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        [Produces(typeof(RefreshTokenResponse))]
        [ProducesErrorResponseType(typeof(ErrorDetails))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {

            return Ok(await _authenticationService.RefreshToken(request));
        }
    }
}
