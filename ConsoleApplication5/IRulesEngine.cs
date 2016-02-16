using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public interface IRulesEngine {
        RuleTypeDictionary RuleTypes { get; set; }
        RuleConditionDictionary ConditionTypes { get; set; }
    }

    public class RulesEngine : IRulesEngine {
        public string Name { get; set; }
        public RuleTypeDictionary RuleTypes { get; set; }
        public RuleConditionDictionary ConditionTypes { get; set; }
        public Token Root { get; set; }

    }
}
