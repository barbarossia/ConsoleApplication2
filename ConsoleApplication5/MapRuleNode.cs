using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class MapRuleNode<T, R> : INode<T, IEnumerable<R>> {
        public string Name { get { return "MapRule"; } }
        public Type RuleType { get; set; }
        public MapRuleNode(Type name) {
            RuleType = name;
        }

        public Func<T, IEnumerable<R>> Compile() {
            var rule = (IMapRule<T, R>)RuleType.CreateInstance();
            return (t) => rule.Execute(t);
        }
    }
}
