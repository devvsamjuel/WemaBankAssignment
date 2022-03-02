using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Entities;

namespace WemaBankAssignment.Interfaces
{
    public interface ILgaRepository : IGenericRepository<Lga>
    {
        Task<Lga> GetLgaDetails(int id);
        Task<List<Lga>> GetAllLgaDetails();
    }
}
