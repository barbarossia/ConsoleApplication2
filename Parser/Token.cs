using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parser {
    public class Token {
        public Token Next { get; set; }
        public string Name { get; set; }
        public XElement Image { get; set; }
        public Type RuleType { get; set; }
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
    }
}
