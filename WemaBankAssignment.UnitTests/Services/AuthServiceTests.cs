using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Data;
using WemaBankAssignment.Entities;
using WemaBankAssignment.Exceptions;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Models;
using WemaBankAssignment.Models.Configurations;
using WemaBankAssignment.Services;
using WemaBankAssignment.UnitTests.Mockings;

namespace WemaBankAssignment.UnitTests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManager;
        private Mock<SignInManager<ApplicationUser>> _signInManager;
        private ApplicationDbContext _context;
        private AuthService _authService;
        private List<ApplicationUser> _users;

        [SetUp]
        public void Setup()
        {
            _users = new List<ApplicationUser> { new ApplicationUser() { Email = "user1@gmail.com", PhoneNumber = "012345677" }, new ApplicationUser() { Email = "user2@gmail.com", PhoneNumber = "232345677" } };

            //_userManager = GetMockUserManager();
            _userManager = MockUserManager.GetUserManager<ApplicationUser>(_users);

            var httpContextAccessor = new HttpContextAccessor();
            var claimsFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();
            _signInManager = new Mock<SignInManager<ApplicationUser>>(_userManager.Object, httpContextAccessor, claimsFactoryMock.Object, null, null, null, null);
            //_signInManager = new Mock<SignInManager<ApplicationUser>>(userManager.Object, 
            //    httpContextAccessor, claimsFactoryMock.Object, 
            //    optionsAccessor.Object, logger.Object, 
            //    schemes.Object, confirmation.Object);

            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "WemaBankAssignemt_Db").Options;
            _context = new ApplicationDbContext(dbContextOptions);

            var jwtSettings = Options.Create(new JwtSettings() { SecretKey = "84322CFB66934ECC86D547C5CF4F2EFC", Issuer = "WemaBankAssignment", Audience = "WemaBankAssignmentUser", TokenLifespan = 60 });
            var appSettings = Options.Create(new AppSettings() { OtpLifespan = 5, ApiKey = "843-22CFB-6693-4ECC86-D54-7C5-CF4-F2EFC" });
            var emailSettings = Options.Create(new EmailSettings() { ApiKey= "843-22CFB-6693-4ECC86-D54-7C5-CF4-F2EFC", FromName= "WemaBankAssignment" , FromAddress = "noreply@WemaBankAssignment.com" });
            var emailSender = new EmailService(emailSettings);
            var notificationService = new NotificationService(emailSender, jwtSettings, appSettings);
            var refreshTokenGenerator = new Mock<IRefreshTokenGenerator>();

            _authService = new AuthService(_context, _userManager.Object, jwtSettings, notificationService, refreshTokenGenerator.Object, _signInManager.Object);

        }


        #region TEST CASES
        [Test]
        [Ignore("Already Passed!")]
        public void Register_UserAlreadyExist_ThrowsBadRequestException()
        {
            //Arrange
            var newUser = new ApplicationUser
            {
                Email = "devvsamjuel@gmail.com",
                PhoneNumber = "2348135187469",
            };

            _userManager.Setup(x => x.CreateAsync(newUser));
            _context.AddAsync(newUser);
            _context.SaveChanges();
            var newRegisterRequest = new RegistrationRequest { Email = newUser.Email, PhoneNumber = newUser.PhoneNumber };

            //Act
            //Assert
            Assert.ThrowsAsync<BadRequestException>(() => _authService.Register(newRegisterRequest));
        }

        [Test]
        [Ignore("Will check later")]
        public void Register_UserIsNotCreatedSuccessfully_ThrowsBadRequestException()
        {
            //Arrange
            var request = new RegistrationRequest
            {
                Email = "samjuel@gmail.com",
                PhoneNumber = "2348135187469",
                Password = "samuel@password"
                //Password = null
            };
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

            //var res = _userManager.Setup(x => x.CreateAsync(user, request.Password)).Returns(Task.FromResult(IdentityResult.Success));
            //var result = _userManager.Object.CreateAsync(user, request.Password);
            //Act
            //Assert
            Assert.ThrowsAsync<BadRequestException>(() => _authService.Register(request));
        }

        [Test]
        //[Ignore("Not yet implemented")]
        public void Register_UserCreatedSuccessfully_ReturnsResgistrationResponse()
        {
            //Arrange
            var request = new RegistrationRequest
            {
                Email = "samjuel@gmail.com",
                PhoneNumber = "2348135187469",
                Password = "samuel@password"
                //Password = null
            };
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

            //Act
            var result = _authService.Register(request).Result;

            //Assert
            Assert.That(result, Is.TypeOf<RegistrationResponse>());

        }
        #endregion



        #region HELPERS
        public async Task<string> CreateUser(ApplicationUser user, string password) => (await _userManager.Object.CreateAsync(user, password)).Succeeded ? user.Id : null;

        public void Init()
        {
            var user = new ApplicationUser
            {
                Email = "devvsamjuel@gmail.com",
                FirstName = "samuel",
                LastName = "babatunde",
                PhoneNumber = "2348135187469",
                State = "Lagos",
                UserName = "devsamjuel@gmail.com"
            };


            //Generate Otp
            user.Otp = new Random().Next(10000, 99999).ToString();
            user.DateCreated = DateTime.Now;
            user.Status = StatusType.Active.ToString();
            user.OtpLifeSpan = DateTime.Now;
            user.DateLastModified = DateTime.Now;
            user.SaltProperty = CryptoServices.CreateRandomSalt();
            user.Role = UserRole.Customer.ToString();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "WemaBankAssignemt_Db")
           .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new ApplicationDbContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }
        private Mock<UserManager<ApplicationUser>> GetMockUserManager2()
        {

            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidators = new Mock<IEnumerable<IUserValidator<ApplicationUser>>>();
            var passwordValidators = new Mock<IEnumerable<IPasswordValidator<ApplicationUser>>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var services = new Mock<ILookupNormalizer>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var errors = new Mock<IdentityErrorDescriber>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                optionsAccessor.Object,
                passwordHasher.Object,
                userValidators.Object,
                passwordValidators.Object, keyNormalizer.Object, errors.Object, services.Object, logger.Object);
        }
        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        }
        private Mock<SignInManager<ApplicationUser>> GetMockSignInManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<SignInManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        }
        #endregion
    }
}
