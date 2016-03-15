using ClassLibrary1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class TestInstance {
        public Test1 T1 { get; private set; }
        public int Seed { get; set; }
        public TestInstance(Test1 t1, int seed) {
            T1 = t1;
            Seed = seed;
        }
        public void Test() {
            InnerContext inner = new InnerContext(Seed);
            Context.InnerContext = inner;
            var engine = Context.EngineConext.Engine;
            Func<Test1, Test1> func = engine.Build<Test1>();
            doTest(T1, func);
        }

        private void doTest(Test1 t1, Func<Test1, Test1> func) {
            var result = func(t1);
            Assert.AreEqual(100, result.Details.Count());
            Assert.AreEqual(505000, result.Result);
        }
    }

}
