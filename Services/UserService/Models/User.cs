using Microsoft.AspNetCore.Identity;

namespace UserService.Models
{
    public class User: IdentityUser
    {
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }

    }
}
