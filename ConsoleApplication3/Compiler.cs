using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public abstract class Compiler  {
        public abstract INode Compile(RuleToken token);
        public abstract INode Compile(MapRuleToken token);
        public abstract INode Compile(ReduceRuleToken token);
        public abstract INode Compile(SequenceToken token);
        public abstract INode Compile(IfToken token);
        public abstract INode Compile(ForEachToken token);
        public INode Compile(Token token) {
            return token.Accept(this);
        }

    }

    public class NodeComiler : Compiler {
        public override INode Compile(ReduceRuleToken token) {
            return (INode)Utilities.CreateType(typeof(ReduceRuleNode<,>), token.SourceType, token.TargetType)
                                        .CreateInstance(token.Name);
        }

        public override INode Compile(MapRuleToken token) {
            return (INode)Utilities.CreateType(typeof(MapRuleNode<, >), token.SourceType, token.TargetType)
                                        .CreateInstance(token.Name);
        }

        public override INode Compile(RuleToken token) {
            return (INode)Utilities.CreateType(typeof(RuleNode<>), token.SourceType)
                                                    .CreateInstance(token.Name);
        }

        public override INode Compile(SequenceToken token) {
            List<Node> values = new List<Node>();
            foreach (var t in token.Nodes) {
                var child = t.Accept(this);
                //SetRule(t.Key, child, values);
            }
            return (INode)Utilities.CreateType(typeof(SequenceNode<,>), token.SourceType, token.TargetType)
                                        .CreateInstance(values);
        }
        public override INode Compile(IfToken token) {
            var truePart = token.TruePart;
            var falsePart = token.FalsePart;
            var trueNode = truePart.Accept(this);
            INode falseNode = null;
            if (falsePart != null) {
                falseNode = falsePart.Accept(this);
            }

            return (INode)Utilities.CreateType(typeof(IfNode<,>), token.SourceType, token.TargetType)
                                        .CreateInstance();
        }
        public override INode Compile(ForEachToken token) {
            var action = token.Action;
            var actionNode = action.Accept(this);

            return (INode)Utilities.CreateType(typeof(ForEachNode<,>), token.SourceType, token.SourceType)
                                        .CreateInstance(actionNode);
        }

        private void SetRule(IFluentScopeKey<INode> key, INode value, List<Node> values) {
            if (key == null) throw new System.ArgumentNullException("key");
            Node rules = GetNodes(key.Key, values);
            if (rules == null) {
                rules = new Node(key.Key);
                rules.Add(value);
            }
            rules.Add(value);

        }

        private Node GetNodes(string key, List<Node> values) {
            if (key == null) throw new System.ArgumentNullException("key");
            return values.SingleOrDefault(n => n.RuleName == key);
        }
    }
} 
