using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionsPrototype
{
    public interface IPersistable
    {
        int Id { get; set; }
    }

    /// <summary>
    /// A permission is a collection of rules determining who is allowed to perform certain actions.
    /// </summary>
    public abstract class Permission : IPersistable
    {
        // We can define extremely broad permissions, like SYS and ADM can do ANYTHING.
        protected virtual IEnumerable<Rule> GetRules()
        {
            yield return new HasAnyRoleRule("ADM", "SYS");
        }
    
        public int Id { get; set; }

        public IEnumerable<User> UsersWithPermission<T>(T obj, UnitOfWork uow) where T : class, IPersistable, new()
        {
            return GetRules().SelectMany(x => x.FindUsersSatisfying(obj, uow)).Distinct();
        }

        public bool HasPermission<T>(T obj, User u, UnitOfWork uow) where T : class, IPersistable, new()
        {
            return UsersWithPermission<T>(obj, uow).Contains(u);
        }
    }

    /// <summary>
    /// We can define them in a tree, so rules (ADM can see everything, for example) are shared among the various actions Create/View/Etc.
    /// </summary>
    public abstract class BudgetDeterminationPermission : Permission
    {
        protected override IEnumerable<Rule> GetRules()
        {
            yield return new StatusRule(new HasAnyRoleRule("BOAD"), BudgetDeterminationStatusEnum.Initial, BudgetDeterminationStatusEnum.InProgress, BudgetDeterminationStatusEnum.Complete);
            foreach (var rule in base.GetRules())
                yield return rule;
        }

        /// <summary>
        /// You can define custom rules specific to a single type
        /// Or you can define general rules <seealso cref="HasAnyRoleRule"/>
        /// By default all rules are "OR"d together, so if your'e granted permissions by any rule, you're granted permissions by all of them
        /// But you can also define rules which take in other rules (ex. the AndRule, which returns only the users that have all permissions, or
        /// the status rule, which only evaluates another rule if the status of an object is a certain value.
        /// </summary>
        public class StatusRule : Rule
        {
            private Rule _otherRule;
            private HashSet<BudgetDeterminationStatusEnum> _statuses;

            public StatusRule(Rule otherRule, params BudgetDeterminationStatusEnum[] statuses)
            {
                _otherRule = otherRule;
                _statuses = new HashSet<BudgetDeterminationStatusEnum>(statuses);
            }

            public override IEnumerable<User> FindUsersSatisfying<T>(T obj, UnitOfWork uow)
            {
                var bd = obj as BudgetDetermination;
                
                if (bd == null) return Enumerable.Empty<User>();
                if (!_statuses.Contains(bd.Status)) return Enumerable.Empty<User>();

                return _otherRule.FindUsersSatisfying(obj, uow);
            }
        }

        public class Create : BudgetDeterminationPermission
        {
            // For some, we don't even need to define rules, because they're caught by the parents.  We might expand this in the future though,
            // so we still have the permission
        }

        public class View : BudgetDeterminationPermission
        {
        }
    }

    public abstract class SpaceAllocationAcknowledgementPermission : Permission
    {
        public class StatusRule : Rule
        {
            private Rule _otherRule;
            private HashSet<SpaceAllocationAcknowledgementStatusEnum> _statuses;

            public StatusRule(Rule otherRule, params SpaceAllocationAcknowledgementStatusEnum[] statuses)
            {
                _otherRule = otherRule;
                _statuses = new HashSet<SpaceAllocationAcknowledgementStatusEnum>(statuses);
            }

            public override IEnumerable<User> FindUsersSatisfying<T>(T obj, UnitOfWork uow)
            {
                var saa = obj as SpaceAllocationAcknowledgement;

                if (saa == null) return Enumerable.Empty<User>();
                if (!_statuses.Contains(saa.Status)) return Enumerable.Empty<User>();

                return _otherRule.FindUsersSatisfying(obj, uow);
            }
        }

        public class View : SpaceAllocationAcknowledgementPermission
        {
            /// <summary>
            /// By the time you get down to defining which rules make up a given permission, it's *very* simple, and *very* declarative.
            /// Very easy to read, and very easy to verify.  This is the important part.
            /// </summary>
            /// <returns></returns>
            protected override IEnumerable<Rule> GetRules()
            {
                // The only thing special about viewing SAA's is 
                // If the SAA is Initial or acknowledged, FO's for the SAA's department can see it.
                yield return new StatusRule(new HasAnyRoleAtRule<SpaceAllocationAcknowledgement>(x => x.Department, "FO"), SpaceAllocationAcknowledgementStatusEnum.Initial, SpaceAllocationAcknowledgementStatusEnum.Acknowledged);
                // It doesn't matter which order we yield these in, because everything is being OR'd 
                foreach (var rule in base.GetRules())
                    yield return rule;
            }
        }
    }
}
