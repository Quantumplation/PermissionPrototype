using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    public class User : IPersistable
    {
        public List<string> Roles { get; set; }

        public int Id { get; set; }
    }
}
