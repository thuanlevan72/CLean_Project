using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Postgres.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}