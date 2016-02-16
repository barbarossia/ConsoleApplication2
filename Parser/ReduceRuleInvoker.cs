using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parser {
    public interface IInvoker {
        LambdaExpression Invoke(Type ruleType);
    }
    public interface IGroupInvoker {
        LambdaExpression Invoke(IEnumerable<LambdaExpression> exprs);
    }
    public class ReduceRuleInvoker<T, TResult> : IInvoker {
        public LambdaExpression Invoke(Type ruleType) {
            var rule = (IReduceRule<T, TResult>)ruleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(IEnumerable<T>));
            var para2 = Expression.Parameter(typeof(TResult));
            Expression<Func<IEnumerable<T>, TResult, TResult>> n = (list, t) => rule.Execute(list, t);
            var i = Expression.Invoke(n, para1, para2);
            return Expression.Lambda<Func<IEnumerable<T>, TResult, TResult>>(i, para1, para2);
        }
    }

    public class ReduceInvoker<T, TResult> : IGroupInvoker {
        public LambdaExpression Invoke(IEnumerable<LambdaExpression> exprs) {
            var converted = exprs.Select(e => (Expression<Func<IEnumerable<T>, TResult, TResult>>)e);
            var result = converted.Aggregate((curr, next)=> curr.Concat(next));
            return result;
        }
    }
}
