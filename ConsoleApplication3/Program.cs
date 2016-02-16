using System;
using System.Collections.Generic;
using System.Xml;

namespace ConsoleApplication3 {
    class Program {
        static void Main(string[] args) {

            var t1 = new Test1() { A = 10 };
            //var func = Build1();
            //var func = ruleGroup.Compile<Test1>();

            var engine = GetRulesEngine();
            var func = engine.Build();
            var result = func(t1);
            Console.ReadLine();
        }

        static RulesEngine<Test1, Test1> GetRulesEngine() {
            string configKey = @"TestRule2.Config";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configKey);
            RulesEngineConfigurationXmlProvider<Test1> xmlProvider = new RulesEngineConfigurationXmlProvider<Test1>();
            xmlProvider.Load(xmlDoc);
            return xmlProvider.GetRulesEngine<Test1, Test1>();
        }
        static Func<Test1, Test1> Build1() {

            Func<Test1, Test1> selfTest1 = RuleHelper.Spin(new IninValueOnT1());
            Func<Test2, Test2> selfTest2 = RuleHelper.Spin(new IninValueOnT2());
            Func<Test3, Test3> selfTest3 = RuleHelper.Spin(new IninValueOnT3());

            var map1 = RuleHelper.Map(new MapRuleOnT1IfTrue());
            var map2 = RuleHelper.Map(new MapRuleOnT2());

            Func<IEnumerable<Test3>, Test2, Test2> reduceTest21 = RuleHelper.Reduce(new ReduceRuleOnT2());
            Func<IEnumerable<Test3>, Test2, Test2> reduceTest22 = RuleHelper.Reduce(new AssignRuleOnT2());

            Func<IEnumerable<Test2>, Test1, Test1> reduceTest11 = RuleHelper.Reduce(new ReduceRuleOnT1());
            Func<IEnumerable<Test2>, Test1, Test1> reduceTest12 = RuleHelper.Reduce(new AssignRuleOnT1());

            Func<Test2, IEnumerable<Test3>> mapTest2 = selfTest2
                .Compose(map2)
                .Concat(selfTest3.Enumerate());

            Func<Test1, IEnumerable<Test2>> mapTest1 = selfTest1
                .Compose(map1)
                .Compose(Helper.Enumerate(mapTest2.Reduce(reduceTest21.Concact(reduceTest22))));

            return mapTest1.Reduce(reduceTest11.Concact(reduceTest12));
        }

        static RuleGroup Build() {
            var group3 = new RuleGroup();
            group3.Set(RegistryKeys.SourceType, typeof(Test3));
            group3.SetRule(RegistryKeys.Rule, new IninValueOnT3());

            var group2 = new RuleGroup();
            group2.Set(RegistryKeys.SourceType, typeof(Test2));
            group2.Set(RegistryKeys.TargetType, typeof(Test3));
            group2.SetRule(RegistryKeys.Rule, new IninValueOnT2());
            group2.SetRule(RegistryKeys.MapRule, new MapRuleOnT2());
            group2.SetRule(RegistryKeys.ReduceRule, new ReduceRuleOnT2());
            group2.SetRule(RegistryKeys.ReduceRule, new AssignRuleOnT2());
            group2.Child = group3;

            var group1 = new RuleGroup();
            group1.Set(RegistryKeys.SourceType, typeof(Test1));
            group1.Set(RegistryKeys.TargetType, typeof(Test2));
            group1.SetRule(RegistryKeys.Rule, new IninValueOnT1());
            group1.SetRule(RegistryKeys.MapRule, new MapRuleOnT1IfTrue());
            group1.SetRule(RegistryKeys.ReduceRule, new ReduceRuleOnT1());
            group1.SetRule(RegistryKeys.ReduceRule, new AssignRuleOnT1());
            group1.Child = group2;

            return group1;
        }
    }
}
