using Application.Dtos.Request;
using Application.Dtos.Response;
using Application.Exceptions;
using Application.Interfaces.ICommand;
using Application.Interfaces.IQuery;
using Application.Interfaces.IServices.ICryptographyService;
using Application.Interfaces.IServices.IUserServices;
using Application.Interfaces.Messaging;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Logging;


namespace Application.UseCase.UserServices
{
    public class UserPostServices : IUserPostServices
    {
        private readonly IUserQuery _userQuery;
        private readonly IUserCommand _userCommand;
        private readonly ICryptographyService _cryptographyService;      
        private readonly ILogger<UserPostServices> _logger;
        private readonly IEventBus _eventBus;

        public UserPostServices(
            IUserQuery userQuery, 
            IUserCommand userCommand, 
            ICryptographyService cryptographyService, 
            ILogger<UserPostServices> logger,
            IEventBus eventBus
        )
        {
            _userQuery = userQuery;
            _userCommand = userCommand;
            _cryptographyService = cryptographyService;
            _logger = logger;
            _eventBus = eventBus;
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

            // Validar tamaño máximo de data URLs (2MB de imagen comprimida ≈ ~2.7MB en base64)
            if (imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                // Limitar data URLs a aproximadamente 3MB (para imágenes comprimidas)
                const int maxDataUrlLength = 3 * 1024 * 1024; // 3MB
                if (imageUrl.Length > maxDataUrlLength)
                {
                    imageUrl = defaultImageUrl;
                }
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
            
            _logger.LogInformation("Guardando usuario en base de datos AUTH. Email: {Email}, Role: {Role}", user.Email, user.Role);
            await _userCommand.Insert(user);

            await _eventBus.PublishAsync(
                new UserCreatedEvent
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Dni = user.Dni,
                    Password = user.Password,
                    ImageUrl = imageUrl,
                    IsEmailVerified = user.IsEmailVerified,
                    AccessFailedCount = user.AccessFailedCount,
                    LockoutEndDate = user.LockoutEndDate
                },
                routingKey: "user.created"
            );

            _logger.LogInformation("Usuario guardado exitosamente en base de datos AUTH. UserId: {UserId}, Email: {Email}", user.UserId, user.Email);
            
            
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
