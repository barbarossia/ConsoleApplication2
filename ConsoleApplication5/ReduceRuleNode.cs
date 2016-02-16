using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ReduceRuleNode<T, R> : IReduceNode<IEnumerable<T>, R> {
        public string Name
        {
            get
            {
                return "ReduceRule";
            }
        }
        public Type RuleType { get; set; }
        public ReduceRuleNode(Type name) {
            RuleType = name;
        }

        public Func<IEnumerable<T>, R, R> Compile() {
            //var ruleType = engine.RuleTypes[RuleType];
            var rule = (IReduceRule<T, R>)RuleType.CreateInstance();
            return (i, o) => rule.Execute(i, o);
        }
    }
}
