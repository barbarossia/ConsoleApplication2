using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        [TestMethod]
        public void Multi_Thread_Test() {
            DateTime processingBeginDateTime = DateTime.UtcNow;
            int count = 100;
            var list = Enumerable.Range(1, count)
               .Select(t => new Test1() { A = t }).ToArray();
            var taskList = new List<Task>();
            for(int i = 0; i < count; i++) {
                int temp = i;
                taskList.Add(Task.Factory.StartNew(() => {
                    Test(list[temp], temp);
                }));
            }
            Task.WaitAll(taskList.ToArray());
            DateTime processingEndDateTime = DateTime.UtcNow;
            double processingSeconds = ProcessTiming.DateDiff("s", processingEndDateTime, processingBeginDateTime);
            Console.WriteLine(processingSeconds);
        }
        [TestMethod]
        public void Multi_Thread_Instance_Test() {
            DateTime processingBeginDateTime = DateTime.UtcNow;
            int count = 100;
            var list = Enumerable.Range(1, count)
               .Select(t => new Test1() { A = t }).ToArray();
            var taskList = new List<Task>();
            for(int i = 0; i < count; i++) {
                int temp = i;
                taskList.Add(Task.Factory.StartNew(() => {
                    RunTestInstance(list[temp], temp);
                }));
            }
            Task.WaitAll(taskList.ToArray());
            DateTime processingEndDateTime = DateTime.UtcNow;
            double processingSeconds = ProcessTiming.DateDiff("s", processingEndDateTime, processingBeginDateTime);
            Console.WriteLine(processingSeconds);
        }
        private void RunTestInstance(Test1 t1, int seed) {
            TestInstance instance = (TestInstance)Utilities.CreateInstance(typeof(TestInstance), t1, seed);
            instance.Test();
        }
        private void Test(Test1 t1, int seed) {
            InnerContext inner = new InnerContext(seed);
            Context.InnerContext = inner;
            var engine = Context.EngineConext.Engine;
            Func<Test1, Test1> func = engine.Build<Test1>();
            doTest(t1, func);
        }
        private void doTest(Test1 t1, Func<Test1, Test1> func) {
            var result = func(t1);
            Assert.AreEqual(100, result.Details.Count());
            Assert.AreEqual(505000, result.Result);
        }
    }
}
