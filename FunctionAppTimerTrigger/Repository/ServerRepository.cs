


using FunctionAppAriel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FunctionAppTimerTrigger
{
    public class ServerRepository : IServerRepository
    {
        private AppDbContext _context;

        private DbContextOptions<AppDbContext> GetAllOptions()
        {
            DbContextOptionsBuilder<AppDbContext> optionsBuilder = 
                            new DbContextOptionsBuilder<AppDbContext>();


            string connectionString = Environment.GetEnvironmentVariable("DefaultConnection", EnvironmentVariableTarget.Process);
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }

        public async Task<List<Account>> GetAccounts()
        {
            using (_context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                    var accounts = await _context.Accounts.ToListAsync();
                    return accounts;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
