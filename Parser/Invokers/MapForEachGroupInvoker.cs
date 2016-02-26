using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public class MapForEachGroupInvoker<T, TMid, TResult> : DualInvokerBase {
        public MapForEachGroupInvoker(LambdaExpression left, LambdaExpression right) : base(left, right) {
        }

        public override LambdaExpression Invoke() {
            var map = (Expression<Func<T, IEnumerable<TMid>>>)Left;
            var forEach = (Expression<Func<IEnumerable<TMid>, IEnumerable<TResult>>>)Right;
            var result = map.Concat(forEach);
            return result;
        }
    }
}
