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
                    {1, new User {Id = 1, Roles = new List<string> {"ADM", "BOAD", "ICRA"}}},
                    {2, new User {Id = 2, Roles = new List<string> {"BOAD", "ICRA"}}},
                    {3, new User {Id = 3, Roles = new List<string> {"DA"}}},
                    {4, new User {Id = 4, Roles = new List<string> {"VW"}}},
                    {5, new BudgetDetermination {Id = 5, Status = BudgetDeterminationStatusEnum.Initial}},
                    {6, new BudgetDetermination {Id = 6, Status = BudgetDeterminationStatusEnum.InProgress}},
                    {7, new BudgetDetermination {Id = 7, Status = BudgetDeterminationStatusEnum.Complete}},
                    {8, new BudgetDetermination {Id = 8, Status = BudgetDeterminationStatusEnum.Archived}},
                }
            };
        }
    }
}
