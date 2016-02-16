using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public class SequenceNode<T, R> : INode<T, R> {
        private List<INode> _nodes;
        private List<Node> _values;
        public SequenceNode(IEnumerable<INode> values) {
            //_values = new List<Node>(values);
            _nodes = new List<INode>(values); 
        }
        public Func<T, R> Compile(IRulesEngine engine) {
            //foreach(var node in _nodes) {
            //    var f = node
            //}
            throw new NotImplementedException();
        }
        //private Func<T, R> CreateConcanate(INode first, INode second, IRulesEngine engine) {
        //    var result =(IConcatenateResult<T, R>)Utilities.CreateType(typeof(ConcatenateResult<,,>), s1, m1, r1)
        //                                                        .CreateInstance(first, second);
        //    return result.GetResult(engine);
        //}
        private Func<T, T> CompileRuleNode(Node node, IRulesEngine engine) {
            return node.Children
                .Select(c => (RuleNode<T>)c)
                .Select(c1 => c1.Compile(engine))
                .Aggregate((curr, next) => curr.Concact(next));
        }
        private Func<T, IEnumerable<Mid>> CompileMapRuleNode<Mid>(Node node, IRulesEngine engine) {
            return node.Children
                .Select(c => (MapRuleNode<T, Mid>)c)
                .Select(c1 => c1.Compile(engine))
                .Aggregate((curr, next) => curr.Concact(next));
        }

        //private Func<IEnumerable<Mid>, R, R> CompileReduceRuleNode<Mid>(Node node, IRulesEngine engine) {
        //    return node.Children
        //        .Select(c => (ReduceRuleNode<Mid, R>)c)
        //        .Select(c1 => c1.Compile(engine))
        //        .Select(c2 => c2.UnCurrey())
        //        .Aggregate((curr, next) => curr.Concact(next));
        //}
    }
}
