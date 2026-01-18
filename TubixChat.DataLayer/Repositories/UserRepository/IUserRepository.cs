using System.Collections.Generic;
using System.Threading.Tasks;
using TubixChat.DataLayer.Entities;

namespace TubixChat.DataLayer.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<bool> UserExistsAsync(string userName);
    Task<IEnumerable<User>> SearchUsersAsync(string searchText);
    Task<IEnumerable<User>> GetActiveUsersAsync();
}
