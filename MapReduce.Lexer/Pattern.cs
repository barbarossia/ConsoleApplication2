using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class Pattern {
        public string Regex { get; private set; }
        public TokenType TokenType { get; private set; }
        public Pattern(string regex, TokenType tokenType) {
            Regex = regex;
            TokenType = tokenType;
        }

        public bool IsMatch(XElement matcher) {
            return matcher.Name == Regex;
        }
    }
}
