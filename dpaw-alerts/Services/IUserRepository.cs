using System;
using System.Collections;
using System.Collections.Generic;
using dpaw_alerts.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace dpaw_alerts.Services
{
    public interface IUserRepository : IDisposable
    {

        Task<ApplicationUser> GetUserByNameAsync(string username);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task CreateAsync(ApplicationUser user, string password);
        Task DeleteAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task AddClaimAsync(ApplicationUser user, string cc);
        bool VerifyUserPassword(string hashedPassword, string providedPassword);
        string HashPassword(string password);

        Task AddUserToRoleAsync(ApplicationUser newUser, string p);

        Task<IEnumerable<string>> GetRolesForUserAsync(ApplicationUser user);
        ApplicationUser GetUserByName(string username);
        IEnumerable<string> GetRolesForUser(ApplicationUser user);
        Task RemoveUserFromRoleAsync(ApplicationUser user, params string[] roleNames);

        Task<ApplicationUser> GetLoginUserAsync(string username, string password);

        Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user);
    }
}