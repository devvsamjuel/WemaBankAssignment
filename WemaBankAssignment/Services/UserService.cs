using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment;
using WemaBankAssignment.Data;
using WemaBankAssignment.Interfaces;
using WemaBankAssignment.Models;

namespace WemaBankAssignment.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Customer> GetCustomer(string userId)
        {
            var employee = await _userManager.FindByIdAsync(userId);
            return new Customer
            {
                Email = employee.Email,
                Id = employee.Id,
                Firstname = employee.FirstName,
                Lastname = employee.LastName
            };
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var employees = await _userManager.GetUsersInRoleAsync(UserRole.Customer.ToString());
            return employees.Select(q => new Customer
            {
                Id = q.Id,
                Email = q.Email,
                Firstname = q.FirstName,
                Lastname = q.LastName
            }).ToList();
        }
    }
}
