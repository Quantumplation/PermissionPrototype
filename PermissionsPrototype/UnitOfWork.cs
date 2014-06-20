using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    public class UnitOfWork
    {
        public Dictionary<int, object> Data;
        
        public T Get<T>(int id)
        {
            return Get<T>(new[] {id}).Single();
        }
        public IEnumerable<T> Get<T>(IEnumerable<int> ids)
        {
            return ids.Select(x => Data[x]).OfType<T>();
        }
        public IEnumerable<T> GetWhere<T>(Func<T, bool> predicate)
        {
            return Data.Values.OfType<T>().Where(predicate);
        }
    }
}
