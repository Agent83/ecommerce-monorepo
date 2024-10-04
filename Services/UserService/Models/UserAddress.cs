using UserService.Enum;

namespace UserService.Models
{
    public class UserAddress
    {
        public Guid Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public AddressType AddressType { get; set; }
        public string Country { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

    }
}