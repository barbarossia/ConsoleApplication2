using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public enum ConcateAction {
        Init,
        Map,
        Reduce,
        ForEach,
    }
    public class DFAMode {
        private List<DFAState> m_dfaStates;
        private DFAState m_currentState;
        public DFAMode(DFAState start) {
            m_dfaStates = new List<DFAState>();
            m_dfaStates.Add(start);
            m_currentState = start;
        }
        public List<DFAState> States
        {
            get
            {
                return m_dfaStates;
            }
        }
        public void AddState(DFAState state) {
            m_dfaStates.Add(state);
        }
        public ConcateAction CurrentState
        {
            get
            {
                return m_currentState.Value;
            }
        }
        public void Input(string rule) {
            //go to next state
            var nextState = m_currentState.OutEdges.Where(e => e.Rule == rule).First().TargetState;
            m_currentState = nextState;
        }
        public void Reset(DFAState state) {
            m_currentState = state;
        }
    }
    public class DFAEdge {
        public DFAState TargetState { get; private set; }
        public string Rule { get; set; }
        public DFAEdge(string rule, DFAState targetState)
        {
            Rule = rule;
            TargetState = targetState;
        }
    }
    public class DFAState {
        private List<DFAEdge> m_outEdges;
        public ConcateAction Value { get; set; }
        public DFAState(ConcateAction value) {
            Value = value;
            m_outEdges = new List<DFAEdge>();
        }
        public List<DFAEdge> OutEdges
        {
            get
            {
                return m_outEdges;
            }
        }
        public void AddEdge(DFAEdge edge) {

            m_outEdges.Add(edge);
        }
    }
    public interface IConcatenateResult<T, R> {
        Func<T, R> GetResult(IRulesEngine engine);
    }

    public class ConcatenateResult<T, M, R> : IConcatenateResult<T, R> {
        public Func<T, M> _node;
        public INode<M, R> _next;
        private DFAMode dfa;
        public ConcatenateResult() {

        }
        public ConcatenateResult(Func<T, M> node, INode<M, R> next) {
            _node = node;
            _next = next;
            dfa = CreateDFA();
        }
        public virtual Func<T, R> GetResult(IRulesEngine engine) {
            Func<M, R> r = _next.Compile(engine);
            return _node.Compose(r);
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
            dfa = new DFAMode(init);
            dfa.AddState(map);
            dfa.AddState(f);
            dfa.AddState(reduce);
            return dfa;
        }
    }
}
