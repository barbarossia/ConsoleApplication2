using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class Lexer {
        private XElement _source;
        private List<IMatcher> InitializeMatchList() {
            return new List<IMatcher>() {
            new TerminalRuleMatcher(Patterns.Rule),
            new TerminalRuleMatcher(Patterns.MapRule),
            new TerminalRuleMatcher(Patterns.ReduceRule),
            new ProductionMatcher(Patterns.Map),
            new ProductionMatcher(Patterns.Reduce),
            new ProductionMatcher(Patterns.MapReduce),
            new ProductionMatcher(Patterns.ForEach)
            };

        }
        public IEnumerable<Token> Lex() {
            List < IMatcher > matchers = InitializeMatchList();
            List<Token> results = new List<Token>();
            Tokenizer tokenizer = new Tokenizer(_source, results, matchers.ToArray());
            tokenizer.Parse();
            return results;
        }
        public Lexer(XElement source) {
            _source = source;
        }
    }
}
