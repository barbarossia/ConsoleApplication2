using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class MapRuleOnT2WithCons : IMapRule<Test2, Test3> {
        private int count;
        public MapRuleOnT2WithCons(int i) {
            count = i;
        }
        public IEnumerable<Test3> Execute(Test2 t2) {
            return Enumerable.Range(1, 3)
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
