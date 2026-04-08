using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Postgres.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResultDto> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return new AuthResultDto { IsSuccess = false, Message = "Email hoặc mật khẩu không đúng." };
        }

        var token = await GenerateJwtTokenAsync(user);
        return new AuthResultDto { IsSuccess = true, Token = token, Message = "Đăng nhập thành công" };
    }

    public async Task<AuthResultDto> RegisterAsync(string email, string password, string fullName)
    {
        var userExists = await _userManager.FindByEmailAsync(email);
        if (userExists != null)
            return new AuthResultDto { IsSuccess = false, Message = "Email đã tồn tại." };

        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return new AuthResultDto { IsSuccess = false, Errors = result.Errors.Select(e => e.Description) };


        // ==========================================
        // THÊM MỚI: TỰ ĐỘNG GÁN QUYỀN "User" CHO TÀI KHOẢN MỚI
        // ==========================================
        await _userManager.AddToRoleAsync(user, "User");

        return new AuthResultDto { IsSuccess = true, Message = "Đăng ký thành công" };
    }

    private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Lấy danh sách quyền của User từ Database
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        // ==========================================
        // THÊM MỚI: NHÉT CÁC ROLE VÀO JWT TOKEN
        // ==========================================
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}