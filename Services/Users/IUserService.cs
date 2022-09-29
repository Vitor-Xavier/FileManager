using FileManager.DTO;
using FileManager.Models;

namespace FileManager.Services.Users
{
    public interface IUserService
    {
        ValueTask<User> GetUserById(int userId, CancellationToken cancellationToken = default);

        Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default);

        ValueTask<UserAuthDto> Authenticate(string username, string password, CancellationToken cancellationToken = default);

        Task CreateUser(User user, CancellationToken cancellationToken = default);

        Task UpdateUser(int userId, User user, CancellationToken cancellationToken = default);

        Task DeleteUser(int userId, CancellationToken cancellationToken = default);
    }
}
