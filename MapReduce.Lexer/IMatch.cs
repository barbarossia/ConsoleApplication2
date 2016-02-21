using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Lexer {
    public interface IMatcher {
        bool IsMatch(Tokenizer tokenizer);
        void Match(Tokenizer tokenizer);
    }
}
