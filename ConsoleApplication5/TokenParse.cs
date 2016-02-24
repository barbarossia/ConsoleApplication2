using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApplication5 {
    public class TokenParse {
        private XElement _inputElement;
        public TokenParse(XElement inputElement) {
            _inputElement = inputElement;
        }
        public Token Parse(Context ctx) {
            if(IsReference(_inputElement)) {
                return CreateReferenceToken(ctx);
            } else if(IsRuleToken(_inputElement)) {
                return CreateRuleToken(ctx);
            } else if(IsMapRuleToken(_inputElement)) {
                return CreateMapRuleToken(ctx);
            } else if(IsReduceRuleToken(_inputElement)) {
                return CreateReduceRuleToken(ctx);
            } else if(IsMapReduceToken(_inputElement)) {
                return CreateMapReduceToken(ctx);
            } else if(IsMapToken(_inputElement)) {
                return CreateMapToken(ctx);
            } else if(IsReduceToken(_inputElement)) {
                return CreateReduceToken(ctx);
            } else if(IsForEach(_inputElement)) {
                return CreateForEachToken(ctx);
            } else if(IsIf(_inputElement)) {
                return CreateIfElseToken(ctx);
            } else if(IsIfCondition(_inputElement)) {
                return CreateConditionToken(ctx);
            } else if(IsIfThen(_inputElement)) {
                return CreateIfThenToken(ctx);
            } else if(IsElse(_inputElement)) {
                return CreateElseToken(ctx);
            } else {
                throw new Exception("");
            }
        }

        private RuleToken CreateRuleToken(Context ctx) {
            var name = _inputElement.Attribute("Type").Value;
            return new RuleToken(name, ctx);
        }
        private Token CreateReferenceToken(Context ctx) {
            string refname = _inputElement.Attribute("ref").Value;
            return (Token)ctx.Items[refname];
        }
        private MapRuleToken CreateMapRuleToken(Context ctx) {
            var name = _inputElement.Attribute("Type").Value;
            return new MapRuleToken(name, ctx);
        }
        private ReduceRuleToken CreateReduceRuleToken(Context ctx) {
            var name = _inputElement.Attribute("Type").Value;
            return new ReduceRuleToken(name, ctx);
        }
        private MapToken CreateMapToken(Context ctx) {
            var result = new MapToken(_inputElement
                .Elements()
                .Select(el => new TokenParse(el).Parse(ctx)));
            return result;
        }
        private ReduceToken CreateReduceToken(Context ctx) {
            return new ReduceToken(_inputElement
                .Elements()
                .Select(el => new TokenParse(el).Parse(ctx)));
        }
        private MapReduceToken CreateMapReduceToken(Context ctx) {
            XElement mapElement = _inputElement.Elements().ElementAt(0);
            TokenParse parse1 = new TokenParse(mapElement);
            MapToken mapPart = (MapToken)parse1.Parse(ctx);

            XElement reduceElement = _inputElement.Elements().ElementAt(1);
            TokenParse parse2 = new TokenParse(reduceElement);
            ReduceToken reducePart = (ReduceToken)parse2.Parse(ctx);
            var result = new MapReduceToken(mapPart, reducePart);
            if(null != _inputElement.Attribute("name")) {
                string name = _inputElement.Attribute("name").Value;
                ctx.Items.Add(name, result);
            }
            return result;

        }
        private ForEachToken CreateForEachToken(Context ctx) {
            XElement bodyElement = _inputElement.Elements().SingleOrDefault();
            TokenParse parse = new TokenParse(bodyElement);
            Token body = parse.Parse(ctx);
            return new ForEachToken(body);
        }

        private ConditionToken CreateConditionToken(Context ctx) {
            string name = _inputElement.Attribute("Type").Value;
            return new ConditionToken(name, ctx);
        }
        private Token CreateIfElseToken(Context ctx) {
            XElement conditionElement = _inputElement.Elements().ElementAt(0);
            TokenParse conditionparse = new TokenParse(conditionElement);
            ConditionToken condition = (ConditionToken)conditionparse.Parse(ctx);
            bool canContinue = condition.Condition.IsQualified();
            if (canContinue) {
                XElement ifElement = _inputElement.Elements().ElementAt(1);
                TokenParse ifParse = new TokenParse(ifElement);
                return ifParse.Parse(ctx);
            } else if (_inputElement.Elements().Count() == 3) {
                XElement elseElement = _inputElement.Elements().ElementAt(2);
                TokenParse elseParse = new TokenParse(elseElement);
                return  elseParse.Parse(ctx);
            }
            return null;
        }

        private Token CreateIfThenToken(Context ctx) {
            XElement bodyElement = _inputElement.Elements().SingleOrDefault();
            TokenParse parse = new TokenParse(bodyElement);
            return parse.Parse(ctx);
        }

        private Token CreateElseToken(Context ctx) {
            XElement bodyElement = _inputElement.Elements().SingleOrDefault();
            TokenParse parse = new TokenParse(bodyElement);
            return parse.Parse(ctx);
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
