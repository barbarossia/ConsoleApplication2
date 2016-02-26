using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public class RuleAndMapInvoker<T, TResult> : DualInvokerBase {
        public RuleAndMapInvoker(LambdaExpression left, LambdaExpression right) : base(left, right) {
        }
        public override LambdaExpression Invoke() {
            var rule = (Expression<Func<T, T>>)Left;
            var map = (Expression<Func<T, IEnumerable<TResult>>>)Right;
            return rule.Concat(map);
        }
    }
}
