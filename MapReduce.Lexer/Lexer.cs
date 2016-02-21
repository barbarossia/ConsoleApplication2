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
            List<IMatcher> matchers = new List<IMatcher>();
            IMatcher rule = new TerminalRuleMatcher(new Pattern("Rule", TokenType.RULE));
            IMatcher mapRule = new TerminalRuleMatcher(new Pattern("MapRule", TokenType.MAPRULE));
            IMatcher reduceRule = new TerminalRuleMatcher(new Pattern("ReduceRule", TokenType.REDUCERULE));
            IMatcher map = new ProductionMatcher(new Pattern("Map", TokenType.MAP));
            IMatcher reduce = new ProductionMatcher(new Pattern("Reduce", TokenType.REDUCE));
            IMatcher mapReduce = new ProductionMatcher(new Pattern("MapReduce", TokenType.MAPREDUCE));

            matchers.Add(rule);
            matchers.Add(mapRule);
            matchers.Add(reduceRule);
            matchers.Add(map);
            matchers.Add(reduce);
            matchers.Add(mapReduce);
            return matchers;
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
