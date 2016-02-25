using MapReduce.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class SyntaxException : ApplicationException {
        public Token Source { get; private set; }
        public SyntaxException(string message) : base(message) {
        }
        public SyntaxException(Token source, string message):this(message) {
            Source = source;
        }
    }

}
