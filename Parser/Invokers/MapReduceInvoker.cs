using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public class MapReduceInvoker<T, TResult> : DualInvokerBase {
        public MapReduceInvoker(LambdaExpression left, LambdaExpression right) : base(left, right) {
        }

        public override LambdaExpression Invoke() {
            var map = (Expression<Func<T, IEnumerable<TResult>>>)Left;
            var reduce = (Expression<Func<IEnumerable<TResult>, T, T>>)Right;
            return map.Concat(reduce);
        }
    }
}
