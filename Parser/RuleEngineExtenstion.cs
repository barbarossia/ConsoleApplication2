using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public static class RuleEngineExtenstion {
        public static Parser CreateParser(this XElement block, Context ctx) {
            var lexer = new MapReduce.Lexer.Lexer(block);
            var results = lexer.Lex();
            TokenBuffer buffer = new TokenBuffer(results);
            ctx.TokenBuffer = buffer;
            Parser parser = new Parser(ctx);
            return parser;
        }

    }
}
