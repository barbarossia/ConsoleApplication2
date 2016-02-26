using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.Invokers {
    public interface IInvoker {
        LambdaExpression Invoke();
    }
}
