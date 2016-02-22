using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class Pattern {
        public Regex Regex { get; private set; }
        public TokenType TokenType { get; private set; }
        public Pattern(string regex, TokenType tokenType) {
            Regex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            TokenType = tokenType;
        }

        public bool IsMatch(XElement matcher) {
            return Regex.IsMatch(matcher.Name.LocalName);
        }
    }
}
