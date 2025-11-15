using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.ICommand;
using Application.Interfaces.IQuery;
using Application.Interfaces.IServices.IUserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.UserServices
{
    public class UserPutServices : IUserPutServices
    {
        private readonly IUserQuery _userQuery;
        private readonly IUserCommand _userCommand;

        public UserPutServices(IUserQuery userQuery, IUserCommand userCommand)
        {
            _userQuery = userQuery;
            _userCommand = userCommand;
        }

        public async Task<UserResponse> UpdateUser(int Id, UserUpdateRequest request)
        {
            var user = await _userQuery.GetUserById(Id);

            if (user == null)
            {
                throw new NotFoundException("No se encontró ningún usuario con el ID  " + Id);
            }
            
            if (user.Email != request.Email)
            {
                await CheckEmailExist(request.Email);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Dni = request.Dni;
            
            // Validar y procesar ImageUrl (aceptar URLs normales o data URLs)
            var imageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                ? user.ImageUrl // Mantener la imagen actual si no se proporciona una nueva
                : request.ImageUrl.Trim();
            
            // Validar tamaño máximo de data URLs (2MB de imagen comprimida ≈ ~2.7MB en base64)
            if (imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                // Limitar data URLs a aproximadamente 3MB (para imágenes comprimidas)
                const int maxDataUrlLength = 3 * 1024 * 1024; // 3MB
                if (imageUrl.Length > maxDataUrlLength)
                {
                    throw new InvalidValueException("La imagen es demasiado grande. Por favor, comprime la imagen o usa una URL externa.");
                }
            }
            
            user.ImageUrl = imageUrl;

            await _userCommand.Update(user);

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
