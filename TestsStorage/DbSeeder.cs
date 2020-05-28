﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TestsStorageService.API;
using TestsStorageService.Db;
using Utilities.Extensions;
using Utilities.Types;

namespace UserService
{
    [Service(ServiceLifetime.Scoped, RegisterAsPolicy.Self)]
    class DbSeeder
    {
        public DbSeeder(TestsContext db)
        {
            if (db.Cases.Count() == 0)
            {
                db.Cases.AddRange(new TestCase[]
                {
                    new TestCase()
                    {
                         TestName = "RP001.C1",
                         AuthorName = "GataullinRR",
                         CreationDate = DateTime.UtcNow.AddDays(-100),
                         DisplayName = "RussiaPost package status checker service",
                         State = TestCaseState.NotRecorded,
                    },
                    new TestCase()
                    {
                        TestName = "B9123",
                        AuthorName = "LibovskyKM",
                        CreationDate = DateTime.UtcNow.AddDays(0),
                        DisplayName = "Bugfix #12 check",
                        State = TestCaseState.Saved,
                        Data = new TestCaseData()
                        {
                            Type = "REST",
                            Data = "HiHiHi".GetASCIIBytes(),
                            Parameters = "<ps name=\"Parameters\"><p name=\"User name\">Radmir</p><p name=\"Amount\">1000 R</p></ps>",
                        }
                    },
                    new TestCase()
                    {
                        TestName = "RP001.C2",
                        AuthorName = "AA",
                        CreationDate = DateTime.UtcNow.AddDays(0),
                        DisplayName = "Delivery status does not updates on order cancel",
                        State = TestCaseState.Saved,
                        Data = new TestCaseData()
                        {
                            Type = "UI",
                            Data = "MeMeMe".GetASCIIBytes(),
                            Parameters = "<p name=\"Passport\">0000 123456</p>",
                        }
                    }
                });

                db.SaveChanges();
            }
        }
    }
}
