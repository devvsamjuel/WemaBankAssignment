using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WemaBankAssignment.Entities;
using WemaBankAssignment.Interfaces;

namespace WemaBankAssignment.Repositories
{
    public class LgaRepository : GenericRepository<Lga>, ILgaRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LgaRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Lga>> GetAllLgaDetails()
        {
            var lgas = await _dbContext.Lgas
                .Include(q => q.State)
                .ToListAsync();
            return lgas;
        }

        public async Task<Lga> GetLgaDetails(int id)
        {
            var lga = await _dbContext.Lgas
                .Include(q => q.State)
                .FirstOrDefaultAsync(q => q.Id == id);

            return lga;
        }
    }
}
