using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using System.Linq;

namespace MapReduce.Parser.UnitTest {
    [TestClass]
    public class UnitTest6 {
        [TestMethod]
        public void LoadConfiguration() {
            string configKey = @"TestRule1.Config";
            RulesEngineConfigurationXmlProvider provider = new RulesEngineConfigurationXmlProvider(configKey);
            var engine = provider.GetRulesEngine();
            var func = engine.Build<Test1>();

            var t1 = new Test1() { A = 10 };
            Test1 result = func(t1);
            Assert.AreEqual(10, result.Details.Count());
            Assert.AreEqual(220, result.Result);
        }
        [TestMethod]
        public void LoadNestedMapReduce() {
            string configKey = @"TestRule2.Config";
            RulesEngineConfigurationXmlProvider provider = new RulesEngineConfigurationXmlProvider(configKey);
            var engine = provider.GetRulesEngine();
            var func = engine.Build<Test1>();

            var t1 = new Test1() { A = 10 };
            Test1 result = func(t1);
            Assert.AreEqual(10, result.Details.Count());
            Assert.AreEqual(220, result.Result);
        }
    }
}
