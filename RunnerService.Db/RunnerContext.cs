using Microsoft.EntityFrameworkCore;
using RunnerService.API;
using RunnerService.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunnerService.Db
{
    public class RunnerContext : DbContext
    {
        public DbSet<TestRunInfo> TestRuns { get; set; }
        public DbSet<Result> RunResults { get; set; }

        public RunnerContext(DbContextOptions<RunnerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StateBase>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder.Entity<StateBase>()
                .HasKey("_Id");
            //builder.Entity<StateBase>()
            //    .Property(s => s.State)
            //    .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Entity<StateBase>()
                .HasDiscriminator(r => r.State)
                .HasValue<ReadyState>(State.Ready)
                .HasValue<RunningState>(State.Running)
                .HasValue<JustCreatedState>(State.JustCreated);

            builder.Entity<Result>()
               .HasOne(r => r.ResultBase);
            //builder.Entity<RunResultBase>()
            //    .Property<int>("_Id")
            //    .UseIdentityColumn();
            //builder.Entity<RunResultBase>()
            //    .HasKey("_Id");

            //builder.Entity<Result>()
            //    .HasOne<RunResultBase>();
            //builder.Entity<RunResultBase>()
            //    .Property(r => r.Result)
            //    .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Entity<RunResultBase>()
                .HasDiscriminator(r => r.Result)
                .HasValue<PassedResult>(RunResult.Passed)
                .HasValue<AbortedResult>(RunResult.Aborted)
                .HasValue<RunnerErrorResult>(RunResult.RunnerError)
                .HasValue<PendingCompletionResult>(RunResult.Running)
                .HasValue<SUTErrorResult>(RunResult.SUTError);

            builder.Entity<RunPlanBase>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder.Entity<RunPlanBase>()
                .HasKey("_Id");
            builder.Entity<RunPlanBase>()
                .HasDiscriminator(r => r.RunPlan)
                .HasValue<ManualRunPlan>(RunPlan.Manual)
                .HasValue<PeriodicRunPlan>(RunPlan.Periodic);

            base.OnModelCreating(builder);
        }
    }
}
