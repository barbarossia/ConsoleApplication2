using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApplication3 {
    public abstract class NodeParse {
        protected XElement m_inputString;
        public NodeParse(XElement input) {
            m_inputString = input;
        }
        public abstract Token Parse();
    }

    public class ParseFactory {
        public static NodeParse CreateNodeParse(XElement input) {
            if(input.IsRule())
                return new RuleNodeParse(input);
            if(input.IsMapRule())
                return new MapRuleNodeParse(input);
            if(input.IsReduceRule())
                return new ReduceRuleNodeParse(input);
            else if(input.IsSequence())
                return new SequenceNodeParse(input);
            else if(input.IsIf())
                return new IfNodeParse(input);
            else if(input.IsForEach())
                return new ForEachNodeParse(input);
            else
                throw new NotImplementedException();
        }
    }
}
