﻿using ClassLibrary1;
using MapReduce.Lexer;
using MapReduce.Parser.Invokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public partial class ParserResult {
        public bool IsSuccessed { get; set; }
        public LambdaExpression Expression { get; set; }
        public TokenInfo Token { get; set; }
        public SyntaxException Error { get; set; }
        private const string METHOD = "Execute";
        private const string REFERNCENAME = "ref";
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
        public static ParserResult CreateRule(Token token, Context context) {
            XElement input = token.Value;
            if (input.Attribute(REFERNCENAME) != null) {
                var resultName = input.Attribute(REFERNCENAME).Value;
                var result = context.Get<ParserResult>(resultName);
                if(result == null) throw new SyntaxException(token, string.Format("cannot find the refenerce: {0}", resultName));
                return result;
            }
            TokenInfo info = TokenInfo.Create(input, context);
            if(info != null) {
                if (token.TokenType == TokenType.RULE) {
                    return CreateRule(info);
                }else if (token.TokenType == TokenType.MAPRULE) {
                    return CreateMapRule(info);
                } else if (token.TokenType == TokenType.REDUCERULE) {
                    return CreateReduceRule(info);
                } else {
                    throw new SyntaxException(token, string.Format("error on create parse result, cannot find the rule: {0}", info.Image));
                }
            } else {
                return null;
            }
        }

        private static ParserResult CreateRule(TokenInfo info) {
            Type theType = info.RuleType;
            var method = theType.GetMethod(METHOD);
            var invoker = (IInvoker)Utilities.CreateType(typeof(RuleInvoker<>), info.SourceType)
                .CreateInstance(theType);
            return new ParserResult(info, true, invoker.Invoke());
        }
        private static ParserResult CreateMapRule(TokenInfo info) {
            Type theType = info.RuleType;
            var method = theType.GetMethod(METHOD);
            var invoker = (IInvoker)Utilities.CreateType(typeof(MapRuleInvoker<,>), info.SourceType, info.TargetType)
               .CreateInstance(theType);
            return new ParserResult(info, true, invoker.Invoke());
        }
        private static ParserResult CreateReduceRule(TokenInfo info) {
            Type theType = info.RuleType;
            var method = theType.GetMethod(METHOD);
            var invoker = (IInvoker)Utilities.CreateType(typeof(ReduceRuleInvoker<,>), info.SourceType, info.TargetType)
                           .CreateInstance(theType);
            return new ParserResult(info, true, invoker.Invoke());
        }
    }
}
