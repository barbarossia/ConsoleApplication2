using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApplication3 {
    public class SequenceNodeParse : NodeParse {
        public SequenceNodeParse(XElement input) : base(input) {
        }
        public override Token Parse() {
            List<Token> list = new List<Token>();
            foreach(var element in m_inputString.Elements()) {
                list.Add(ParseFactory.CreateNodeParse(element).Parse());
            }

            return new SequenceToken(list);
        }
    }

    public class IfNodeParse : NodeParse {
        public IfNodeParse(XElement input) : base(input) {
        }

        public override Token Parse() {
            XElement coditionElement = m_inputString.Element("If.Condition");
            ConditionNode cond = CreateCondetionNode(coditionElement);
            XElement trueElement = m_inputString.Element("If.Then");
            XElement falseElement = m_inputString.Element("If.Else");
            Token truePart = ParseFactory.CreateNodeParse(trueElement.Elements().SingleOrDefault()).Parse();
            Token falsePart = null;
            if(falseElement != null) {
                falsePart = ParseFactory.CreateNodeParse(falseElement.Elements().SingleOrDefault()).Parse();
            }
            return new IfToken(cond, truePart, falsePart);
        }

        private ConditionNode CreateCondetionNode(XElement element) {
            return new ConditionNode() { Condition = element.Attribute("Name").Value };
        }
    }

    public class ForEachNodeParse : NodeParse {
        public ForEachNodeParse(XElement input) : base(input) {
        }

        public override Token Parse() {
            XElement actionElement = m_inputString.Elements().SingleOrDefault();
            var result = ParseFactory.CreateNodeParse(actionElement).Parse();
           return new ForEachToken(result);
        }
    }

    public class RuleNodeParse : NodeParse {
        public RuleNodeParse(XElement input) : base(input) {
        }
        public override Token Parse() {
            if(m_inputString.IsRule())
                return CreateRuleNode(m_inputString);
            return null;
        }

        private RuleToken CreateRuleNode(XElement node) {
            var name = node.Attribute("Name").Value;
            Type type = Type.GetType(node.Attribute("SourceType").Value);
            return new RuleToken(name, type);
        }
    }

    public class MapRuleNodeParse : NodeParse {
        public MapRuleNodeParse(XElement input) : base(input) {
        }
        public override Token Parse() {
            if(m_inputString.IsMapRule()) {
                return CreateRuleNode(m_inputString);
            }
                
            return null;
        }

        private MapRuleToken CreateRuleNode(XElement node) {
            var name = node.Attribute("Name").Value;
            Type s = Type.GetType(node.Attribute("SourceType").Value);
            Type t = Type.GetType(node.Attribute("TargetType").Value);
            return new MapRuleToken(name, s, t);
        }
    }

    public class ReduceRuleNodeParse : NodeParse {
        public ReduceRuleNodeParse(XElement input) : base(input) {
        }
        public override Token Parse() {
            if(m_inputString.IsReduceRule())
                return CreateRuleNode(m_inputString);
            return null;
        }

        private ReduceRuleToken CreateRuleNode(XElement node) {
            var name = node.Attribute("Name").Value;
            Type s = Type.GetType(node.Attribute("SourceType").Value);
            Type t = Type.GetType(node.Attribute("TargetType").Value);
            return new ReduceRuleToken(name, s, t);
        }
    }
}
