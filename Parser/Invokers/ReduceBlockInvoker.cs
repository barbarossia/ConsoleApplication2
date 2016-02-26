using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.Invokers {
    public class ReduceBlockInvoker<T, TResult> : GroupInvokerBase {
        public ReduceBlockInvoker(params LambdaExpression[] exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var converted = Exprs.Select(e => (Expression<Func<IEnumerable<T>, TResult, TResult>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }
    }
}
