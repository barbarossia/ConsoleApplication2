using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApplication3;

namespace UnitTestProject1 {
    [TestClass]
    public class DFAUnitTest {
        [TestMethod]
        public void CreateDFATest() {
            var dfa = CreateDFA();
            dfa.Input(RuleKinds.Rule);

            Assert.AreEqual(ConcateAction.Init, dfa.CurrentState);
            dfa.Input(RuleKinds.MapRule);
            Assert.AreEqual(ConcateAction.Map, dfa.CurrentState);
            dfa.Input(RuleKinds.ReduceRule);
            Assert.AreEqual(ConcateAction.Reduce, dfa.CurrentState);
        }

        private DFAMode CreateDFA() {
            DFAState init = new DFAState(ConcateAction.Init);
            DFAState map = new DFAState(ConcateAction.Map);
            DFAState f = new DFAState(ConcateAction.ForEach);
            DFAState reduce = new DFAState(ConcateAction.Reduce);
            init.AddEdge(new DFAEdge("Rule", init));
            init.AddEdge(new DFAEdge("MapRule", map));

            map.AddEdge(new DFAEdge("MapRule", map));
            map.AddEdge(new DFAEdge("ForEachRule", f));
            map.AddEdge(new DFAEdge("ReduceRule", reduce));

            f.AddEdge(new DFAEdge("ForEachRule", f));

            reduce.AddEdge(new DFAEdge("ReduceRule", reduce));
            var dfa = new DFAMode(init);
            dfa.AddState(map);
            dfa.AddState(f);
            dfa.AddState(reduce);

            return dfa;
        }
    }
}
