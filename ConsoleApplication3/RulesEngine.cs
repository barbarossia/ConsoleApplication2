using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public interface IRulesEngine {
        RuleTypeDictionary RuleTypes { get; set; }
        RuleConditionDictionary ConditionTypes { get; set; }
    }
    public class RulesEngine<T, R> : IRulesEngine {
        public string Name { get; set; }
        public RuleTypeDictionary RuleTypes { get; set; }
        public RuleConditionDictionary ConditionTypes { get; set; }
        public Token Root { get; set; }
        public Func<T, R> Build() {
            var compiler = (Compiler)Utilities.CreateInstance(typeof(NodeComiler));
            var node = compiler.Compile(Root);
            return null;
        }

    }
}
