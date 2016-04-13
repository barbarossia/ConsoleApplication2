using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public class RuleGroupInvoker<T> : GroupInvokerBase {
        public RuleGroupInvoker(params LambdaExpression[] exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var converted = Exprs.Select(e => (Expression<Func<T, T>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }
    }
}
