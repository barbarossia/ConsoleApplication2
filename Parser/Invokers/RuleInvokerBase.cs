using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MapReduce.Parser.Invokers {
    public abstract class RuleInvokerBase : IInvoker {
        public RuleInvokerBase(Type ruleType) {
            RuleType = ruleType;
        }

        public Type RuleType { get; private set; }
        public abstract LambdaExpression Invoke();
    }
}
