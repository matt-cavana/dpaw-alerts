using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using dpaw_alerts.Models;

namespace dpaw_alerts.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly AlertsUserStore _store;
        private readonly ColsUserManager _manager;

        public UserRepository()
        {
            _store = new AlertsUserStore();
            _manager = new ColsUserManager(_store);
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string username)
        {
            return await _store.FindByNameAsync(username);
        }

        public ApplicationUser GetUserByName(string username)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByName(username);
            return (user);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _store.FindByIdAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _store.Users.ToArrayAsync();
        }

        public async Task CreateAsync(ApplicationUser user, string password)
        {
            await _manager.CreateAsync(user, password);
        }
        
        public async Task AddClaimAsync(ApplicationUser user, string cc)
        {
            await _manager.AddClaimAsync(user.Id, new Claim(ClaimTypes.UserData, cc));
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            await _manager.DeleteAsync(user);
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            await _manager.UpdateAsync(user);
        }

        public bool VerifyUserPassword(string hashedPassword, string providedPassword)
        {
            return _manager.PasswordHasher.VerifyHashedPassword(hashedPassword, providedPassword) ==
                   PasswordVerificationResult.Success;
        }

        public string HashPassword(string password)
        {
            return _manager.PasswordHasher.HashPassword(password);
        }

        public async Task AddUserToRoleAsync(ApplicationUser user, string role)
        {
            await _manager.AddToRoleAsync(user.Id, role);
        }

        public async Task<IEnumerable<string>> GetRolesForUserAsync(ApplicationUser user)
        {
            return await _manager.GetRolesAsync(user.Id);
        }

        public IEnumerable<string> GetRolesForUser(ApplicationUser user)
        {
            return  _manager.GetRoles(user.Id);
        }

        public async Task RemoveUserFromRoleAsync(ApplicationUser user, params string[] roleNames)
        {
            await _manager.RemoveFromRolesAsync(user.Id, roleNames);
        }

        public async Task<ApplicationUser> GetLoginUserAsync(string username, string password)
        {
            return await _manager.FindAsync(username, password);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user)
        {
            return await _manager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _manager.Dispose();
                _store.Dispose();
            }

            _disposed = true;
        }
    }
}