using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class TerminalRuleMatcher : MatcherBase {
        public TerminalRuleMatcher(Pattern pattern) : base(pattern) {
        }

        public override void Match(Tokenizer tokenizer) {
            tokenizer.Results.Add(new Token(_pattern.TokenType, tokenizer.Source));
        }
    }
}
