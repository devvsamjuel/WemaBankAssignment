using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WemaBankAssignment.Entities
{
    public class ApiLogs
    {
        [Key]
        public long Id { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public DateTime? RequestTime { get; set; }
        public DateTime? ResponseTime { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string Url { get; set; }
        public string ReferenceId { get; set; }
        public string Vendor { get; set; }
        public string AuthToken { get; set; }
        public string UserId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
