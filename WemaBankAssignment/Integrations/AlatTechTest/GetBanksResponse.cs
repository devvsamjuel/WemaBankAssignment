using System;

namespace WemaBankAssignment.Integrations.AlatTechTest
{
    public class GetBanksResponse
    {
        public Bank[] result { get; set; }
        public object errorMessage { get; set; }
        public object errorMessages { get; set; }
        public bool hasError { get; set; }
        public DateTime timeGenerated { get; set; }
    }

    public class Bank
    {
        public string bankName { get; set; }
        public string bankCode { get; set; }
    }

}
