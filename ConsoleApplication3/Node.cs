using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public class Node {
        private readonly List<INode> children = new List<INode>();
        public string RuleName { get; set; }
        public IEnumerable<INode> Children { get { return children; }  }
        public void Add(INode node) {
            children.Add(node);
        }
        public Node(string name) {
            RuleName = name;
        }
    }
    public interface INode {
        //string RuleKind { get; }
    }
    public interface INode<T, R> : INode {
        Func<T, R> Compile(IRulesEngine engine);
    }

    public class RuleNode<T> : INode<T, T> {
        public RuleNode(string name) {
            Name = name;
        }
        public string Name { get; set; }

        public Func<T, T> Compile(IRulesEngine engine) {
            var ruleType = engine.RuleTypes[Name];
            var invoker = CreateInvoker();
            return invoker.Invoke((IRule<T>)ruleType.CreateInstance());
        }
        protected IRuleInvoker<T, T> CreateInvoker() {
            return (IRuleInvoker<T, T>)Utilities.CreateType(typeof(RuleInvoker<,>), typeof(T), typeof(T))
                                                                .CreateInstance();
        }
    }
    public class MapRuleNode<T, R> : INode<T, IEnumerable<R>> {
        public MapRuleNode(string name) {
            Name = name;
        }
        public string Name { get; set; }

        public Func<T, IEnumerable<R>> Compile(IRulesEngine engine) {
            var ruleType = engine.RuleTypes[Name];
            var invoker = CreateInvoker();
            return invoker.Invoke((IRule<T, IEnumerable<R>>)ruleType.CreateInstance());
        }
        protected IRuleInvoker<T, IEnumerable<R>> CreateInvoker() {
            return (IRuleInvoker<T, IEnumerable<R>>)Utilities.CreateType(typeof(RuleInvoker<,>), typeof(T), typeof(IEnumerable<R>))
                                                                .CreateInstance();
        }

    }
    public class ReduceRuleNode<T, R> : INode<T, R> {
        public ReduceRuleNode(string name) {
            Name = name;
        }
        public string Name { get; set; }

        public Func<T, R> Compile(IRulesEngine engine) {
            var ruleType = engine.RuleTypes[Name];
            var invoker = CreateInvoker();
            return invoker.Invoke((IRule<T, R>)ruleType.CreateInstance());
        }
        protected IRuleInvoker<T, R> CreateInvoker() {
            return (IRuleInvoker<T, R>)Utilities.CreateType(typeof(RuleInvoker<,>), typeof(T), typeof(R))
                                                                .CreateInstance();
        }

    }
    public class ConditionNode  {
        public string Condition { get; set; }
    }
   
    public class IfNode<T, R> : INode<T, R> {
        public IfNode() {

        }
        public IfNode(ConditionNode cond, INode<T, R> truePart, INode<T, R> fasePart = null) {
            Condition = cond;
            TruePart = truePart;
            FalsePart = fasePart;
        }
        public ConditionNode Condition { get; set; }
        public INode<T, R> TruePart { get; set; }
        public INode<T, R> FalsePart { get; set; }
        public Func<T, R> Compile(IRulesEngine engine) {
            var conditionType = engine.ConditionTypes[Condition.Condition];
            var condition = (ICondition)conditionType.CreateInstance();
            if(condition.Is()) {
                return TruePart.Compile(engine);
            } else if(FalsePart != null) {
                return FalsePart.Compile(engine);
            } else
                return null;
        }
    }
    public class ForEachNode<T, R> : INode<IEnumerable<T>, IEnumerable<R>> {
        public ForEachNode() {

        }
        public ForEachNode(INode<T, R> action) {
            Action = action;
        }
        public INode<T, R> Action { get; set; }

        public Func<IEnumerable<T>, IEnumerable<R>> Compile(IRulesEngine engine) {
            return Action.Compile(engine).Enumerate();
        }

    }
}

