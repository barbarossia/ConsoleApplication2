using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ConditionToken : Token {
        public ICondition Condition { get; private set; }
        public override INode Accept<T, R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }
        public ConditionToken(string ruleType, Context ctx) {
            string theType = (string)ctx.Items[ruleType];
            var the = Type.GetType(theType);
            Condition = (ICondition)the.CreateInstance();

        }

    }
}
