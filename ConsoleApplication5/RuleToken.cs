using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class RuleToken : Token {
        public Type RuleType { get; private set; }
        public RuleToken(string ruleName, Context ctx) {
            string ruleType = (string)ctx.Items[ruleName];
            RuleType = Type.GetType(ruleType);
            var method = RuleType.GetMethod("Execute");
            SourceType = GetType(method.GetParameters()[0].ParameterType);
            TargetType = GetType(method.ReturnType);

        }
        public RuleToken(string ruleType, string sourceType, string targetType) {
            RuleType = Type.GetType(ruleType);
            SourceType = Type.GetType(sourceType);
            TargetType = !string.IsNullOrEmpty(targetType) ? Type.GetType(targetType) : SourceType;

        }

        public override INode Accept<T, R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }

        private Type GetType(Type theType) {
            Type result = theType;
            if("IEnumerable`1" == theType.Name) {
                result = theType.GenericTypeArguments[0];
            }
            return result;
        }

    }
}
