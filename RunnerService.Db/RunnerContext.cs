﻿using Microsoft.EntityFrameworkCore;
using RunnerService.APIModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunnerService.Db
{
    public class RunnerContext : DbContext
    {
        public DbSet<TestRunInfo> TestRuns { get; set; }

        public RunnerContext(DbContextOptions<RunnerContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TestRunInfo>()
                .HasKey(r => r.TestId);

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
                .HasValue<AwaitingStartState>(State.AwaitingStart)
                .HasValue<ReadyState>(State.Ready)
                .HasValue<RunningState>(State.Running)
                .HasValue<JustCreatedState>(State.JustCreated);

            builder.Entity<RunResultBase>()
                .Property<int>("_Id")
                .UseIdentityColumn();
            builder.Entity<RunResultBase>()
                .HasKey("_Id");
            //builder.Entity<RunResultBase>()
            //    .Property(r => r.Result)
            //    .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.Entity<RunResultBase>()
                .HasDiscriminator(r => r.Result)
                .HasValue<PassedResult>(RunResult.Passed)
                .HasValue<AbortedByUserResult>(RunResult.AbortedByUser)
                .HasValue<RunnerErrorResult>(RunResult.RunnerError)
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
