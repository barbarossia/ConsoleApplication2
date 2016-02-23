using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class RuleInvoker<T> : IInvoker {
        public LambdaExpression Invoke(Type ruleType) {
            var rule = (IRule<T>)ruleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, T>> n = (t) => rule.Execute(t);
            var i = Expression.Invoke(n, para1);
            return Expression.Lambda<Func<T, T>>(i, para1);
        }
    }

    public class RuleGroupInvoker<T> : GroupInvokerBase {
        public RuleGroupInvoker(params LambdaExpression[] exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var converted = Exprs.Select(e => (Expression<Func<T, T>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }

    }

    public class ForEachInvoker<T, TResult> : GroupInvokerBase {
        public ForEachInvoker(params LambdaExpression[] exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var converted = (Expression<Func<T, TResult>>)Exprs.First();
            var result = converted.Enumerate();
            return result;
        }
    }
    public class ForEachGroupInvoker<T, TMid, TResult> : GroupInvokerBase {
        public ForEachGroupInvoker(params LambdaExpression[] exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var map = (Expression<Func<T, IEnumerable<TMid>>>)Exprs.First();
            var forEach = (Expression<Func<IEnumerable<TMid>, IEnumerable<TResult>>>)Exprs.Last();
            var result = map.Concat(forEach);
            return result;
        }
    }
}
