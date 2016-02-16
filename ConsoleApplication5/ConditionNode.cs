using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ConditionNode : IConditionNode {
        public string Name
        {
            get
            {
                return "ConditionRule";
            }
        }

        public Func<bool> Compile() {
            var rule = (ICondition)RuleType.CreateInstance();
            return () => rule.Is();
        }

        public Type RuleType { get; set; }
        public ConditionNode(Type name) {
            RuleType = name;
        }
    }
}
