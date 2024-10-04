using UserService.Models;

namespace UserService.Interface
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}
