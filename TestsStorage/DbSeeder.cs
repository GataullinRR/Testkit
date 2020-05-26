using System;
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
                         TestId = "RP001.C1",
                         AuthorName = "GataullinRR",
                         CreationDate = DateTime.UtcNow.AddDays(-100),
                         DisplayName = "RussiaPost package status checker service",
                         State = TestCaseState.NotRecorded,
                    },
                    new TestCase()
                    {
                        TestId = "B9123",
                        AuthorName = "LibovskyKM",
                        CreationDate = DateTime.UtcNow.AddDays(0),
                        DisplayName = "Bugfix #12 check",
                        State = TestCaseState.Recorder,
                        Data = new TestCaseData()
                        {
                            Type = "REST",
                            Data = "HiHiHi".GetASCIIBytes(),
                        }
                    },
                    new TestCase()
                    {
                        TestId = "RP001.C2",
                        AuthorName = "AA",
                        CreationDate = DateTime.UtcNow.AddDays(0),
                        DisplayName = "Delivery status does not updates on order cancel",
                        State = TestCaseState.Recorder,
                        Data = new TestCaseData()
                        {
                            Type = "UI",
                            Data = "MeMeMe".GetASCIIBytes(),
                        }
                    }
                });

                db.SaveChanges();
            }
        }
    }
}
