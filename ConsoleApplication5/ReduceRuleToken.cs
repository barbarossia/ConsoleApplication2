using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ReduceRuleToken : RuleToken {
        public ReduceRuleToken(string ruleType, Context ctx) : base(ruleType, ctx) {
        }

        public ReduceRuleToken(string ruleType, string sourceType, string targetType) : base(ruleType, sourceType, targetType) {
        }

        public override INode Accept<T, R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }
    }
}
