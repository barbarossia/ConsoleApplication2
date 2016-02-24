using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class ConditionOnRuleTest1 : IQulification {
        public bool IsQualified() {
            return false;
        }
    }
}
