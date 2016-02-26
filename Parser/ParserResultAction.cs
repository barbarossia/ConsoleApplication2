using ClassLibrary1;
using MapReduce.Parser.Invokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public partial class ParserResult {
        public ParserResult Concat(ParserResult other) {
            if(!IsSuccessed || !other.IsSuccessed) return new ParserResult(false, new SyntaxException(""));
            switch(Token.Name) {
                case RegisterKeys.Rule:
                    switch(other.Token.Name) {
                        case RegisterKeys.Rule:
                            return RuleAction(other);
                        case RegisterKeys.MapRule:
                            return RuleMapAction(other);
                        default:
                            return new ParserResult(false, new SyntaxException(""));
                    }
                case RegisterKeys.ReduceRule:
                    switch(other.Token.Name) {
                        case RegisterKeys.ReduceRule:
                            return ReduceAction(other);
                        default:
                            return new ParserResult(false, new SyntaxException(""));
                    }
                case RegisterKeys.MapRule:
                    switch(other.Token.Name) {
                        case RegisterKeys.MapRule:
                            return MapRuleAction(other);
                        case RegisterKeys.ReduceRule:
                            return MapReduceAction(other);
                        case RegisterKeys.ForEach:
                            return MapForEachAction(other);
                        default:
                            return new ParserResult(false, new SyntaxException(""));
                    }
                default:
                    return new ParserResult(false, new SyntaxException(""));
            }
        }
        private ParserResult MapForEachAction(ParserResult other) {
            Type source = Token.SourceType;
            Type mid = Token.TargetType;
            Type target = other.Token.TargetType;
            var invoker = (IInvoker)Utilities.CreateType(typeof(MapForEachGroupInvoker<,,>), source, mid, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.MapRule, source, target);
            return new ParserResult(token, true, expr);
        }
        private ParserResult MapReduceAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.SourceType;
            var invoker = (IInvoker)Utilities.CreateType(typeof(MapReduceInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.Rule, source, source);
            return new ParserResult(token, true, expr);
        }
        private ParserResult RuleAction(ParserResult other) {
            Type source = Token.SourceType;
            var invoker = (IInvoker)Utilities.CreateType(typeof(RuleGroupInvoker<>), source)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.Rule, source, source);
            return new ParserResult(token, true, expr);
        }

        private ParserResult ReduceAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.TargetType;
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceBlockInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.ReduceRule, source, target);
            return new ParserResult(token, true, expr);
        }

        private ParserResult MapRuleAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.TargetType;
            var invoker = (IInvoker)Utilities.CreateType(typeof(MapGroupInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.MapRule, source, target);
            return new ParserResult(token, true, expr);
        }

        private ParserResult RuleMapAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.TargetType;
            var invoker = (IInvoker)Utilities.CreateType(typeof(RuleAndMapInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.MapRule, source, target);
            return new ParserResult(token, true, expr);
        }
    }
}
