using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    public abstract class Rule
    {
        public abstract IEnumerable<User> FindUsersSatisfying<T>(T obj, UnitOfWork uow) where T : class, IPersistable, new();

        public virtual bool UserSatisfies<T>(User u, T obj, UnitOfWork uow) where T : class, IPersistable, new()
        {
            return FindUsersSatisfying(obj, uow).Any(x => x == u);
        }
    }

    public class AndRule : Rule
    {
        private readonly IEnumerable<Rule> _rules;

        public AndRule(params Rule[] rules) : this((IEnumerable<Rule>)rules)
        {
            
        }

        public AndRule(IEnumerable<Rule> rules)
        {
            _rules = rules;
        }

        public override IEnumerable<User> FindUsersSatisfying<T>(T obj, UnitOfWork uow)
        {
            var users = new HashSet<User>();
            foreach (var rule in _rules)
            {
                if(users.Count == 0) users.UnionWith(rule.FindUsersSatisfying(obj, uow));
                else
                {
                    users.IntersectWith(rule.FindUsersSatisfying(obj, uow));
                }
            }
            return users;
        }
    }

    public class HasAnyRoleRule : Rule
    {
        // This logic could be more complex in the real system, here we're just simulating it
        private readonly HashSet<string> _globalroles;

        public HasAnyRoleRule(params string[] roles) : this((IEnumerable<string>) roles)
        {
        }

        public HasAnyRoleRule(IEnumerable<string> groles)
        {
            _globalroles = new HashSet<string>(groles);
        }

        public HasAnyRoleRule(IEnumerable<string> globalRoles, IEnumerable<Tuple<string, string>> specificRoles)
        {
            _globalroles = new HashSet<string>(globalRoles);
        }

        public override IEnumerable<User> FindUsersSatisfying<T>(T obj, UnitOfWork uow)
        {
            // Anyone with any of the global roles at any position,
            // or anyone with a specific role 
            return uow.GetWhere<User>(x => _globalroles.Overlaps(x.Roles.Select(y => y.Item1)));
        }
    }

    /// <summary>
    /// This gives some example of how useful the rules can be, because we can define very common patterns that are
    /// relatively complex, but use them over and over in a bunch of permissions.
    /// </summary
    public class HasAnyRoleAtRule<U> : Rule where U : class, IPersistable, new()
    {
        private readonly HashSet<string> _roles;
        private Func<U, string> _ouSelector; 

        public HasAnyRoleAtRule(Func<U, string> ouSelector, params string[] roles) : this(ouSelector, (IEnumerable<string>) roles)
        {
        }

        public HasAnyRoleAtRule(Func<U, string> ouSelector, IEnumerable<string> roles)
        {
            _ouSelector = ouSelector;
            _roles = new HashSet<string>(roles);
        }

        public override IEnumerable<User> FindUsersSatisfying<T>(T obj, UnitOfWork uow)
        {
            return
                uow.GetWhere<User>(
                    x => x.Roles.Any(y => _roles.Contains(y.Item1) && _ouSelector(obj as U).StartsWith(y.Item2)));
        }
    }
}
