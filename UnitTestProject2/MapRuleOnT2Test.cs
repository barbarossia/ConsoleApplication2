using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class MapRuleOnT2Test : IMapRule<Test2, Test3> {
        public IEnumerable<Test3> Execute(Test2 t2) {
            return Enumerable.Range(1, 100)
                .Select(t => new Test3() { C = t })
                .ToList();
        }
        public RuleKind RuleKind
        {
            get
            {
                return RuleKind.MapRule;
            }
        }
    }
}
