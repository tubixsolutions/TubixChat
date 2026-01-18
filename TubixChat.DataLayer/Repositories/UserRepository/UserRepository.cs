using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TubixChat.DataLayer.Context;
using TubixChat.DataLayer.Entities;

namespace TubixChat.DataLayer.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EfCoreContext context) : base(context)
        {
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _dbSet
                .Include(u => u.State)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> UserExistsAsync(string userName)
        {
            return await _dbSet.AnyAsync(u => u.UserName == userName);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchText)
        {
            return await _dbSet
                .Include(u => u.State)
                .Where(u => u.StateId == 1 &&
                    (u.UserName.Contains(searchText) ||
                     u.FullName.Contains(searchText)))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Include(u => u.State)
                .Where(u => u.StateId == 1)
                .ToListAsync();
        }
    }
}
