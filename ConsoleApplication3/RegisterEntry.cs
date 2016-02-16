using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public interface IFluentScopeKey<T> {
        string Key { get; }
    }

    internal static class RegistryKeys {
        public static readonly IFluentScopeKey<IRule> Rule = Key<IRule>("Rule");
        public static readonly IFluentScopeKey<IRule> MapRule = Key<IRule>("MapRule");
        public static readonly IFluentScopeKey<IRule> ReduceRule = Key<IRule>("ReduceRule");
        public static readonly IFluentScopeKey<Type> SourceType = Key<Type>("SourceType");
        public static readonly IFluentScopeKey<Type> TargetType = Key<Type>("TargetType");
        public static readonly IFluentScopeKey<INode> Node = Key<INode>("RuleNode");
        public static readonly IFluentScopeKey<INode> MapNode = Key<INode>("MapRuleNode");
        public static readonly IFluentScopeKey<INode> ReduceNode = Key<INode>("ReduceRuleNode");
        public static readonly IFluentScopeKey<INode> SequenceNode = Key<INode>("SequenceNode");
        public static readonly IFluentScopeKey<INode> IfNode = Key<INode>("IfNode");
        public static readonly IFluentScopeKey<INode> ForEachNode = Key<INode>("ForEachNode");
        public static readonly IFluentScopeKey<IRule> ForEachRule = Key<IRule>("ForEachRule");
        private struct PropertyBagKey<T> : IFluentScopeKey<T> {
            public string Key { get; set; }
        }

        private static IFluentScopeKey<T> Key<T>(string key) {
            var result = new PropertyBagKey<T>() { Key = key };
            return result;
        }
    }
}
