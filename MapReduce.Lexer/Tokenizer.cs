using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class Tokenizer { 
        private XElement _source;
        private List<Token> _results;
        private List<IMatcher> _matchers;
        public List<Token> Results { get { return _results; } }
        public XElement Source { get { return _source; } }
        public List<IMatcher> Matchers { get { return _matchers; } }
        public Tokenizer(XElement source, List<Token> results, params IMatcher[] matchers) {
            _source = source;
            _results = results;
            _matchers = new List<IMatcher>(matchers);
        }

        public void Parse() {
            foreach(var matcher in _matchers) {
                if (matcher.IsMatch(this)) {
                    matcher.Match(this);
                }
            }
        }
    }
}
