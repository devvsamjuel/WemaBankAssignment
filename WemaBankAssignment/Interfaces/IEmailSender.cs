using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Models;

namespace WemaBankAssignment.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmail(Email email);
    }
}
