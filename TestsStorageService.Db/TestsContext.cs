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
                .Entity<TestCaseData>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder
                .Entity<TestCaseData>()
                .HasKey("_Id");

            builder
                .Entity<KeyParameter>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder
                .Entity<KeyParameter>()
                .HasKey("_Id");

            builder
                .Entity<TestCase>()
                .HasQueryFilter(p => !p.IsDeleted);
            
            base.OnModelCreating(builder);
        }
    }
}
