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
        private TokenBuffer tokenBuffer;
        private Context context;
        private ParserResult currentResult;
        private Stack<ParserResult> innerResults = new Stack<ParserResult>();
        private Stack<ParserResult> innerReduceResults = new Stack<ParserResult>();
        public ParserResult Result { get { return currentResult; } }
        private ParserResult currentReduceResult;
        private int level = 0;
        public Context Context { get { return context; } }
        public Parser(Context ctx) {
            context = ctx;
            tokenBuffer = ctx.TokenBuffer;
        }
        private void Action(ParserResult other) {
            if(innerResults.Count <= level) {
                innerResults.Push(other);
            } else {
                currentResult = innerResults.Pop();
                currentResult = currentResult.Concat(other);
                innerResults.Push(currentResult);
            }
            currentResult = innerResults.Peek();
        }
        private void MapReduceAction() {
            currentResult = innerResults.Pop();
            currentReduceResult = innerReduceResults.Pop();
            if(currentResult != null && currentReduceResult != null &&
                currentResult.Token.Name == RegisterKeys.MapRule &&
                currentReduceResult.Token.Name == RegisterKeys.ReduceRule) {
                currentResult = currentResult.Concat(currentReduceResult);
                innerResults.Push(currentResult);
            }
        }
        private void ReduceAction(ParserResult other) {
            if((currentReduceResult == null || innerReduceResults.Count == 0) && 
                innerReduceResults.Count <= level) {
                innerReduceResults.Push(other);
            } else {
                currentReduceResult = innerReduceResults.Pop();
                currentReduceResult = currentReduceResult.Concat(other);
                innerReduceResults.Push(currentReduceResult);
            }
            currentReduceResult = innerReduceResults.Peek();
        }
        private void ForEachAction() {
            ParserResult tmpResult = innerResults.Pop();
            Type source = tmpResult.Token.SourceType;
            Type target = tmpResult.Token.TargetType;
            if (tmpResult.Token.Name == RegisterKeys.MapRule) {
                var gTarget = typeof(IEnumerable<>);
                var mm = gTarget.MakeGenericType(target);
                target = mm;
            }
            var invoker = (IGroupInvoker)Utilities.CreateType(typeof(ForEachInvoker<,>), source, target)
                           .CreateInstance(tmpResult.Expression);
            var expr = invoker.Invoke();
            var token = new TokenInfo(RegisterKeys.ForEach, source, target);
            var forEachResult = new ParserResult(token, true, expr);
            level--;
            Action(forEachResult);
        }

        public bool Execute() {
            return MapReduceBlock();
        }
        private bool MapReduceBlock() {
            Token t;
            bool parseSuccess = false;
            tokenBuffer.Backup();
            t = tokenBuffer.Current;
            if(t != null && t.TokenType == TokenType.MAPREDUCE) {
                tokenBuffer.Consume();
                if(MapBlock()) {
                    if(ReduceBlock()) {
                        parseSuccess = true;
                        MapReduceAction();
                    }
                }
                if(parseSuccess) {
                    t = tokenBuffer.Current;
                    if(t != null && t.TokenType == TokenType.EOF) {
                        tokenBuffer.Consume();
                        tokenBuffer.Commit();
                    } else {
                        currentResult = new ParserResult(false, new SyntaxException("MapReduce section should have end"));
                    }
                }
            }
            if(!parseSuccess) {
                tokenBuffer.Rollback();
                currentResult = new ParserResult(false, new SyntaxException("Map node must contains more than one element!"));
            }
            return parseSuccess;
        }
        public bool MapBlock() {
            Token t;
            bool parseSuccess = false;
            tokenBuffer.Backup();
            t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.MAP) {
                tokenBuffer.Consume();
                parseSuccess = true;
                OptionalRuleListParser();
                parseSuccess = MapRuleListParser();
                ForEachBlock();
                if (parseSuccess) {
                    t = tokenBuffer.Current;
                    if (t != null && t.TokenType == TokenType.EOF) {
                        tokenBuffer.Consume();
                        tokenBuffer.Commit();
                    } else {
                        parseSuccess = false;
                        currentResult = new ParserResult(false, new SyntaxException("Map section should have end"));
                    }
                }
            }
            if (!parseSuccess) {
                tokenBuffer.Rollback();
            }
            return parseSuccess;
        }
        private bool ForEachBlock() {
            Token t;
            bool parseSuccess = false;
            tokenBuffer.Backup();
            t = tokenBuffer.Current;
            if(t != null && t.TokenType == TokenType.FOREACH) {
                tokenBuffer.Consume();
                level++;
                parseSuccess = SelectionForEachListParser();
                if(parseSuccess) {
                    t = tokenBuffer.Current;
                    if(t != null && t.TokenType == TokenType.EOF) {
                        tokenBuffer.Consume();
                        tokenBuffer.Commit();
                        ForEachAction();
                    } else {
                        parseSuccess = false;
                        currentResult = new ParserResult(false, new SyntaxException("Foreach section should have end"));
                    }
                }
            }
            if(!parseSuccess) {
                tokenBuffer.Rollback();
            }
            return parseSuccess;
        }
        private bool SelectionForEachListParser() {
            tokenBuffer.Backup();
            bool parseSuccess = false;
            if(RuleParser()) {
                parseSuccess = true;
            } else if(MapRuleParser()) {
                parseSuccess = true;
            } else if(MapBlock()) {
                parseSuccess = true;
            } else if(MapReduceBlock()) {
                parseSuccess = true;
            } else {
                parseSuccess = false;
            }
            if(!parseSuccess) {
                tokenBuffer.Rollback();
            }
            return parseSuccess;
        }
        private bool OptionalRuleListParser() {
            bool parseSuccess;
            do {
                parseSuccess = RuleParser();
            } while(parseSuccess && tokenBuffer.Current.TokenType != TokenType.EOF);
            return true;
        }

        private bool RuleParser() {
            Token t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.RULE) {
                TokenInfo info = Create(t.Value);
                Type theType = info.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(RuleInvoker<>), info.SourceType)
                    .CreateInstance();
                Action(new ParserResult(info, true, invoker.Invoke(theType)));
                tokenBuffer.Consume();
                return true;
            }
            return false;
        }
        private bool MapRuleListParser() {
            tokenBuffer.Backup();
            bool parseSuccess = false;
            parseSuccess = MapRuleParser();
            if (parseSuccess) {
                while (parseSuccess) {

                    parseSuccess = MapRuleParser();
                }
                parseSuccess = true;
            }
            if (!parseSuccess) {
                currentResult = new ParserResult(false, new SyntaxException("Map section must contains one or more elements!"));
            }
            return parseSuccess;
        } 
        private bool MapRuleParser() {
            Token t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.MAPRULE) {
                TokenInfo info = Create(t.Value);
                Type theType = info.RuleType;
                var method = theType.GetMethod("Execute");
                var invoker = (IInvoker)Utilities.CreateType(typeof(MapRuleInvoker<,>), info.SourceType, info.TargetType)
                   .CreateInstance();
                Action(new ParserResult(info, true, invoker.Invoke(theType)));
                tokenBuffer.Consume();
                return true;        
            }
            return false;
        }

        public bool ReduceBlock() {
            Token t;
            bool parseSuccess = false;
            tokenBuffer.Backup();
            t = tokenBuffer.Current;
            if (t != null && t.TokenType == TokenType.REDUCE) {
                tokenBuffer.Consume();
                parseSuccess = ReduceList();
                if (parseSuccess) {
                    t = tokenBuffer.Current;
                    if (t != null && t.TokenType == TokenType.EOF) {
                        tokenBuffer.Consume();
                        tokenBuffer.Commit();
                    } else {
                        parseSuccess = false;
                        currentResult = new ParserResult(false
                            , new SyntaxException("Reduce section has not End!"));
                    }
                }
            }
            if (!parseSuccess) {
                tokenBuffer.Rollback();
            }
            return parseSuccess;
        }

        private bool ReduceList() {
            tokenBuffer.Backup();
            bool parseSuccess = false;
            parseSuccess = ReduceRuleParser();
            if (parseSuccess) {
                while (parseSuccess && tokenBuffer.Current.TokenType != TokenType.EOF) {
                    parseSuccess = ReduceRuleParser();
                }
                parseSuccess = true;
            }
            if(!parseSuccess) {
                tokenBuffer.Rollback();
                currentResult = new ParserResult(false, new SyntaxException("Reduce section must contains more than one element!"));
            }
            return parseSuccess;
        }
        
        private bool ReduceRuleParser() {
            Token t = tokenBuffer.Current;
            if (t!= null && t.TokenType == TokenType.REDUCERULE) {
                TokenInfo info = Create(t.Value);
                Type theType = info.RuleType;
                var method = theType.GetMethod("Execute");
                LambdaExpression expression = ReduceRule(theType, info.SourceType, info.TargetType);
                ReduceAction(new ParserResult(info, true, expression));
                tokenBuffer.Consume();
                return true;
            }
            return false;
        }

        private LambdaExpression ReduceRule(Type ruleType, Type sourceType, Type targetType) {
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceRuleInvoker<, >), sourceType, targetType)
                            .CreateInstance();
            return invoker.Invoke(ruleType);
        }

        private TokenInfo Create(XElement input) {
            var name = input.Attribute("Type").Value;
            string ruleType = (string)context.Items[name];
            Type theType = Type.GetType(ruleType);
            var method = theType.GetMethod("Execute");
            Type source = GetType(method.GetParameters()[0].ParameterType);
            Type target = GetType(method.ReturnType);
            return new TokenInfo(input.Name.LocalName, input, theType, source, target);
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
