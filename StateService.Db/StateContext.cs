using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace StateService.Db
{
    public class StateContext : DbContext
    {
        public DbSet<StateInfo> States { get; set; }

        public StateContext(DbContextOptions<StateContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
