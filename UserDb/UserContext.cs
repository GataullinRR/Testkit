using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace UserServiceDb
{
    public class UserContext : IdentityDbContext<User>
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Dont know why but i'm gettin NullRefEx at Database.EnsureCreated(); without this line...
            // It is started from Commit 5dacbd29
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(b => { }));
        }
    }
}
