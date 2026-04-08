using Api.Services;
using Application;
using Application.DependencyInjections;
using Application.Interfaces;
using Infrastructure.Postgres;
using Infrastructure.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog; // BỔ SUNG USING NÀY
using System.Text;

// ==========================================
// 0. KHỞI TẠO SERILOG TỪ SỚM (Bắt lỗi khởi động)
// ==========================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("🚀 Đang khởi động Todo API Server...");
    var builder = WebApplication.CreateBuilder(args);

    // Kích hoạt Serilog đọc cấu hình từ appsettings.json và thay thế bộ Log mặc định
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration));

    // ==========================================
    // 1. ĐĂNG KÝ CÁC TẦNG KIẾN TRÚC
    // ==========================================
    builder.Services.AddInfrastructurePostgres(builder.Configuration);
    builder.Services.AddInfrastructureRedis(builder.Configuration);
    builder.Services.AddApplication();

    // ==========================================
    // 2. ĐĂNG KÝ DỊCH VỤ PHỤ TRỢ & CẤU HÌNH API
    // ==========================================
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // BỔ SUNG: Cấu hình CORS (Cho phép Frontend gọi API mà không bị chặn)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // ==========================================
    // 3. CẤU HÌNH AUTHENTICATION & JWT BEARER
    // ==========================================
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    // ==========================================
    // 4. CẤU HÌNH SWAGGER (HỖ TRỢ NHẬP TOKEN)
    // ==========================================
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Nhập 'Bearer [khoảng trắng] [token của bạn]'. Ví dụ: Bearer eyJhbGciOi...",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
    });

    var app = builder.Build();

    // BỔ SUNG: Kích hoạt ghi log cho TỪNG CÚ CLICK GỌI API
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // BỔ SUNG: Áp dụng rule CORS đã tạo ở trên
    app.UseCors("AllowAll");

    // ==========================================
    // 5. KÍCH HOẠT MIDDLEWARE BẢO MẬT
    // ==========================================
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "💥 API bị sập ngay lúc khởi động! Hãy kiểm tra lại kết nối Database hoặc Redis.");
}
finally
{
    // Đảm bảo mọi log cuối cùng được đẩy đi trước khi app tắt hẳn
    Log.CloseAndFlush();
}