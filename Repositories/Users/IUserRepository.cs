using FileManager.DTO;
using FileManager.Models;

namespace FileManager.Repositories.Users
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default);

        Task<bool> UsernameIsDefined(string username, CancellationToken cancellationToken = default);

        Task<UserAuthDto> GetUserByUsernamePassword(string username, string password, CancellationToken cancellationToken = default);
    }
}
