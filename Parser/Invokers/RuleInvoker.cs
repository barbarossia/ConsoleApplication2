using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.Invokers {
    public class RuleInvoker<T> : RuleInvokerBase {
        public RuleInvoker(Type ruleType) : base(ruleType) {
        }

        public override LambdaExpression Invoke() {
            var rule = (IRule<T>)RuleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, T>> n = (t) => rule.Execute(t);
            return Expression.Lambda<Func<T, T>>(Expression.Invoke(n, para1), para1);
        }
    }
}
