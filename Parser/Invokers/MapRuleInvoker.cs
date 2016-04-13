using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public class MapRuleInvoker<T, TResult> : RuleInvokerBase {
        public MapRuleInvoker(Type ruleType) : base(ruleType) {
        }

        public override LambdaExpression Invoke() {
            var rule = (IMapRule<T, TResult>)RuleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, IEnumerable<TResult>>> n = (t) => rule.Execute(t);
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(Expression.Invoke(n, para1), para1);
        }
    }
}
