using LiteDB;
using Microsoft.AspNetCore.Identity;
using Sharp.Redux.HubServer.Core;
using Sharp.Redux.HubServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubServer.Services
{
    public class UserStore : DisposableObject, IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        readonly LiteCollection<ApplicationUser> users;
        public UserStore(LiteDatabase db)
        {
            users = db.GetCollection<ApplicationUser>();
            users.EnsureIndex(u => u.Id, unique: true);
            users.EnsureIndex(u => u.NormalizedUserName);
        }
        public Task<IdentityResult> CreateAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            ApplicationUser.Id = ApplicationUser.Email;
            try
            {
                users.Insert(ApplicationUser);
            }
            catch
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = $"Could not insert ApplicationUser {ApplicationUser.Email}." }));
            }
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            try
            {
                users.Delete(ApplicationUser.Id);
            }
            catch
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = $"Could not delete ApplicationUser {ApplicationUser.Email}." }));
            }
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(IdentityResult.Success);

        }
        public Task<ApplicationUser> FindByIdAsync(string ApplicationUserId, CancellationToken cancellationToken)
        {
            return Task.FromResult(users.FindById(ApplicationUserId));
        }

        public Task<ApplicationUser> FindByNameAsync(string normalizedApplicationUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(users.FindOne(Query.EQ(nameof(ApplicationUser.NormalizedUserName), normalizedApplicationUserName)));
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(ApplicationUser.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(ApplicationUser.Id);
        }

        public Task<string> GetUserNameAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(ApplicationUser.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser ApplicationUser, string normalizedName, CancellationToken cancellationToken)
        {
            ApplicationUser.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser ApplicationUser, string ApplicationUserName, CancellationToken cancellationToken)
        {
            ApplicationUser.UserName = ApplicationUserName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            try
            {
                users.Update(ApplicationUser);
            }
            catch
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = $"Could not update ApplicationUser {ApplicationUser.Email}." }));
            }
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task SetPasswordHashAsync(ApplicationUser ApplicationUser, string passwordHash, CancellationToken cancellationToken)
        {
            ApplicationUser.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(ApplicationUser.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser ApplicationUser, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(ApplicationUser.PasswordHash));
        }
    }
}
