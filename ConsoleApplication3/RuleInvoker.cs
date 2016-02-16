using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {

    public interface IRuleInvoker<T, TResult> {
        Func<T, TResult> Invoke(IRule<T, TResult> rule);
    }

    public class RuleInvoker<T, TResult> : IRuleInvoker<T, TResult> {
        public Func<T, TResult> Invoke(IRule<T, TResult> rule) {
            return (t) => rule.Execute(t);
        }
    }
    //public class IfElseRuleInvoker<T, TResult> : IRuleInvoker<T, TResult> {
    //    public Func<T, TResult> Invoke(IRule<T, TResult> rule) {
    //        return Invoke((IComplexRule<T, TResult>)rule);
    //    }

    //    private Func<T, TResult> Invoke(IComplexRule<T, TResult> rule) {
    //        if (rule.Condition.Execute())
    //    }
    //    private Func<T, TResult> Concat(IEnumerable<IRule<T, TResult>> rules) {
    //        RuleInvoker<T, TResult> invoker = new RuleInvoker<T, TResult>();
    //        return rules.Select(r => invoker.Invoke(r))
    //            .Aggregate((curr, next) => curr.Concact(next));
    //    }
    //}
}
