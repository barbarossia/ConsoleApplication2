using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ReduceNode<T, R> : IReduceNode<IEnumerable<T>, R> {
        private readonly List<IReduceNode<IEnumerable<T>, R>> _values;
        public string Name
        {
            get
            {
                return "ReduceRule";
            }
        }
        public ReduceNode(IEnumerable<IReduceNode<IEnumerable<T>, R>> nodes) {
            _values = new List<IReduceNode<IEnumerable<T>, R>>(nodes);
        }
        public Func<IEnumerable<T>, R, R> Compile() {
            return _values
               .Select(node => node.Compile())
               .Aggregate((curr, next) => curr.ConcactReduce(next));
        }
    }
}
