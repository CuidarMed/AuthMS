using Domain.Entities;
using System;

namespace Application.Dtos.Response
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Dni { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEndDate { get; set; }



        public static explicit operator UserResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Dni = user.Dni,
                ImageUrl = user.ImageUrl,
                Role = user.Role,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                AccessFailedCount = user.AccessFailedCount,
                LockoutEndDate = user.LockoutEndDate
            };
        }
    }
}
