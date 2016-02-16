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
        private Context ctx;
        public MapReduceParser(Context context) {
            ctx = context;
        }
        public Result MapParser(XElement token) {
            if(token.Name == RegisterKeys.Map) {
                Result result = null;
                var initResult = InitListParser(token);
                var mapResult = MapRuleListParser(token);
                if(initResult.IsSuccessed && mapResult.IsSuccessed) {
                    result = MapAction(token, initResult, mapResult);
                } else if(initResult.IsSuccessed) {
                    result = new Result(token, true, initResult.Expression);
                } else if(mapResult.IsSuccessed) {
                    result = new Result(token, true, mapResult.Expression);
                } else {
                    result = new Result(token, false, null);
                }
                return result;
            } else {
                return new Result(token, false, null);
            }
        }
        public Result InitListParser(XElement token) {
            List<Result> results = new List<Result>();
            foreach(var t in token.Elements()) {
                results.Add(InitParser(t));
            }
            return InitAction(token, results);
        }
        public Result InitParser(XElement token) {
            var name = token.Attribute("Type").Value;
            if(token.Name == RegisterKeys.Rule) {
                string ruleType = (string)ctx.Items[name];
                Type theType = Type.GetType(ruleType);
                var method = theType.GetMethod("Execute");
                Type source = GetType(method.GetParameters()[0].ParameterType);
                var invoker = (IInvoker)Utilities.CreateType(typeof(RuleInvoker<>), source)
                    .CreateInstance();
                return new Result(token, true, invoker.Invoke(theType), source);
            }
            return new Result(token, false, null);
        }
        public Result MapRuleListParser(XElement token) {
            List<Result> results = new List<Result>();
            foreach(var t in token.Elements()) {
                results.Add(MapRuleParser(t));
            }
            return MapRuleAction(token, results);
        } 
        public Result MapRuleParser(XElement token) {
            var name = token.Attribute("Type").Value;
            if(token.Name == RegisterKeys.MapRule) {
                string ruleType = (string)ctx.Items[name];
                Type theType = Type.GetType(ruleType);
                var method = theType.GetMethod("Execute");
                Type source = GetType(method.GetParameters()[0].ParameterType);
                Type target = GetType(method.ReturnType);
                var invoker = (IInvoker)Utilities.CreateType(typeof(MapRuleInvoker<, >), source, target)
                    .CreateInstance();
                return new Result(token, true, invoker.Invoke(theType), source, target);
            }
            return new Result(token, false, null);
        }
        private Result MapRuleAction(XElement token, IEnumerable<Result> results) {
            if(!results.Where(r => r.IsSuccessed).Any()) return new Result(token, false, null);
            Type source = results.ElementAt(0).SourceType;
            Type target = results.ElementAt(0).TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(MapGroupInvoker<,>), source, target)
                           .CreateInstance();
            var expr = invoker.Invoke(results.Select(r => r.Expression));
            return new Result(token, true, expr);
        }
        private Result MapAction(XElement token, Result init, Result map) {
            return null;
        }
        private Result InitAction(XElement token, IEnumerable<Result> results) {
            if(!results.Where(r => r.IsSuccessed).Any()) return new Result(token, false, null);
            Type source = results.ElementAt(0).SourceType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(RuleGroupInvoker<>), source)
                           .CreateInstance();
            var expr = invoker.Invoke(results.Select(r => r.Expression));
            return new Result(token, true, expr);
        }
        public Result ReduceParser(XElement token) {
            if(token.Name == RegisterKeys.Reduce) {
                List<Result> results = new List<Result>();
                foreach(var t in token.Elements()) {
                    results.Add(ReduceRuleParser(t));
                }
                return ReduceAction(token, results.Where(r => r.IsSuccessed));
            } else {
                return new Result(token, false, null);
            }
        }
        private Result ReduceAction(XElement token, IEnumerable<Result> results) {
            if(!results.Any()) return new Result(token, false, null);
            Type source = results.ElementAt(0).SourceType;
            Type target = results.ElementAt(0).TargetType;
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(ReduceInvoker<,>), source, target)
                           .CreateInstance();
            var expr = invoker.Invoke(results.Select(r => r.Expression));
            return new Result(token, true, expr);
        }
        public Result ReduceRuleParser(XElement token) {
            var name = token.Attribute("Type").Value;
            if(token.Name == RegisterKeys.ReduceRule) {
                string ruleType = (string)ctx.Items[name];
                Type theType = Type.GetType(ruleType);
                var method = theType.GetMethod("Execute");
                Type source = GetType(method.GetParameters()[0].ParameterType);
                Type target = GetType(method.ReturnType);
                LambdaExpression expression = ReduceRule(theType, source, target);
                return new Result(token, true, expression, source, target);
            }
            return new Result(token, false, null);
        }

        public LambdaExpression ReduceRule(Type ruleType, Type sourceType, Type targetType) {
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceRuleInvoker<, >), sourceType, targetType)
                            .CreateInstance();
            return invoker.Invoke(ruleType);
        }

        private Type GetType(Type theType) {
            Type result = theType;
            if("IEnumerable`1" == theType.Name) {
                result = theType.GenericTypeArguments[0];
            }
            return result;
        }
    }
}
