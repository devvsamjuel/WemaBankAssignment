using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WemaBankAssignment.Entities;

namespace WemaBankAssignment.Interfaces
{
    public interface IStateRepository : IGenericRepository<State>
    {
        Task<State> GetStateDetails(int id);
        Task<List<State>> GetAllStateDetails();
    }
}
