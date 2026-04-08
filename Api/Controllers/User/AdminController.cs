using Application.Dtos;
using Infrastructure.Postgres.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// ==========================================
// THÊM MỚI: CHỈ NHỮNG TOKEN CÓ ROLE "Admin" MỚI ĐƯỢC GỌI API NÀY
// ==========================================
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    // Trong thực tế, Admin sẽ có các Command/Query Handler riêng,
    // ở đây dùng trực tiếp DbContext để test cho nhanh.
    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        // Trả về danh sách tất cả user trong hệ thống (Tính năng của Admin)
        var users = await _context.Users
            .Select(u => new { u.Id, u.Email, u.FullName })
            .ToListAsync();

        return Ok(users);
    }
}