using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parser {
    public class RuleInvoker<T> : IInvoker {
        public LambdaExpression Invoke(Type ruleType) {
            var rule = (IRule<T>)ruleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, T>> n = (t) => rule.Execute(t);
            var i = Expression.Invoke(n, para1);
            return Expression.Lambda<Func<T, T>>(i, para1);
        }
    }

    public class RuleGroupInvoker<T> : IGroupInvoker {
        public LambdaExpression Invoke(IEnumerable<LambdaExpression> exprs) {
            var converted = exprs.Select(e => (Expression<Func<T, T>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }
    }
}
