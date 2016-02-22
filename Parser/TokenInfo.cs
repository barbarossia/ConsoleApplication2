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
    }
}
