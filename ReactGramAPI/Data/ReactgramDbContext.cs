using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReactGramAPI.Models;

namespace ReactGramAPI.Data;

public class ReactgramDbContext : IdentityDbContext<User>
{
    public ReactgramDbContext(DbContextOptions<ReactgramDbContext> options) : base(options) { }

    public DbSet<Photo> Photo { get; set; }
    public DbSet<Comment> Comment { get; set; }
    public DbSet<Like> Like { get; set; }
}
