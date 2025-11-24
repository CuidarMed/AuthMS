using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.ICommand;
using Application.Interfaces.IQuery;
using Application.Interfaces.IServices;
using Application.Interfaces.IServices.ICryptographyService;
using Application.Interfaces.IServices.IUserServices;
using Domain.Entities;
using Microsoft.Extensions.Logging;
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
        private readonly IDirectoryService _directoryService;
        private readonly ILogger<UserPostServices> _logger;

        public UserPostServices(
            IUserQuery userQuery, 
            IUserCommand userCommand, 
            ICryptographyService cryptographyService, 
            IDirectoryService directoryService,
            ILogger<UserPostServices> logger
        )
        {
            _userQuery = userQuery;
            _userCommand = userCommand;
            _cryptographyService = cryptographyService;
            _directoryService = directoryService;
            _logger = logger;
        }

        public async Task<UserResponse> Register(UserRequest request)
        {
            await CheckEmailExist(request.Email);
            var hashedPassword = await _cryptographyService.HashPassword(request.Password);

            // Validar y asignar el rol: si viene vacío o null, por defecto es Patient
            var role = string.IsNullOrWhiteSpace(request.Role) 
                ? UserRoles.Patient 
                : request.Role.Trim(); // Limpiar espacios en blanco

            // Validar que el rol sea válido (comparación case-sensitive)
            if (role != UserRoles.Patient && role != UserRoles.Doctor)
            {
                throw new InvalidValueException($"El rol '{role}' no es válido. Los roles permitidos son: '{UserRoles.Patient}' o '{UserRoles.Doctor}'");
            }
            
            _logger.LogInformation("Registrando usuario con rol: {Role}", role);

            const string defaultImageUrl = "https://icons.veryicon.com/png/o/internet--web/prejudice/user-128.png";

            // Usar ImageUrl del request si está disponible, de lo contrario usar valor por defecto
            var imageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                ? defaultImageUrl
                : request.ImageUrl.Trim();

            // Evitar guardar datos base64 o cadenas muy largas (columna varchar(500))
            if (imageUrl.Length > 500 || imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                imageUrl = defaultImageUrl;
            }

            var user = new User
            {
                Role = role,
                IsActive = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Dni = request.Dni,
                Password = hashedPassword,
                ImageUrl = imageUrl,
                IsEmailVerified = true,
            };            

            await _userCommand.Insert(user);
            
            // Si el usuario es un Patient, crear el registro en DirectoryMS
            if (user.Role == UserRoles.Patient)
            {
                _logger.LogInformation("Intentando crear paciente en DirectoryMS para UserId: {UserId}", user.UserId);
                var patientCreated = await _directoryService.CreatePatientAsync(user.UserId, user.FirstName, user.LastName, user.Dni);
                if (patientCreated)
                {
                    _logger.LogInformation("Paciente creado exitosamente en DirectoryMS para UserId: {UserId}", user.UserId);
                }
                else
                {
                    _logger.LogWarning("No se pudo crear el paciente en DirectoryMS para UserId: {UserId}. El usuario se creó exitosamente en AuthMS.", user.UserId);
                }
            }
            // Si el usuario es un Doctor, crear el registro en DirectoryMS
            else if (user.Role == UserRoles.Doctor)
            {
                _logger.LogInformation("Intentando crear doctor en DirectoryMS para UserId: {UserId}", user.UserId);
                var doctorCreated = await _directoryService.CreateDoctorAsync(user.UserId, user.FirstName, user.LastName);
                if (doctorCreated)
                {
                    _logger.LogInformation("Doctor creado exitosamente en DirectoryMS para UserId: {UserId}", user.UserId);
                }
                else
                {
                    _logger.LogWarning("No se pudo crear el doctor en DirectoryMS para UserId: {UserId}. El usuario se creó exitosamente en AuthMS.", user.UserId);
                }
            }
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
