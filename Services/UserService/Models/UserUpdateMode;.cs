using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class UserUpdateModel
    {
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; }

        public string CurrentPassword { get; set; }

        public UserAddressUpdateModel Address { get; set; }  // For updating address info
    }
}
