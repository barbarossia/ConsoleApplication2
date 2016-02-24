using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public abstract class ReduceRuleBase<T, TResult> : IReduceRule<T, TResult> {
        public RuleKind RuleKind
        {
            get
            {
                return RuleKind.ReduceRule;
            }
        }
        public abstract TResult Execute(IEnumerable<T> list, TResult t1);
    }

    public class ReduceRuleOnT1 : ReduceRuleBase<Test2, Test1> {
        public override Test1 Execute(IEnumerable<Test2> list, Test1 t1) {
            t1.Result = list.Sum(t => t.Result);
            return t1;
        }
    }

    public class AssignRuleOnT1 : ReduceRuleBase<Test2, Test1> {
        public override Test1 Execute(IEnumerable<Test2> list, Test1 t1) {
            t1.Details = list;
            return t1;
        }
    }

    public class ReduceRuleOnT2 : ReduceRuleBase<Test3, Test2> {
        public override Test2 Execute(IEnumerable<Test3> list, Test2 t2) {
            t2.Result = list.Sum(t => t.C);
            return t2;
        }
    }

    public class AssignRuleOnT2 : ReduceRuleBase<Test3, Test2> {
        public override Test2 Execute(IEnumerable<Test3> list, Test2 t2) {
            t2.Details = list;
            return t2;
        }
    }
}
