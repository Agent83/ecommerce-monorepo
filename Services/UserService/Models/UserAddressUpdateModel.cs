using UserService.Enum;

namespace UserService.Models
{
    public class UserAddressUpdateModel
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }
    }
}