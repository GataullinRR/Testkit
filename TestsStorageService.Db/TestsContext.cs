using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TestsStorageService.API;

namespace TestsStorageService.Db
{
    public class TestsContext : DbContext
    {
        public DbSet<TestCase> Cases { get; set; }

        public TestsContext(DbContextOptions<TestsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<TestCase>()
                .OwnsOne(p => p.Data);

            base.OnModelCreating(builder);
        }
    }
}
