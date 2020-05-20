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
                         Id = "RP001",
                         AuthorName = "GataullinRR",
                         CaseInfo = new TestsStorageService.API.CSTestCaseInfo()
                         {
                             CaseSourceId = "7710",
                             DisplayName = "RussiaPost package status checker service",
                             Data = "LaLaLa".GetASCIIBytes(),
                             TargetType = "REST"
                         },
                    },
                    new TestCase()
                    {
                         Id = "B9123",
                         AuthorName = "LibovskyKM",
                         CaseInfo = new TestsStorageService.API.CSTestCaseInfo()
                         {
                             CaseSourceId = "S989-221",
                             DisplayName = "Bugfix #12 check",
                             Data = "HiHiHi".GetASCIIBytes(),
                             TargetType = "REST"
                         },
                    },
                    new TestCase()
                    {
                         Id = "B3133",
                         AuthorName = "AA",
                         CaseInfo = new TestsStorageService.API.CSTestCaseInfo()
                         {
                             CaseSourceId = "921",
                             DisplayName = "Delivery status does not updates on order cancel",
                             Data = "MeMeMe".GetASCIIBytes(),
                             TargetType = "UI"
                         },
                    }
                });

                db.SaveChanges();
            }
        }
    }
}
