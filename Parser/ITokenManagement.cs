using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public interface ITokenManagement {
        TokenInfo GetNextToken(TokenInfo current);
        TokenInfo Root { get; }
    }

    public class TokenManagement : ITokenManagement {
        private XElement _root;
        private Context ctx;
        public TokenManagement(XElement root, Context context) {
            _root = root;
            ctx = context;
        }
        public TokenInfo Root { get { return CreateSequnce(_root); } }
        public TokenInfo GetNextToken(TokenInfo current) {
            TokenInfo result = null;
            if(current == null) return null;
            XElement _current = current.Image;
            if(_current == null) return null;
            if(IsMapReduceToken(_current)) {
                _root = _current;
                if(_current.Elements().Any()) {
                    _current = _current.Elements().First();
                    result = CreateSequnce(_current);
                } else {
                    return null;
                }
            } else if(IsReduceToken(_current) || IsMapToken(_current) ) {
                _root = _current;
                if(_current.Elements().Any()) {
                    _current = _current.Elements().First();
                    result = Create(_current);
                } else {
                    _current = (XElement)_current.NextNode;
                    result = CreateSequnce(_current);
                }
            } else if(IsReduceRuleToken(_current) || IsRuleToken(_current) || IsMapRuleToken(_current)) {
                if(_current.NextNode != null) {
                    _current = (XElement)_current.NextNode;
                    result = Create(_current);
                } else {
                    _current = (XElement)_root.NextNode;
                    result = CreateSequnce(_current);
                }
            } 
            return result;
        }
        private TokenInfo CreateSequnce(XElement input) {
            if(input == null) return null;
            return new TokenInfo() { Name = input.Name.LocalName, Image = input };
        }

        private TokenInfo Create(XElement input) {
            var name = input.Attribute("Type").Value;
            string ruleType = (string)ctx.Items[name];
            Type theType = Type.GetType(ruleType);
            var method = theType.GetMethod("Execute");
            Type source = GetType(method.GetParameters()[0].ParameterType);
            Type target = GetType(method.ReturnType);
            return new TokenInfo() { RuleType = theType, Name = input.Name.LocalName, SourceType = source, TargetType = target, Image = input};
        }
        private Type GetType(Type theType) {
            Type result = theType;
            if("IEnumerable`1" == theType.Name) {
                result = theType.GenericTypeArguments[0];
            }
            return result;
        }

        public bool IsRuleToken(XElement node) {
            return node.Name == "Rule";
        }
        public bool IsReference(XElement node) {
            return node.Attribute("ref") != null;
        }
        public bool IsMapRuleToken(XElement node) {
            return node.Name == "MapRule";
        }
        public bool IsReduceRuleToken(XElement node) {
            return node.Name == "ReduceRule";
        }
        public bool IsMapReduceToken(XElement node) {
            return node.Name == "MapReduce";
        }
        public bool IsMapToken(XElement node) {
            return node.Name == "Map";
        }
        public bool IsReduceToken(XElement node) {
            return node.Name == "Reduce";
        }
        public bool IsIf(XElement node) {
            return node.Name == "If";
        }
        public bool IsIfCondition(XElement node) {
            return node.Name == "If.Condition";
        }
        public bool IsIfThen(XElement node) {
            return node.Name == "If.Then";
        }
        public bool IsElse(XElement node) {
            return node.Name == "If.Else";
        }
        public bool IsForEach(XElement node) {
            return node.Name == "ForEach";
        }
    }
}
