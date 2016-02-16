using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication3;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
            var reduce1 = new ReduceRuleOnT2();
            var reduce2 = new AssignRuleOnT2();

            var r = new Test2() {
                Result = 0,
            };

            var details = new List<Test3>() {
                    new Test3() { C = 1},
                    new Test3() { C = 2},
                };

            var func = ConcatRules(new List<IRule<IEnumerable<Test3>, Func<Test2, Test2>>>() { reduce1, reduce2});
            var result = func(details, r);
            Assert.AreEqual(3, r.Result);
        }
        [TestMethod]
        public void Concatant() {
            var rule1 = new IninValueOnT1();
            var rule2 = new MapRuleOnT1IfTrue();
            var rule3 = new ReduceRuleOnT1();
            var node = new MapRuleNode<Test1, Test2>("MapRule");
            var node2 = new ReduceRuleNode<IEnumerable<Test2>, Func<Test1, Test1>>("ReduceRule");
            IRulesEngine engine = new RulesEngine<Test1, Test2>();
            RuleTypeDictionary d = new RuleTypeDictionary();
            d.Add(RuleKinds.MapRule, rule2.GetType());
            d.Add(RuleKinds.ReduceRule, rule3.GetType());
            engine.RuleTypes = d;
            Func<Test1, Test1> func1 = (r) => rule1.Execute(r);
            var concat1 = (IConcatenateResult<Test1, IEnumerable<Test2>>)Utilities.CreateType(typeof(ConcatenateResult<,,>), typeof(Test1), typeof(Test1), typeof(IEnumerable<Test2>))
                                                              .CreateInstance(func1, node);
            Func<Test1, IEnumerable<Test2>> r1 = concat1.GetResult(engine);
            var concat2 = (IConcatenateResult<Test1, Func<Test1, Test1>>)Utilities.CreateType(typeof(ConcatenateResult<,,>), typeof(Test1), typeof(IEnumerable<Test2>), typeof(Func<Test1, Test1>))
                                                              .CreateInstance(r1, node2);
            var r2 = concat2.GetResult(engine);

        }

        private Func<T, R, R> ConcatRules<T, R>(IEnumerable<IRule<T, Func<R, R>>> rules) {
            if (rules == null || rules.Count() == 0) return null;
            RuleInvoker<T, Func<R, R>> invoker = CreateInvoker<T, Func<R, R>>(typeof(T), typeof(Func<R, R>));
            var ruleChain = rules.Select(r => invoker.Invoke(r)).Select(v => v.UnCurrey());

            return ruleChain.Aggregate((curr, next) => curr.Concact(next));

        }

        private RuleInvoker<T, TResult> CreateInvoker<T, TResult>(Type sourceType, Type targetType) {
            return (RuleInvoker<T, TResult>)Utilities.CreateType(typeof(RuleInvoker<,>), sourceType, targetType)
                                                                .CreateInstance();
        }
    }
}
