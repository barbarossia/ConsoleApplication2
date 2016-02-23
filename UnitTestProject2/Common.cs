﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using MapReduce.Parser;
using System.Linq.Expressions;
using MapReduce.Lexer;

namespace UnitTestProject2 {
    public static class Common {
        public static Parser CreateParser(this string xml, string block) {
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element(block);
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Context ctx = new Context(buffer);

            Parser parser = new Parser(ctx);
            return parser;
        }
        public static void AddContext(this Parser parser, string key, object value) {
            parser.Context.Items.Add(key, value);
        }
    }
}