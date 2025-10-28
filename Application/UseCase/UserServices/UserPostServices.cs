using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.ICommand;
using Application.Interfaces.IQuery;
using Application.Interfaces.IServices.IAuthServices;
using Application.Interfaces.IServices.ICryptographyService;
using Application.Interfaces.IServices.IUserServices;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserServices
{
    public class UserPostServices : IUserPostServices
    {
        private readonly IUserQuery _userQuery;
        private readonly IUserCommand _userCommand;
        private readonly ICryptographyService _cryptographyService;
        private readonly IEmailVerificationService _emailVerificationService;

        public UserPostServices(
            IUserQuery userQuery, 
            IUserCommand userCommand, 
            ICryptographyService cryptographyService, 
            IEmailVerificationService emailVerificationService
        )
        {
            _userQuery = userQuery;
            _userCommand = userCommand;
            _cryptographyService = cryptographyService;
            _emailVerificationService = emailVerificationService;
        }

        public async Task<UserResponse> Register(UserRequest request)
        {
            await CheckEmailExist(request.Email);
            var hashedPassword = await _cryptographyService.HashPassword(request.Password);

            var user = new User
            {
                Role = UserRoles.User,
                IsActive = false,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Dni = request.Dni,
                Password = hashedPassword,
                ImageUrl = "https://icons.veryicon.com/png/o/internet--web/prejudice/user-128.png",
            };            

            await _userCommand.Insert(user);
            await _emailVerificationService.SendVerificationEmail(user.Email);

            return new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Dni = user.Dni,
                ImageUrl = user.ImageUrl,
                Role = user.Role
            };           
        }

        private async Task CheckEmailExist(string email)
        {
            var emailExist = await _userQuery.ExistEmail(email);

            if (emailExist)
            {
                throw new InvalidEmailException("El correo electrónico ingresado ya está registrado.");
            }
        }
    }
}
