using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    public class BudgetDetermination : IPersistable
    {
        public int Id { get; set; }
        public BudgetDeterminationStatusEnum Status { get; set; }
    }

    public enum BudgetDeterminationStatusEnum
    {
        Initial,
        InProgress,
        Complete,
        Archived
    }
}
