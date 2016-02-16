using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public abstract class Token {
        public IFluentScopeKey<IRule> Key { get; set; }
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public Token() {

        }
        public Token(Type source, Type target) {
            SourceType = source;
            TargetType = target;
        }
        public abstract INode Accept(Compiler compiler);
    }

    public class RuleToken : Token {
        public RuleToken(string name, Type source) : base(source, null) {
            Name = name;
            Key = RegistryKeys.Rule;
        }
        public string Name { get; set; }

        public override INode Accept(Compiler compiler) {
            return (INode)compiler.Compile(this);
        }
    }
    public class MapRuleToken : Token {
        public MapRuleToken(string name, Type source, Type target) : base(source, target) {
            Name = name;
            Key = RegistryKeys.MapRule;
        }
        public string Name { get; set; }
        public override INode Accept(Compiler compiler) {;
            return compiler.Compile(this);
        }
    }

    public class ReduceRuleToken : Token {
        public ReduceRuleToken(string name, Type source, Type target) : base(source, target) {
            Name = name;
            Key = RegistryKeys.ReduceRule;
        }
        public string Name { get; set; }
        public override INode Accept(Compiler compiler) {
            return compiler.Compile(this);
        }
    }
    public class SequenceToken: Token {
        public List<Token> Nodes { get; set; }
        public SequenceToken(IEnumerable<Token> list) {
            Nodes = new List<Token>(list);
            SourceType = list.First().SourceType;
            TargetType = list.Last().TargetType;
            //Key = RegistryKeys.SequenceNode;
        }
        public override INode Accept(Compiler compiler) {
            return compiler.Compile(this);
        }
    }
    public class IfToken : Token {
        public IfToken(ConditionNode cond, Token truePart, Token falsePart = null) {
            Condition = cond;
            TruePart = truePart;
            FalsePart = falsePart;
            SourceType = truePart.SourceType;
            TargetType = truePart.TargetType;
            //Key = RegistryKeys.IfNode;
        }
        public ConditionNode Condition { get; set; }
        public Token TruePart { get; set; }
        public Token FalsePart { get; set; }
        public override INode Accept(Compiler compiler) {
            return compiler.Compile(this);
        }

    }
    public class ForEachToken : Token {
        public ForEachToken(Token action) {
            Action = action;
            SourceType = action.SourceType;
            TargetType = action.TargetType;
            Key = RegistryKeys.ForEachRule;
        }
        public Token Action { get; set; }
        public override INode Accept(Compiler compiler) {
            return compiler.Compile(this);
        }
    }
}
