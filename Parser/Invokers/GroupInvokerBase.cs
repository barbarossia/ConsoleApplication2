using System;
using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.Invokers {
    public abstract class GroupInvokerBase : IInvoker {
        public GroupInvokerBase(params LambdaExpression[] exprs) {
            Exprs = exprs;
        }

        public IEnumerable<LambdaExpression> Exprs { get; private set; }
        public abstract LambdaExpression Invoke();
    }
}
