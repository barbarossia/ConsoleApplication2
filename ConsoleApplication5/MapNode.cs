using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class MapNode<T, R> : INode<T, IEnumerable<R>> {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public string Name
        {
            get
            {
                return "MapRule";
            }
        }

        public Func<T, IEnumerable<R>> Compile() {
            Func<T, IEnumerable<R>> result = null;
            Func<T, T> rule = CompileRule();
            if(rule == null) return result;
            Func<T, IEnumerable<R>> mapRule = CompileMapRule();
            if(mapRule != null) result = rule.ConcatRuleMap(mapRule);
            Func<IEnumerable<R>, IEnumerable<R>> foreachRule = CompileForEachRule();
            if (foreachRule != null) result = result.ConcatForEach(foreachRule);
            return result;
        }

        private Func<T, T> CompileRule() {
            var ruleNodes = GetRules<T>("Rule");
            if(ruleNodes == null) return null;
            return ruleNodes
                .Select(node => node.Compile())
                .Aggregate((curr, next) => curr.ConcatRule(next));
        }
        private Func<T, IEnumerable<R>> CompileMapRule() {
            var ruleNodes = GetRules<T, IEnumerable<R>>("MapRule");
            if(ruleNodes == null) return null;
            return ruleNodes
                .Select(node => node.Compile())
                .Aggregate((curr, next) => curr.ConcatMap(next));
        }
        private Func<IEnumerable<R>, IEnumerable<R>> CompileForEachRule() {
            var rules = GetRules<R>("ForEachRule");
            if(rules == null) return null;
            var foreachRuleNode = rules.SingleOrDefault();
            if(foreachRuleNode == null) return null;
            return foreachRuleNode.Compile().Enumerate();

        }
        private IEnumerable<INode<TResult>> GetRules<TResult>(string ruleName) {
            var rules = GetRules(ruleName);
            if(rules == null) return null;
            return rules.Cast<INode<TResult>>();
        }
        private IEnumerable<INode<T1, T2>> GetRules<T1, T2>(string ruleName) {
            var rules = GetRules(ruleName);
            if(rules == null) return null;
            return rules.Cast<INode<T1, T2>>();
        }
        public void SetRule(INode value) {
            string key = value.Name;
            if(key == null) throw new System.ArgumentNullException("key");
            List<INode> rules = GetRules(key);
            if(rules == null) {
                rules = new List<INode>();
                _values[key] = rules;
            }
            rules.Add(value);

        }

        private List<INode> GetRules(string key) {
            if(key == null) throw new System.ArgumentNullException("key");
            object result;
            if(_values.TryGetValue(key, out result)) {
                return (List<INode>)result;
            }

            //throw new KeyNotFoundException(key);
            return null;
        }
    }
}
