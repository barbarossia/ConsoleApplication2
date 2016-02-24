using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class ParserResult {
        public bool IsSuccessed { get; set; }
        public LambdaExpression Expression { get; set; }
        public TokenInfo Token { get; set; }
        public SyntaxException Error { get; set; }
        public ParserResult(TokenInfo token, bool isSuccessed, LambdaExpression expr, SyntaxException error = null) {
            IsSuccessed = isSuccessed;
            Expression = expr;
            Token = token;
            Error = error;
        }
        public ParserResult(bool isSuccessed, SyntaxException error) {
            IsSuccessed = isSuccessed;
            Error = error;
        }
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
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(ForEachGroupInvoker<, ,>), source, mid, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.MapRule, source, target);
            return new ParserResult(token, true, expr);
        }
        private ParserResult MapReduceAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapReduceInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.Rule, source, source);
            return new ParserResult(token, true, expr);
        }
        private ParserResult RuleAction(ParserResult other) {
            Type source = Token.SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(RuleGroupInvoker<>), source)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.Rule, source, source);
            return new ParserResult(token, true, expr);
        }

        private ParserResult ReduceAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(ReduceInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.ReduceRule, source, target);
            return new ParserResult(token, true, expr);
        }

        private ParserResult MapRuleAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapGroupInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.MapRule, source, target);
            return new ParserResult(token, true, expr);
        }

        private ParserResult RuleMapAction(ParserResult other) {
            Type source = Token.SourceType;
            Type target = other.Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(InitMapInvoker<,>), source, target)
                           .CreateInstance(Expression, other.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.MapRule, source, target);
            return new ParserResult(token, true, expr);
        }
    }
}
