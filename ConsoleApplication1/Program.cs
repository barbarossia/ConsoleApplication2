using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
    class Program {
        static void Main(string[] args) {
            Test4();

        }

        static void Test1() {
            IRule<Test1> rule = new IninValueOnT1();
            Expression1 expr = new Expression1();
            var func = expr.Create<Test1>(rule.GetType());
            //var func = (Expression<Func<Test1, Test1>>)expr.Create(rule.GetType());
            var func1 = func.Compile();

            Test1 t1 = new Test1() { A = 10 };
            var result = func1(t1);
            //var result = func1.DynamicInvoke(t1);
        }

        static void Test2() {
            IReduceRule<Test2, Test1> rule1 = new ReduceRuleOnT1();
            IReduceRule<Test2, Test1> rule2 = new AssignRuleOnT1();
            Expression<Func<IEnumerable<Test2>, Test1, Test1>> expr1 = (list, t) => rule1.Execute(list, t);
            Expression<Func<IEnumerable<Test2>, Test1, Test1>> expr2 = (list, t) => rule2.Execute(list, t);
            var result = expr1.Create(expr2);
            //var r = result.Compile();
            List<Test2> listT2 = new List<Test2>() { new Test2() { B = 2, Result = 1 } };
            Test1 t1 = new Test1() { A = 10 };
            var func = result.Compile();
            var r = func(listT2, t1);
        }

        static void Test3() {
            IMapRule<Test2, Test3> rule1 = new MapRuleOnT2();
            IMapRule<Test2, Test3> rule2 = new MapRuleOnT2Extra();
            Expression<Func<Test2, IEnumerable<Test3>>> expr1 = (t) => rule1.Execute(t);
            Expression<Func<Test2, IEnumerable<Test3>>> expr2 = (t) => rule2.Execute(t);

            Test2 t2 = new Test2() { B = 2, Result = 1 };
            var result = expr1.AddRange(expr2);
            var func = result.Compile();
            var r = func(t2);
            
        }

        static void Test4() {
            IRule<Test2> rule = new IninValueOnT2();
            Expression<Func<Test2, Test2>> selector = (t) => rule.Execute(t);
            Func<Test2, Test2> selector1 = (t) => rule.Execute(t);
            List<Test2> listT2 = new List<Test2>() { new Test2() { B = 2, Result = 1 }, new Test2() { B = 3, Result = 1 } };
            //Expression<Func<IEnumerable<Test2>, IEnumerable<Test2>>> expr = (list) => list.Select(l=> selector1(l));
            var expr = selector.Enumerate();
            var func = expr.Compile();
            var result = func(listT2);
        }

        class MapRuleOnT2Extra : IMapRule<Test2, Test3> {
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
}
