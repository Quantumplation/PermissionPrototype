using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    public class SpaceAllocationAcknowledgement : IPersistable
    {
        public int Id { get; set; }
        public SpaceAllocationAcknowledgementStatusEnum Status { get; set; }
        public string Department { get; set; }
    }

    public enum SpaceAllocationAcknowledgementStatusEnum
    {
        Initial,
        Acknowledged,
        Archived,
    }
}
