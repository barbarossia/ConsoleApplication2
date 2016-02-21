using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public abstract class MatcherBase : IMatcher {
        protected Pattern _pattern;
        public MatcherBase(Pattern pattern) {
            _pattern = pattern;
        }

        public bool IsMatch(Tokenizer tokenizer) {
            return tokenizer != null && _pattern.IsMatch(tokenizer.Source);
        }

        public abstract void Match(Tokenizer tokenizer);
    }
}
