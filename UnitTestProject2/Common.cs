using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using MapReduce.Parser;
using System.Linq.Expressions;
using MapReduce.Lexer;

namespace MapReduce.Parser.UnitTest {
    public static class Common {
        public static Parser CreateParser(this string xml, string block) {
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element(block);
            var lexer = new MapReduce.Lexer.Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            MapReduce.Parser.Context ctx = new MapReduce.Parser.Context(buffer);

            Parser parser = new Parser(ctx);
            return parser;
        }
        public static void AddContext(this Parser parser, string key, string value) {
            Type ruleType = Type.GetType(value);
            parser.Context.Set(key, ruleType);
        }
        public static void AddResult(this Parser parser, string key, ParserResult value) {
            parser.Context.Set(key, value);
        }
    }
}
