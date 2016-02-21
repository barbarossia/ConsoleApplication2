using ClassLibrary1;
using MapReduce.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class Parser {
        private ITokenManagement tokenManagement;
        private TokenInfo currentToken;
        public Parser(ITokenManagement tokenMgmt) {
            tokenManagement = tokenMgmt;
            currentToken = tokenManagement.Root;
        }
        private void Advance() {
            currentToken = tokenManagement.GetNextToken(currentToken);
        }
        private TokenBuffer tokenBuffer;
        private Context context;
        public Parser(TokenBuffer buffer, Context ctx) {
            tokenBuffer = buffer;
            context = ctx;
        }
        public ParserResult Execute() {
            if(currentToken.Name == RegisterKeys.MapReduce) {
                ParserResult result = null;
                Advance();
                var mapResult = MapBlock();
                if(mapResult.IsSuccessed) {
                    var reduceResult = ReduceBlock();
                    if(reduceResult.IsSuccessed) {
                        result = MapReduceAction(mapResult, reduceResult);
                        return result;
                    } else {
                        return new ParserResult(reduceResult.Token, false, null, new SyntaxException(""));
                    }
                } else {
                    return new ParserResult(mapResult.Token, false, null, new SyntaxException(""));
                }
            } else {
                return new ParserResult(currentToken, false, null, new SyntaxException(""));
            }
        }
        private ParserResult MapReduceAction(ParserResult mapResult, ParserResult reduceResult) {
            Type source = mapResult.Token.SourceType;
            Type target = reduceResult.Token.SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapReduceInvoker<,>), source, target)
                           .CreateInstance(mapResult.Expression, reduceResult.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo() { SourceType = source, TargetType = target };
            return new ParserResult(token, true, expr);
        }

        public ParserResult MapBlock() {
            List<ParserResult> results = new List<ParserResult>();
            Token t;
            ParserResult result = null;
            bool parseSuccess = false;
            tokenBuffer.Backup();
            t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.MAP) {
                tokenBuffer.Consume();
                parseSuccess = true;
                result = OptionalRuleListParser();
                if (result.IsSuccessed) {
                    results.Add(result);
                }
                result = MapRuleListParser();
                parseSuccess = result.IsSuccessed;
                if (parseSuccess) {
                    results.Add(result);
                    t = tokenBuffer.Current;
                    if (t != null && t.TokenType == TokenType.EOF) {
                        tokenBuffer.Commit();
                        result = MapAction(results);
                    } else {
                        parseSuccess = false;
                        result = new ParserResult(null, false, null);
                    }
                }
            }
            if (!parseSuccess) {
                tokenBuffer.Rollback();
                result = new ParserResult(null, false, null, new SyntaxException("Map node must contains more than one element!"));
            }
            return result;
        }


        private ParserResult OptionalRuleListParser() {
            List<ParserResult> results = new List<ParserResult>();
            tokenBuffer.Backup();
            bool parseSuccess = true;
            while (parseSuccess && tokenBuffer.Current.TokenType != TokenType.EOF) {
                var result = RuleParser();
                if (result.IsSuccessed) { results.Add(result); tokenBuffer.Consume(); }
                    parseSuccess = result.IsSuccessed;
            }
            if (results.Any()) {
                var result = RuleAction(results);
                return result;
            } else {
                tokenBuffer.Rollback();
                return new ParserResult(null, false, null);
            }
        }
        private ParserResult RuleParser() {
            Token t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.RULE) {
                TokenInfo info = Create(t.Value);
                Type theType = info.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(RuleInvoker<>), info.SourceType)
                    .CreateInstance();
                return new ParserResult(info, true, invoker.Invoke(theType));
            }
            return new ParserResult(null, false, null);
        }
        private ParserResult MapRuleListParser() {
            List<ParserResult> results = new List<ParserResult>();
            tokenBuffer.Backup();
            bool parseSuccess = false;
            var result = MapRuleParser();
            parseSuccess = result.IsSuccessed;
            if (parseSuccess) {
                while (parseSuccess) {
                    results.Add(result);
                    tokenBuffer.Consume();
                    result = MapRuleParser();
                    parseSuccess = result.IsSuccessed;
                }
                parseSuccess = true;
            }
            if (parseSuccess) {
                result = MapRuleAction(results);
                return result;
            } else {
                tokenBuffer.Rollback();
                return new ParserResult(null, false, null);
            }
        } 
        public ParserResult MapRuleParser() {
            Token t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.MAPRULE) {
                TokenInfo info = Create(t.Value);
                Type theType = info.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(MapRuleInvoker<,>), info.SourceType, info.TargetType)
                   .CreateInstance();
                return new ParserResult(info, true, invoker.Invoke(theType));              
            } 
            return new ParserResult(null, false, null);
        }
        private ParserResult MapRuleAction(IEnumerable<ParserResult> results) {
            var p = results.Where(r => r.IsSuccessed).First();
            Type source = p.Token.SourceType;
            Type target = p.Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapGroupInvoker<,>), source, target)
                           .CreateInstance(results.Where(r => r.IsSuccessed).Select(r => r.Expression));
            var expr = invoker.Invoke();
            var token = new TokenInfo() { SourceType = source, TargetType = target, Name = RegisterKeys.MapRule };
            return new ParserResult(token, true, expr);
        }
        private ParserResult MapAction(IEnumerable<ParserResult> results) {
            int count = results.Count();
            if (count == 1) return results.First();
            if (count == 2) { 
                ParserResult init = results.First();
                ParserResult map = results.Last();
                Type source = init.Token.SourceType;
                Type target = map.Token.TargetType;
                var invoker = (IGroupInvoker)Utilities.CreateType(typeof(InitMapInvoker<,>), source, target)
                               .CreateInstance(init.Expression, map.Expression);
                var expr = invoker.Invoke();
                var token = new TokenInfo() { SourceType = source, TargetType = target, Name = RegisterKeys.MapRule };
                return new ParserResult(token, true, expr);
            }
            return new ParserResult(null, false, null);
        }
        private ParserResult RuleAction(IEnumerable<ParserResult> results) {
            Type source = results.First().Token.SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(RuleGroupInvoker<>), source)
                           .CreateInstance(results.Where(r => r.IsSuccessed).Select(r => r.Expression));
            var expr = invoker.Invoke();
            var token = new TokenInfo() { SourceType = source, Name =RegisterKeys.Rule };
            return new ParserResult(token, true, expr);
        }
        public ParserResult ReduceBlock() {
            Token t;
            ParserResult result=null;
            bool parseSuccess = false;
            tokenBuffer.Backup();
            t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.REDUCE) {
                tokenBuffer.Consume();
                result = ReduceList();
                parseSuccess = result.IsSuccessed;
                if (parseSuccess) {
                    t = tokenBuffer.Current;
                    if (t != null && t.TokenType == TokenType.EOF) {
                        tokenBuffer.Commit();
                    } else {
                        parseSuccess = false;
                        result = new ParserResult(null, false, null);
                    }
                }
            }
            if (!parseSuccess) {
                tokenBuffer.Rollback();
                result = new ParserResult(null, false, null, new SyntaxException("Reduce node must contains more than one element!"));
            }
            return result;
        }

        private ParserResult ReduceList() {
            List<ParserResult> results = new List<ParserResult>();
            tokenBuffer.Backup();
            bool parseSuccess = false;
            var result = ReduceRuleParser();
            parseSuccess = result.IsSuccessed;
            if (parseSuccess) {
                while (parseSuccess) {
                    results.Add(result);
                    tokenBuffer.Consume();
                    result = ReduceRuleParser();
                    parseSuccess = result.IsSuccessed;
                }
                parseSuccess = true;
            }
            if (parseSuccess) {                            
                result = ReduceAction(results);
                return result;
            } else {
                tokenBuffer.Rollback();
                return new ParserResult(null, false, null);
            }
        }
        
        private ParserResult ReduceAction(IEnumerable<ParserResult> results) {
            Type source = results.First().Token.SourceType;
            Type target = results.First().Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(ReduceInvoker<,>), source, target)
                           .CreateInstance(results.Where(r => r.IsSuccessed).Select(r => r.Expression));
            var expr = invoker.Invoke();
            var token = new TokenInfo() { SourceType = source, TargetType = target };
            return new ParserResult(token, true, expr);
        }
        private ParserResult ReduceRuleParser() {
            Token t = tokenBuffer.Current;
            if (t!= null && t.TokenType == TokenType.REDUCERULE) {
                TokenInfo info = Create(t.Value);
                Type theType = info.RuleType;
                var method = theType.GetMethod("Execute");
                LambdaExpression expression = ReduceRule(theType, info.SourceType, info.TargetType);
                return new ParserResult(info, true, expression);
            }
            return new ParserResult(null, false, null);
        }

        private LambdaExpression ReduceRule(Type ruleType, Type sourceType, Type targetType) {
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceRuleInvoker<, >), sourceType, targetType)
                            .CreateInstance();
            return invoker.Invoke(ruleType);
        }

        private bool HasSuccessed(IEnumerable<ParserResult> results) {
            return results.Any();
        }
        private TokenInfo Create(XElement input) {
            var name = input.Attribute("Type").Value;
            string ruleType = (string)context.Items[name];
            Type theType = Type.GetType(ruleType);
            var method = theType.GetMethod("Execute");
            Type source = GetType(method.GetParameters()[0].ParameterType);
            Type target = GetType(method.ReturnType);
            return new TokenInfo() { RuleType = theType, Name = input.Name.LocalName, SourceType = source, TargetType = target, Image = input };
        }
        private Type GetType(Type theType) {
            Type result = theType;
            if ("IEnumerable`1" == theType.Name) {
                result = theType.GenericTypeArguments[0];
            }
            return result;
        }
    }
}
