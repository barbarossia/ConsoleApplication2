using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class MapRuleOnT2Add : IMapRule<Test2, Test3> {
        public virtual IEnumerable<Test3> Execute(Test2 t2) {
            Console.WriteLine("Call me, IninValueOnT2Add");
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

    public class MapRuleOnT2AddInherit : MapRuleOnT2Add {
        public override IEnumerable<Test3> Execute(Test2 t2) {
            Console.WriteLine("Call me, MapRuleOnT2AddInherit");
            return Enumerable.Range(4, 6)
                .Select(t => new Test3() { C = t })
                .ToList();
        }

    }
}
