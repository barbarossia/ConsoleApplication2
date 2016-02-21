using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class TokenInfo {
        public TokenInfo Next { get; set; }
        public string Name { get; set; }
        public XElement Image { get; set; }
        public Type RuleType { get; set; }
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
    }
}
