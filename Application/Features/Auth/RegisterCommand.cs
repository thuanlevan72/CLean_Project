using Application.Dtos;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Auth;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<AuthResultDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(request.Email, request.Password, request.FullName);
    }
}