using Microsoft.EntityFrameworkCore;
using RealState.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RealState.DbContexts
{
    public class GlobalDbContext : IdentityDbContext<User>
    {
        public GlobalDbContext(DbContextOptions<GlobalDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
    }
}
