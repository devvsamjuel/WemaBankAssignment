using System;
using System.Threading.Tasks;

namespace WemaBankAssignment.Interfaces
{
    public interface IUtilityService
    {
        Task SaveApiLogs(string userId, string token, DateTime? requestTime, DateTime? responseTime, string Url, string Response, string Request, string StatusCode, string StatusMessage, string Vendor, string RefId, string longitude, string latititude);
    }
}