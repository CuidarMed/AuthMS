using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.IQuery;
using Application.Interfaces.IServices.IUserServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCase.UserServices
{
    public class UserGetServices : IUserGetServices
    {
        private readonly IUserQuery _userQuery;

        public UserGetServices(IUserQuery userQuery)
        {
            _userQuery = userQuery;
        }


        public async Task<UserResponse> GetUserById(int id)
        {
            var user = await _userQuery.GetUserById(id);

            if (user == null)
                throw new NotFoundException("No se encontró el usuario");

            return (UserResponse)user;
        }

        public async Task<IEnumerable<UserResponse>> GetUsers(string role = null, string search = null)
        {
            var normalizedRole = string.IsNullOrWhiteSpace(role) ? null : role.Trim();
            var users = await _userQuery.GetUsersAsync(normalizedRole, search);

            return users.Select(user => (UserResponse)user).ToList();
        }
    }
}
