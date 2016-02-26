using System;
using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public class ForEachBlockInvoker<T, TResult> : IInvoker {
        private LambdaExpression innerExpression;
        public ForEachBlockInvoker(LambdaExpression expression)  {
            innerExpression = expression;
        }

        public LambdaExpression Invoke() {
            var converted = (Expression<Func<T, TResult>>)innerExpression;
            var result = converted.Enumerate();
            return result;
        }
    }
}
