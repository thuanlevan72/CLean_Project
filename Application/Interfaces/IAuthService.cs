using Application.Dtos;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(string email, string password);
    Task<AuthResultDto> RegisterAsync(string email, string password, string fullName);
}