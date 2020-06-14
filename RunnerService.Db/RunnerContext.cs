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
            builder.Entity<StateBase>()
                .HasDiscriminator(r => r.State)
                .HasValue<ReadyState>(State.Ready)
                .HasValue<RunningState>(State.Running)
                .HasValue<JustCreatedState>(State.JustCreated);

            builder.Entity<Result>()
               .HasOne(r => r.ResultBase);
            builder.Entity<RunResultBase>()
                .HasDiscriminator(r => r.Result)
                .HasValue<PassedResult>(RunResult.Passed)
                .HasValue<AbortedResult>(RunResult.Aborted)
                .HasValue<RunnerErrorResult>(RunResult.FatalError)
                .HasValue<PendingCompletionResult>(RunResult.Running)
                .HasValue<SUTErrorResult>(RunResult.Error);

            builder.Entity<RunPlanBase>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder.Entity<RunPlanBase>()
                .HasKey("_Id");
            builder.Entity<RunPlanBase>()
                .HasDiscriminator(r => r.RunPlan)
                .HasValue<ManualRunPlan>(RunPlan.Manual)
                .HasValue<PeriodicRunPlan>(RunPlan.Periodic);

            builder.Entity<StateInfo>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder.Entity<StateInfo>()
                .HasKey("_Id");

            base.OnModelCreating(builder);
        }
    }
}
