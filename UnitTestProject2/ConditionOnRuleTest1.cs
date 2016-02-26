using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class ConditionOnRuleTest1ReturnTrue : IQulification {
        public bool IsQualified() {
            return true;
        }
    }

    public class ConditionOnRuleTest1ReturnFalse : IQulification {
        public bool IsQualified() {
            return false;
        }
    }
}
