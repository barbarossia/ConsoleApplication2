using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class TestQulificationUseContext : IQulification {
        public bool IsQualified() {
            return Context.InnerContext.RandomNum % 2 == 0;
        }
    }

    public class TestQulificationUseContextElse : IQulification {
        public bool IsQualified() {
            return Context.InnerContext.RandomNum % 2 != 0;
        }
    }
}
