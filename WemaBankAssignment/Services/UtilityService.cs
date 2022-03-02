using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WemaBankAssignment.Entities;
using WemaBankAssignment.Interfaces;

namespace WemaBankAssignment.Services
{
    public class UtilityService : IUtilityService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UtilityService> log;

        public UtilityService(ApplicationDbContext context,
            ILogger<UtilityService> _log
            )
        {
            _context = context;
            log = _log;
        }


        public async Task SaveApiLogs(string userId, string token, DateTime requestTime, DateTime responseTime, string Url, string Response, string Request, string StatusCode, string StatusMessage, string Vendor, string RefId, string longitude, string latititude)
        {
            try
            {
                var apiLogs = new ApiLogs();
                apiLogs.UserId = userId;
                apiLogs.AuthToken = token;
                apiLogs.Vendor = Vendor;
                apiLogs.Request = Request;
                apiLogs.Response = Response;
                apiLogs.RequestTime = requestTime;
                apiLogs.ResponseTime = responseTime;
                apiLogs.StatusCode = StatusCode;
                apiLogs.StatusMessage = StatusMessage;
                apiLogs.Url = Url;
                apiLogs.Longitude = longitude;
                apiLogs.Latitude = latititude;
                apiLogs.ReferenceId = RefId;
                await _context.AddAsync(apiLogs);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                log.LogInformation(string.Concat("Error occured in the SaveApiLogs ==>  ", ex.ToString()));
            }
        }

    }
}
