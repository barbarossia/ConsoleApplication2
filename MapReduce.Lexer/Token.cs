using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class Token {
        public TokenType TokenType { get; set; }
        public XElement Value { get; set; }
        public Token(TokenType tokenType, XElement value) {
            TokenType = tokenType;
            Value = value;
        }
        public Token(TokenType tokenType) {
            TokenType = tokenType;
        }
    }
}
