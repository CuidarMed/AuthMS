using Application.Interfaces.IQuery;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Query
{
    public class UserQuery : IUserQuery
    {
        private readonly AppDbContext _context;

        public UserQuery(AppDbContext context) 
        {
            _context = context;
        }               

        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user;
        }

        public async Task<bool> ExistEmail(string email)
        {
            var result = await _context.Users.AnyAsync(u => u.Email == email);

            return result;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var result = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(string role = null, string search = null)
        {
            var query = _context.Users
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
            {
                var normalizedRole = role.Trim();
                query = query.Where(u => u.Role == normalizedRole);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(normalizedSearch) ||
                    u.LastName.ToLower().Contains(normalizedSearch) ||
                    u.Email.ToLower().Contains(normalizedSearch) ||
                    u.Dni.ToLower().Contains(normalizedSearch));
            }

            return await query.ToListAsync();
        }
    }
}
