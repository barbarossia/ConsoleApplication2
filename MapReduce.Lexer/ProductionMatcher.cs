using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public class ProductionMatcher : MatcherBase {
        public ProductionMatcher(Pattern pattern) : base(pattern) {
        }

        public override void Match(Tokenizer tokenizer) {
            tokenizer.Results.Add(new Token(_pattern.TokenType, tokenizer.Source));
            MatchInside(tokenizer);
            tokenizer.Results.Add(new Token(TokenType.EOF));
        }
        private void MatchInside(Tokenizer tokenizer) {
            var list = tokenizer.Source.Elements();
            foreach(var node in list) {
                var t = new Tokenizer(node, tokenizer.Results, tokenizer.Matchers.ToArray());
                t.Parse();
            }
        }
    }
}
