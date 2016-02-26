using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace MapReduce.Parser.Invokers {
    public class MapGroupInvoker<T, TResult> : GroupInvokerBase {
        public MapGroupInvoker(params LambdaExpression[] exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var converted = Exprs.Select(e => (Expression<Func<T, IEnumerable<TResult>>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }
    }
}
