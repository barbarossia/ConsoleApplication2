using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public interface IMapRule<T, TResult> : IRule<T, IEnumerable<TResult>> {
    }

    public interface IReduceRule<T, TResult>: IRule<IEnumerable<T>, Func<TResult, TResult>> {
        TResult ExecuteCore(IEnumerable<T> list, TResult t1);
    }

    public interface IRule<T> : IRule<T, T>{
    }
    public interface IRule {
        string RuleKind { get; }
    }
    public interface IRule<T, R> : IRule {
        R Execute(T entity); 
    }

    public interface ICondition {
        bool Is();
    }

    public abstract class RuleBase<T> : IRule<T> {
        public string RuleKind {
            get {
                return RuleKinds.Rule;
            }
        }
        public abstract T Execute(T entity);
    }
   

    public class IninValueOnT1 : RuleBase<Test1> {
        public override Test1 Execute(Test1 entity) {
            entity.Value = 1;
            return entity;
        }
    }

    public class ConditionRuleOnT1 : ICondition {
        public bool Is() {
            return true;
        }
    }

    public class IninValueOnT2 : RuleBase<Test2> {
        public override Test2 Execute(Test2 entity) {
            entity.Value = 2;
            return entity;
        }
    }

    public class IninValueOnT3 : RuleBase<Test3> {
        public override Test3 Execute(Test3 entity) {
            entity.Value = 3;
            return entity;
        }
    }

    public class MapRuleOnT1IfTrue : IMapRule<Test1, Test2> {
        public IEnumerable<Test2> Execute(Test1 t1) {
            return Enumerable.Range(1, t1.A)
                .Select(t => new Test2() { B = t })
                .ToList();
        }
        public string RuleKind {
            get {
                return RuleKinds.MapRule;
            }
        }
    }

    public class MapRuleOnT1IfFalse : IMapRule<Test1, Test2> {
        public IEnumerable<Test2> Execute(Test1 t1) {
            return Enumerable.Range(1, t1.A / 2)
                .Select(t => new Test2() { B = t })
                .ToList();
        }
        public string RuleKind
        {
            get
            {
                return RuleKinds.MapRule;
            }
        }
    }

    public class MapRuleOnT2 : IMapRule<Test2, Test3> {
        public IEnumerable<Test3> Execute(Test2 t2) {
            return Enumerable.Range(1, t2.B)
                .Select(t => new Test3() { C = t })
                .ToList();
        }
        public string RuleKind {
            get {
                return RuleKinds.MapRule;
            }
        }
    }
    public abstract class ReduceRuleBase<T, TResult>: IReduceRule<T, TResult> {
        public Func<TResult, TResult> Execute(IEnumerable<T> entity) {
            return t => ExecuteCore(entity, t);
        }
        public string RuleKind {
            get {
                return RuleKinds.ReduceRule;
            }
        }
        public abstract TResult ExecuteCore(IEnumerable<T> list, TResult t1);
    }

    public class ReduceRuleOnT1 : ReduceRuleBase<Test2, Test1> {
        public override Test1 ExecuteCore(IEnumerable<Test2> list, Test1 t1) {
            t1.Result = list.Sum(t => t.Result);
            return t1;
        }
    }

    public class AssignRuleOnT1 : ReduceRuleBase<Test2, Test1> {
        public override Test1 ExecuteCore(IEnumerable<Test2> list, Test1 t1) {
            t1.Details = list;
            return t1;
        }
    }

    public class ReduceRuleOnT2 : ReduceRuleBase<Test3, Test2> {
        public override Test2 ExecuteCore(IEnumerable<Test3> list, Test2 t2) {
            t2.Result = list.Sum(t => t.C);
            return t2;
        }
    }

    public class AssignRuleOnT2 : ReduceRuleBase<Test3, Test2> {
        public override Test2 ExecuteCore(IEnumerable<Test3> list, Test2 t2) {
            t2.Details = list;
            return t2;
        }
    }
}
