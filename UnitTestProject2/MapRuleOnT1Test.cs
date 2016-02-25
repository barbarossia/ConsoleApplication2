using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapReduce.Parser.UnitTest {
    public class MapRuleOnT1Test : IMapRule<Test1, Test2> {
        public IEnumerable<Test2> Execute(Test1 t2) {
            return Enumerable.Range(1, 100)
                .Select(t => new Test2() { B = t })
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
