using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.Invokers {
    public abstract class DualInvokerBase : IInvoker {
        public LambdaExpression Left { get; private set; }
        public LambdaExpression Right { get; private set; }
        public abstract LambdaExpression Invoke();
        public DualInvokerBase(LambdaExpression left, LambdaExpression right) {
            Left = left;
            Right = right;
        }
    }
}
