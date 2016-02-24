using ClassLibrary1;
using MapReduce.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class TokenInfo {
        public string Name { get; set; }
        public XElement Image { get; set; }
        public Type RuleType { get; set; }
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public TokenInfo(string name, XElement image, Type ruleType, Type source, Type target) {
            Name = name;
            Image = image;
            RuleType = ruleType;
            SourceType = source;
            TargetType = target;
        }
        public TokenInfo(string name, Type source, Type target) {
            Name = name;
            SourceType = source;
            TargetType = target;
        }

        public static TokenInfo Create(XElement input, Context context) {
            if(input.Attribute("Condition") != null) return CreateQulificationRule(input, context);
            return CreateRule(input, context);
        }
        private static TokenInfo CreateQulificationRule(XElement input, Context context) {
            var typeName = input.Attribute("Condition").Value;
            Type ruleType = context.Get<Type>(typeName);
            if(ruleType == null) {
                ruleType = Type.GetType(typeName);
                context.Set(typeName, ruleType);
            }
            var rule = (IQulification)ruleType.CreateInstance();
            if(rule.IsQualified()) {
                return CreateRule(input, context);
            } else {
                return null;
            }
        }
        private static TokenInfo CreateRule(XElement input, Context context) {
            var typeName = input.Attribute("Type").Value;
            Type ruleType = context.Get<Type>(typeName);
            if(ruleType == null) {
                ruleType = Type.GetType(typeName);
                context.Set(typeName, ruleType);
            }
            var method = ruleType.GetMethod("Execute");
            Type source = GetType(method.GetParameters()[0].ParameterType);
            Type target = GetType(method.ReturnType);
            return new TokenInfo(input.Name.LocalName, input, ruleType, source, target);
        }
        private static Type GetType(Type theType) {
            Type result = theType;
            if("IEnumerable`1" == theType.Name) {
                result = theType.GenericTypeArguments[0];
            }
            return result;
        }
    }
}
