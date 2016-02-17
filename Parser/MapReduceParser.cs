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
                Result result = null;
                Advance();
                var initResult = InitListParser();
                var mapResult = MapRuleListParser();
                if(initResult.IsSuccessed && mapResult.IsSuccessed) {
                    result = MapAction(initResult, mapResult);
                } else if(initResult.IsSuccessed) {
                    result = new Result(initResult.Token, true, initResult.Expression);
                } else if(mapResult.IsSuccessed) {
                    result = new Result(mapResult.Token, true, mapResult.Expression);
                } else {
                    result = new Result(currentToken, false, null, new SyntaxException(""));
                }
                return result;
            } else {
                return new Result(currentToken, false, null, new SyntaxException(""));
            }
        }
        public Result InitListParser() {
            List<Result> results = new List<Result>();
            Result result;
            Token saved = currentToken;
            do {
                result = InitParser();
                if(result.IsSuccessed) {
                    results.Add(result);
                    Advance();
                }
            } while(result.IsSuccessed && currentToken != null);
            if(HasSuccessed(results)) {
                return InitAction(results.Where(r => r.IsSuccessed));
            } else {
                currentToken = saved;
                return new Result(null, false, null, new SyntaxException(""));
            }
        }
        public Result InitParser() {
            if(currentToken != null && currentToken.Name == RegisterKeys.Rule) {
                Type theType = currentToken.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(RuleInvoker<>), currentToken.SourceType)
                    .CreateInstance();
                return new Result(currentToken, true, invoker.Invoke(theType));
            } 
            return new Result(currentToken, false, null);
        }
        public Result MapRuleListParser() {
            List<Result> results = new List<Result>();
            Result result;
            Token saved = currentToken;
            do {
                result = MapRuleParser();
                if(result.IsSuccessed) {
                    results.Add(result);
                    Advance();
                }
            } while(result.IsSuccessed && currentToken != null);
            if(HasSuccessed(results)) {
                return MapRuleAction(results.Where(r => r.IsSuccessed));
            } else {
                currentToken = saved;
                return new Result(null, false, null, new SyntaxException(""));
            }
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
        private Result MapAction(Result init, Result map) {
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
                List<Result> results = new List<Result>();
                Advance();
                Result result;
                do {
                    result = ReduceRuleParser();
                    if(result.IsSuccessed) {
                        results.Add(result);
                        Advance();
                    }
                } while(result.IsSuccessed && currentToken !=null);
                if(HasSuccessed(results)) {
                    return ReduceAction(results.Where(r => r.IsSuccessed));
                } else {
                    currentToken = saved;
                    return new Result(null, false, null, new SyntaxException(""));
                }
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
        public Result ReduceRuleParser() {
            if(currentToken!= null && currentToken.Name == RegisterKeys.ReduceRule) {
                Type theType = currentToken.RuleType;
                var method = theType.GetMethod("Execute");
                LambdaExpression expression = ReduceRule(theType, currentToken.SourceType, currentToken.TargetType);
                return new Result(currentToken, true, expression);
            }
            return new Result(currentToken, false, null);
        }

        public LambdaExpression ReduceRule(Type ruleType, Type sourceType, Type targetType) {
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceRuleInvoker<, >), sourceType, targetType)
                            .CreateInstance();
            return invoker.Invoke(ruleType);
        }

        private bool HasSuccessed(IEnumerable<Result> results) {
            return results.Where(r => r.IsSuccessed).Any();
        }

    }
}
