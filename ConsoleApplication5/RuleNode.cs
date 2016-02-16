using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class RuleNode<T> : INode<T> {
        public Func<T, T> Compile() {
            //var ruleType = engine.RuleTypes[RuleType];
            var rule = (IRule<T>)RuleType.CreateInstance();
            return (t) => rule.Execute(t);
        }
        public string Name { get { return "Rule"; } }
        public Type RuleType { get; set; }
        public RuleNode(Type name) {
            RuleType = name;
        }
    }
}
