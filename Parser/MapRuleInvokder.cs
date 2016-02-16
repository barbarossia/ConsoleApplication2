using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parser {
    public class MapRuleInvoker<T, TResult> : IInvoker {
        public LambdaExpression Invoke(Type ruleType) {
            var rule = (IMapRule<T, TResult>)ruleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, IEnumerable<TResult>>> n = (t) => rule.Execute(t);
            var i = Expression.Invoke(n, para1);
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(i, para1);
        }
    }

    public class MapGroupInvoker<T, TResult> : IGroupInvoker {
        public LambdaExpression Invoke(IEnumerable<LambdaExpression> exprs) {
            var converted = exprs.Select(e => (Expression<Func<T, IEnumerable<TResult>>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }
    }
}
