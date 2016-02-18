using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parser {
    public class MapReduceParser {
        private ITokenManagement tokenManagement;
        private Token currentToken;
        public MapReduceParser(ITokenManagement tokenMgmt) {
            tokenManagement = tokenMgmt;
            currentToken = tokenManagement.Root;
        }
        private void Advance() {
            currentToken = tokenManagement.GetNextToken(currentToken);
        }
        public Result Execute() {
            if(currentToken.Name == RegisterKeys.MapReduce) {
                Result result = null;
                Advance();
                var mapResult = MapParser();
                if(mapResult.IsSuccessed) {
                    var reduceResult = ReduceParser();
                    if(reduceResult.IsSuccessed) {
                        result = MapReduceAction(mapResult, reduceResult);
                        return result;
                    } else {
                        return new Result(reduceResult.Token, false, null, new SyntaxException(""));
                    }
                } else {
                    return new Result(mapResult.Token, false, null, new SyntaxException(""));
                }
            } else {
                return new Result(currentToken, false, null, new SyntaxException(""));
            }
        }
        private Result MapReduceAction(Result mapResult, Result reduceResult) {
            Type source = mapResult.Token.SourceType;
            Type target = reduceResult.Token.SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapReduceInvoker<,>), source, target)
                           .CreateInstance(mapResult.Expression, reduceResult.Expression);
            var expr = invoker.Invoke();
            var token = new Token() { SourceType = source, TargetType = target };
            return new Result(token, true, expr);
        }

        public Result MapParser() {
            if(currentToken.Name == RegisterKeys.Map) {
                Advance();
                return OptionalParser((results) => MapAction(results)
                , new ParserAndAction((results) => InitAction(results), () => InitParser())
                , new ParserAndAction((results) => MapRuleAction(results), () => MapRuleParser()));
            } else {
                return new Result(currentToken, false, null, new SyntaxException(""));
            }
        }
        public class ParserAndAction {
            public ParserAction Action { get; set; }
            public TerminalParser Parser { get; set; }
            public ParserAndAction(ParserAction action, TerminalParser parser) {
                Action = action;
                Parser = parser;
            }
        }
        private Result OptionalParser(ParserAction action, params ParserAndAction[] parsers) {
            List<Result> results = new List<Result>();
            Result lastResult = null;
            Token saved = currentToken;
            bool last = false;
            var iterator = parsers.GetEnumerator();
            last = iterator.MoveNext();
            do {
                if(!last) break;
                var parserAndAction = (ParserAndAction)iterator.Current;
                var parser = parserAndAction.Parser;
                var tmpReslt = parser();
                if(tmpReslt.IsSuccessed) {
                    var action = parserAndAction.Action;
                    lastResult = action(tmpReslt);
                    results.Add(lastResult);
                    saved = currentToken;
                } else {
                    currentToken = saved;
                    last = iterator.MoveNext();
                }               
            } while(last && currentToken != null);
            if(lastResult!= null && lastResult.IsSuccessed && !iterator.MoveNext()) {
                return action(results);
            } else if(HasSuccessed(results)) {
                return results.Where(r => r.IsSuccessed).First();
            } else {
                return new Result(null, false, null, new SyntaxException(""));
            }
        }
        public delegate Result TerminalParser();
        public delegate Result ParserAction(IEnumerable<Result> results);
        private Result ListParser(ParserAction action, TerminalParser parser) {
            List<Result> results = new List<Result>();
            Result lastResult;
            Token saved = currentToken;
            do {
                lastResult = parser();
                if(lastResult.IsSuccessed) {
                    results.Add(lastResult);
                    saved = currentToken;
                    Advance();
                }
            } while(lastResult.IsSuccessed && currentToken != null );
            if(lastResult.IsSuccessed) {
                return action(results);
            } else {
                return new Result(null, false, null, new SyntaxException(""));
            }
        }

        private Result InitListParser() {
            return OptionalParser((results) => InitAction(results), () => InitParser());
        }
        private Result InitParser() {
            if(currentToken != null && currentToken.Name == RegisterKeys.Rule) {
                Type theType = currentToken.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(RuleInvoker<>), currentToken.SourceType)
                    .CreateInstance();
                return new Result(currentToken, true, invoker.Invoke(theType));
            } 
            return new Result(currentToken, false, null);
        }
        private Result MapRuleListParser() {
            return OptionalParser((results) => MapRuleAction(results), () => MapRuleParser());
        } 
        public Result MapRuleParser() {
            if(currentToken != null && currentToken.Name == RegisterKeys.MapRule) {
                Type theType = currentToken.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(MapRuleInvoker<, >), currentToken.SourceType, currentToken.TargetType)
                    .CreateInstance();
                return new Result(currentToken, true, invoker.Invoke(theType));
            } 
            return new Result(currentToken, false, null);
        }
        private Result MapRuleAction(IEnumerable<Result> results) {
            var p = results.Where(r => r.IsSuccessed).First();
            Type source = p.Token.SourceType;
            Type target = p.Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapGroupInvoker<,>), source, target)
                           .CreateInstance(results.Where(r => r.IsSuccessed).Select(r => r.Expression));
            var expr = invoker.Invoke();
            var token = new Token() { SourceType = source, TargetType = target };
            return new Result(token, true, expr);
        }
        private Result MapAction(IEnumerable<Result> results) {
            Result init = results.First();
            Result map = results.Last();
            Type source = init.Token.SourceType;
            Type target = map.Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(InitMapInvoker<,>), source, target)
                           .CreateInstance(init.Expression, map.Expression);
            var expr = invoker.Invoke();
            var token = new Token() { SourceType = source, TargetType = target };
            return new Result(token, true, expr);
        }
        private Result InitAction(IEnumerable<Result> results) {
            Type source = results.First().Token.SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(RuleGroupInvoker<>), source)
                           .CreateInstance(results.Where(r => r.IsSuccessed).Select(r => r.Expression));
            var expr = invoker.Invoke();
            var token = new Token() { SourceType = source };
            return new Result(token, true, expr);
        }
        public Result ReduceParser() {
            Token saved = currentToken;
            if (currentToken.Name == RegisterKeys.Reduce) {
                Advance();
                return ListParser((results) => ReduceAction(results), () => ReduceRuleParser());
            } else {
                currentToken = saved;
                return new Result(saved, false, null);
            }
        }
        
        private Result ReduceAction(IEnumerable<Result> results) {
            Type source = results.First().Token.SourceType;
            Type target = results.First().Token.TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(ReduceInvoker<,>), source, target)
                           .CreateInstance(results.Where(r => r.IsSuccessed).Select(r => r.Expression));
            var expr = invoker.Invoke();
            var token = new Token() { SourceType = source, TargetType = target };
            return new Result(token, true, expr);
        }
        private Result ReduceRuleParser() {
            if(currentToken!= null && currentToken.Name == RegisterKeys.ReduceRule) {
                Type theType = currentToken.RuleType;
                var method = theType.GetMethod("Execute");
                LambdaExpression expression = ReduceRule(theType, currentToken.SourceType, currentToken.TargetType);
                return new Result(currentToken, true, expression);
            }
            return new Result(currentToken, false, null);
        }

        private LambdaExpression ReduceRule(Type ruleType, Type sourceType, Type targetType) {
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceRuleInvoker<, >), sourceType, targetType)
                            .CreateInstance();
            return invoker.Invoke(ruleType);
        }

        private bool HasSuccessed(IEnumerable<Result> results) {
            return results.Any();
        }

    }
}
