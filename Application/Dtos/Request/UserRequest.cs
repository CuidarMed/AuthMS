using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Request
{
    public class UserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Dni { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Patient" o "Doctor"
        public string ImageUrl { get; set; } // URL de la imagen de perfil
    }
}
