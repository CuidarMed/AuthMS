using Application.Interfaces.IServices.ICryptographyService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Seeders
{
    public class DefaultAdminSeeder
    {
        private readonly AppDbContext _context;
        private readonly ICryptographyService _cryptographyService;
        private readonly ILogger<DefaultAdminSeeder> _logger;

        private const string DefaultAdminEmail = "admin@cuidarmed.com";
        private const string DefaultAdminPassword = "Admin123!";

        public DefaultAdminSeeder(
            AppDbContext context,
            ICryptographyService cryptographyService,
            ILogger<DefaultAdminSeeder> logger)
        {
            _context = context;
            _cryptographyService = cryptographyService;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == DefaultAdminEmail);
            if (exists)
            {
                _logger.LogInformation("Usuario administrador por defecto ya existe ({Email})", DefaultAdminEmail);
                return;
            }

            var hashedPassword = await _cryptographyService.HashPassword(DefaultAdminPassword);

            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "CuidarMed",
                Email = DefaultAdminEmail,
                Dni = "ADMIN01",
                Password = hashedPassword,
                Role = UserRoles.Admin,
                ImageUrl = "https://icons.veryicon.com/png/o/internet--web/prejudice/user-128.png",
                IsActive = true,
                IsEmailVerified = true
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Usuario administrador por defecto creado ({Email})", DefaultAdminEmail);
        }
    }
}




