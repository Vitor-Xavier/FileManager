using FileManager.Context;
using FileManager.DTO;
using FileManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Repositories.Users
{
    public class UserRepository : Repository<User, FileManagerContext>, IUserRepository
    {
        public UserRepository(FileManagerContext context) : base(context) { }

        public Task<User> GetUserByUsername(string username, CancellationToken cancellationToken = default) =>
            _context.Users.Where(user => user.Username == username).AsNoTracking().SingleOrDefaultAsync(cancellationToken);

        public Task<bool> UsernameIsDefined(string username, CancellationToken cancellationToken = default) =>
            _context.Users.AsNoTracking().AnyAsync(user => user.Username == username, cancellationToken);

        public Task<UserAuthDto> GetUserByUsernamePassword(string username, string password, CancellationToken cancellationToken = default) =>
            _context.Users.Where(user => user.Username == username && user.Password == password && !user.Deleted).AsNoTracking().Select(user => new UserAuthDto(user.Username, user.Password, null)).SingleOrDefaultAsync(cancellationToken);
    }
}
