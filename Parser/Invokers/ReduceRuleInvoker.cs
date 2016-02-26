using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.Invokers {
    public class ReduceRuleInvoker<T, TResult> : RuleInvokerBase {
        public ReduceRuleInvoker(Type ruleType) : base(ruleType) {
        }

        public override LambdaExpression Invoke() {
            var rule = (IReduceRule<T, TResult>)RuleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(IEnumerable<T>));
            var para2 = Expression.Parameter(typeof(TResult));
            Expression<Func<IEnumerable<T>, TResult, TResult>> n = (list, t) => rule.Execute(list, t);
            return Expression.Lambda<Func<IEnumerable<T>, TResult, TResult>>(Expression.Invoke(n, para1, para2), para1, para2);
        }
    }
}
