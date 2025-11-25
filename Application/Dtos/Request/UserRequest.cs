using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Request
{
    public class UserRequest
    {
        // Datos obligatorios
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Dni { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Patient" o "Doctor"
        public string ImageUrl { get; set; } // URL de la imagen de perfil
        public string Phone { get; set; }
        // Datos opcionales

        // Datos de Patient
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? HealthPlan { get; set; }
        public string? MembershipNumber { get; set; }

        // Datos de Doctor
        public string? LicenseNumber { get; set; }
        public string? Biography { get; set; }
        public string? Specialty { get; set; }

    }
}
