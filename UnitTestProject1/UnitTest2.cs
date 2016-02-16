using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication3;
using System.Collections.Generic;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest2 {
        [TestMethod]
        public void TestMethod1() {
            var rule1 = new IninValueOnT1();
            IRulesEngine engine = new RulesEngine<Test1, Test2>();
            RuleTypeDictionary d = new RuleTypeDictionary();
            d.Add(RuleKinds.Rule, rule1.GetType());
            engine.RuleTypes = d;

            var token = new RuleToken("IninValueOnT1", typeof(Test1));
            var parse = token.AsParser<Test1, Test1>();
            var r = parse(engine);
        }
        [TestMethod]
        public void TestMethod2() {
            var rule1 = new MapRuleOnT1IfTrue();
            IRulesEngine engine = new RulesEngine<Test1, Test2>();
            RuleTypeDictionary d = new RuleTypeDictionary();
            d.Add(RuleKinds.MapRule, rule1.GetType());
            engine.RuleTypes = d;

            var token = new MapRuleToken("MapRuleOnT1IfTrue", typeof(Test1), typeof(IEnumerable<Test2>));
            var parse = token.AsParser<Test1, IEnumerable<Test2>>();
            var r = parse(engine);
        }
        [TestMethod]
        public void TestMethod3() {
            var rule1 = new IninValueOnT1();
            var rule2 = new MapRuleOnT1IfTrue();
            var rule21 = new IninValueOnT2();
            IRulesEngine engine = new RulesEngine<Test1, Test2>();
            RuleTypeDictionary d = new RuleTypeDictionary();
            d.Add(RuleKinds.Rule, rule1.GetType());
            d.Add(RuleKinds.MapRule, rule2.GetType());
            d.Add(RuleKinds.ForEachRule, rule21.GetType());
            engine.RuleTypes = d;

            var token1 = new RuleToken("IninValueOnT1", typeof(Test1));
            var token2 = new MapRuleToken("MapRuleOnT1IfTrue", typeof(Test1), typeof(IEnumerable<Test2>));
            var token21 = new RuleToken("IninValueOnT2", typeof(Test2));
            var token22 = new ForEachToken(token21);
            //var parse = token21.AsParser<Test2, Test2>();
            //var f1 = parse(engine);
            //var ff = f1.Value.Enumerate();

            ParseFunc<Test1, IEnumerable<Test2>> f = null;
            f = from _1 in token1.AsParser<Test1, Test1>()
                from _2 in token2.AsParser<Test1, IEnumerable<Test2>>()
                select _1.Compose(_2);

            var f1 = from _3 in f
                     from _4 in token22.AsForEachParser<Test2, Test2>()
                     select _3.Compose(_4);
            //f = from _1 in token1.AsParser<Test1, Test1>()
            //    from _2 in token2.AsParser<Test1, IEnumerable<Test2>>()
            //    from left in f
            //    from _4 in token22.AsForEachParser<Test2, Test2>()
            //    from right in f
            //    select _1.Compose(_2);


            var funcResult = f1(engine);
             var t1 = new Test1() { A = 10 };
             var result = funcResult.Value(t1);
        }
        [TestMethod]
        public void TestMethod4() {
            var rule1 = new IninValueOnT1();
            var rule2 = new MapRuleOnT1IfTrue();
            var rule3 = new ReduceRuleOnT1();
            IRulesEngine engine = new RulesEngine<Test1, Test2>();
            RuleTypeDictionary d = new RuleTypeDictionary();
            d.Add(RuleKinds.Rule, rule1.GetType());
            d.Add(RuleKinds.MapRule, rule2.GetType());
            d.Add(RuleKinds.ReduceRule, rule3.GetType());
            engine.RuleTypes = d;

            var token1 = new RuleToken("IninValueOnT1", typeof(Test1));
            var token2 = new MapRuleToken("MapRuleOnT1IfTrue", typeof(Test1), typeof(IEnumerable<Test2>));
            var token3 = new ReduceRuleToken("ReduceRuleOnT1", typeof(IEnumerable<Test2>), typeof(Test1));
            //ParseFunc<Test1,  IEnumerable<Test2>> f = null;
            //f = (from _1 in token1.AsParser<Test1, Test1>()
            //        from _2 in token2.AsParser<Test1, IEnumerable<Test2>>()
            //        from left in f
            //        select _1.Compose(_2));

            //var funcResult = parse(engine);
            var t1 = new Test1() { A = 10 };
            //var t2 = new List<Test2>() { new Test2() { Result = 10, } };
            //var result = funcResult.Value.UnCurrey()(t2, t1);
        }

    }
}
