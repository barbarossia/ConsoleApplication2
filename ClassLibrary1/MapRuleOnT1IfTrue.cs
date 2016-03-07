using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public class MapRuleOnT1IfTrue : IMapRule<Test1, Test2> {
        public IEnumerable<Test2> Execute(Test1 t1) {
            Console.WriteLine("Call me, MapRuleOnT1IfTrue");
            return Enumerable.Range(1, t1.A)
                .Select(t => new Test2() { B = t });
        }
        public RuleKind RuleKind
        {
            get
            {
                return RuleKind.MapRule;
            }
        }
    }

    public class MapRuleOnT1IfFalse : IMapRule<Test1, Test2> {
        public IEnumerable<Test2> Execute(Test1 t1) {
            Console.WriteLine("Call me, MapRuleOnT1IfFalse");
            return Enumerable.Range(1, t1.A / 2)
                .Select(t => new Test2() { B = t });
        }
        public RuleKind RuleKind
        {
            get
            {
                return RuleKind.MapRule;
            }
        }
    }

    public class MapRuleOnT2 : IMapRule<Test2, Test3> {
        public IEnumerable<Test3> Execute(Test2 t2) {
            Console.WriteLine("Call me, MapRuleOnT2");
            return Enumerable.Range(1, t2.B)
                .Select(t => new Test3() { C = t });
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
