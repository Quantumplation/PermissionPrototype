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

    public abstract class Permission : IPersistable
    {
        protected abstract IEnumerable<Rule> GetRules();
    
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

    public abstract class BudgetDeterminationPermission : Permission
    {
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
            protected override IEnumerable<Rule> GetRules()
            {
                yield return new HasAnyRoleRule("ADM", "BOAD");
            }
        }

        public class View : BudgetDeterminationPermission
        {
            protected override IEnumerable<Rule> GetRules()
            {
                yield return new StatusRule(new HasAnyRoleRule("ADM", "BOAD"), BudgetDeterminationStatusEnum.Initial, BudgetDeterminationStatusEnum.InProgress, BudgetDeterminationStatusEnum.Complete);
                yield return new StatusRule(new HasAnyRoleRule("ADM"), BudgetDeterminationStatusEnum.Archived);
            }
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
            protected override IEnumerable<Rule> GetRules()
            {
                yield return new HasAnyRoleRule("ADM", "BOAD");
                yield return new StatusRule(new HasAnyRoleAtRule<SpaceAllocationAcknowledgement>(x => x.Department, "FO"), SpaceAllocationAcknowledgementStatusEnum.Initial, SpaceAllocationAcknowledgementStatusEnum.Acknowledged);
            }
        }
    }
}
