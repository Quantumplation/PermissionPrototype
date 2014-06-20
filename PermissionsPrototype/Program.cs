using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            var uow = new UnitOfWork
            {
                Data = new Dictionary<int, object>
                {
                    {1, new User {Id = 1, Name = "Arnold", Roles = new List<Tuple<string, string>> {Tuple.Create("ADM", "GLOBAL"), Tuple.Create("BOAD", "GLOBAL"), Tuple.Create("ICRA", "GLOBAL")}}},
                    {2, new User {Id = 2, Name = "Beth", Roles = new List<Tuple<string, string>> {Tuple.Create("BOAD", "GLOBAL"), Tuple.Create("ICRA", "GLOBAL")}}},
                    {9, new User {Id = 9, Name = "Cathy", Roles = new List<Tuple<string, string>> {Tuple.Create("ICRA", "GLOBAL")}}},
                    {3, new User {Id = 3, Name = "Donald", Roles = new List<Tuple<string, string>> {Tuple.Create("DA", "GLOBAL.SCHOOLS.STEIN"), Tuple.Create("FO", "GLOBAL.SCHOOLS.STEIN")}}},
                    {4, new User {Id = 4, Name = "Erwin", Roles = new List<Tuple<string, string>> {Tuple.Create("VW", "GLOBAL.OTSS"), Tuple.Create("FO", "GLOBAL.SCHOOLS.CIMS")}}},
                    {13, new User {Id = 13, Name = "Fernando", Roles = new List<Tuple<string, string>> {Tuple.Create("VW", "GLOBAL.OTSS")}}},
                    {14, new User {Id = 14, Name = "Gertrude", Roles = new List<Tuple<string, string>> {Tuple.Create("FO", "GLOBAL.SCHOOLS")}}},
                    {17, new User {Id = 17, Name = "Harold", Roles = new List<Tuple<string, string>> {Tuple.Create("FO", "GLOBAL.SCHOOLS.PLY.HELP")}}},
                    {5, new BudgetDetermination {Id = 5, Status = BudgetDeterminationStatusEnum.Initial}},
                    {6, new BudgetDetermination {Id = 6, Status = BudgetDeterminationStatusEnum.InProgress}},
                    {7, new BudgetDetermination {Id = 7, Status = BudgetDeterminationStatusEnum.Complete}},
                    {8, new BudgetDetermination {Id = 8, Status = BudgetDeterminationStatusEnum.Archived}},
                    {10, new SpaceAllocationAcknowledgement { Id = 10, Status = SpaceAllocationAcknowledgementStatusEnum.Initial, Department = "GLOBAL.SCHOOLS.STEIN"}},
                    {11, new SpaceAllocationAcknowledgement { Id = 11, Status = SpaceAllocationAcknowledgementStatusEnum.Acknowledged, Department = "GLOBAL.SCHOOLS.CIMS"}},
                    {12, new SpaceAllocationAcknowledgement { Id = 12, Status = SpaceAllocationAcknowledgementStatusEnum.Initial, Department = "GLOBAL.SCHOOLS.PLY"}},
                    {15, new SpaceAllocationAcknowledgement { Id = 15, Status = SpaceAllocationAcknowledgementStatusEnum.Initial, Department = "GLOBAL.OTSS"}},
                    {16, new SpaceAllocationAcknowledgement { Id = 16, Status = SpaceAllocationAcknowledgementStatusEnum.Initial, Department = "GLOBAL.SCHOOLS.PLY.HELP"}},
                }
            };

            var bdPerm = new BudgetDeterminationPermission.View();
            var saaPerm = new SpaceAllocationAcknowledgementPermission.View();
            for (int x = 5; x <= 8; x++)
            {
                var obj = uow.Get<BudgetDetermination>(x);
                Console.WriteLine("Users that can see {0}: {1}", x, String.Join(", ", bdPerm.UsersWithPermission(obj, uow)));
            }
            for (int x = 10; x <= 16; x++)
            {
                var obj = uow.Get<SpaceAllocationAcknowledgement>(x);
                Console.WriteLine("Users that can see {0}: {1}", x, String.Join(", ", saaPerm.UsersWithPermission(obj, uow)));
                if (x == 12) x = 15;
            }
            Console.ReadKey();
        }
    }
}
