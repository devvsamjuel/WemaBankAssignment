using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Models.Configurations;
using WemaBankAssignment.Services;

namespace WemaBankAssignment.UnitTests.Services
{
    [TestFixture]
    public class AlatDevServiceTest
    {
        private Mock<IUtilityService> _utilityService;
        private Mock<ILogger<AlatDevService>> _log;
        private AlatDevService _alatDevService;

        [SetUp]
        public void Setup()
        {
            var jwtSettings = Options.Create(new JwtSettings() { SecretKey = "84322CFB66934ECC86D547C5CF4F2EFC", Issuer = "WemaBankAssignment", Audience = "WemaBankAssignmentUser", TokenLifespan = 60 });
            var appSettings = Options.Create(new AppSettings() { OtpLifespan = 5, ApiKey = "843-22CFB-6693-4ECC86-D54-7C5-CF4-F2EFC" });
            var emailSettings = Options.Create(new EmailSettings() { ApiKey = "843-22CFB-6693-4ECC86-D54-7C5-CF4-F2EFC", FromName = "WemaBankAssignment", FromAddress = "noreply@WemaBankAssignment.com" });
            var alatDevSettings = Options.Create(new AlatDevSettings()
            {
                GetBanksUrl = "https://wema-alatdev-apimgt.azure-api.net/alat-test/api/Shared/GetAllBanks",
                //GetBanksUrl = null,
                SubscriptionHeader = "Ocp-Apim-Subscription-Key",
                SubscriptionKey = "8878b2f2d31d4f5aad221a59754b45e7"
            });

            _utilityService = new Mock<IUtilityService>();
            _log = new Mock<ILogger<AlatDevService>>();
            //_utilityService.Setup(x=>x.SaveApiLogs(null,null, null, null, null, null,null,null,null,null,null,null,null));

            _alatDevService = new AlatDevService(alatDevSettings,_utilityService.Object, _log.Object);
        }

        [Test]
        public void GetBanks_ExceptionIsCaught_ReturnsGetBanksResponseWithWithHasErrorTrue()
        {
            //Arrange
            var alatDevSettings = Options.Create(new AlatDevSettings()
            {
                GetBanksUrl = null,
                SubscriptionHeader = "Ocp-Apim-Subscription-Key",
                SubscriptionKey = "8878b2f2d31d4f5aad221a59754b45e7"
            });

            _alatDevService = new AlatDevService(alatDevSettings, _utilityService.Object, _log.Object);
            //Act
            var result = _alatDevService.GetBanks().Result;

            //Assert
            Assert.That(result.hasError, Is.True);
        }

        [Test]
        public void GetBanks_ApiCallFails_ReturnsGetBanksResponseWithWithHasErrorTrue()
        {
            //Arrange
            var alatDevSettings = Options.Create(new AlatDevSettings()
            {
                GetBanksUrl = "wrongurl",
                SubscriptionHeader = "Ocp-Apim-Subscription-Key",
                SubscriptionKey = "8878b2f2d31d4f5aad221a59754b45e7"
            });

            _alatDevService = new AlatDevService(alatDevSettings, _utilityService.Object, _log.Object);
            //Act
            var result = _alatDevService.GetBanks().Result;

            //Assert
            Assert.That(result.hasError, Is.True);
        }


        [Test]
        public void GetBanks_ApiCallSucceeds_ReturnsGetBanksResponseWithWithHasErrorFalse()
        {
            //Arrange

            //Act
            var result = _alatDevService.GetBanks().Result;

            //Assert
            Assert.That(result.hasError, Is.False);
            Assert.That(result.result.Count(),Is.GreaterThan(0));
        }
    }
}
