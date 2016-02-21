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

namespace UnitTestProject2 {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestReduce() {
            string xml = @"
                    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1' />
        <ReduceRule Type = 'AssignRuleOnT1' />
    </Reduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            XElement source = _xDoc.Element("Reduce");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.ReduceBlock();
            var resultFunc = (Expression<Func<IEnumerable<Test2>, Test1, Test1>>)parserResult.Expression;

            Func<IEnumerable<Test2>, Test1, Test1> func = resultFunc.Compile();

            var t2List = new List<Test2>() { new Test2() { Result = 1 }, new Test2() { Result = 2 }, };
            var t1 = new Test1();
            Test1 result = func(t2List, t1);
            Assert.AreEqual(3, result.Result);
            Assert.AreEqual(2, result.Details.Count());
        }
        [TestMethod]
        public void TestReduceNoItems() {
            string xml = @"
                    <Reduce>
    </Reduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            XElement source = _xDoc.Element("Reduce");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.ReduceBlock();
            Assert.IsFalse(parserResult.IsSuccessed);
            Assert.IsInstanceOfType(parserResult.Error, typeof(SyntaxException));
        }
        [TestMethod]
        public void TestReduceWithNotExpected() {
            string xml = @"
                           <Reduce>
        <Rule Type = 'IninValueOnT2' />
           </Reduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            XElement source = _xDoc.Element("Reduce");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.ReduceBlock();
            Assert.IsFalse(parserResult.IsSuccessed);
            Assert.IsInstanceOfType(parserResult.Error, typeof(SyntaxException));
        }

        [TestMethod]
        public void TestInit() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            XElement source = _xDoc.Element("Map");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.MapBlock();
            Assert.IsFalse(parserResult.IsSuccessed);
            Assert.IsInstanceOfType(parserResult.Error, typeof(SyntaxException));
        }

        [TestMethod]
        public void TestInitGroup() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <Rule Type = 'IninValueOnT2Add' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            ctx.Items.Add("IninValueOnT2Add", "UnitTestProject2.IninValueOnT2Add, UnitTestProject2");
            XElement source = _xDoc.Element("Map");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.MapBlock();
            Assert.IsFalse(parserResult.IsSuccessed);
            Assert.IsInstanceOfType(parserResult.Error, typeof(SyntaxException));
        }

        [TestMethod]
        public void TestMap() {
            string xml = @"
                <Map>
                    <MapRule Type = 'MapRuleOnT2' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            XElement source = _xDoc.Element("Map");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.MapBlock();
            var resultFunc = (Expression<Func<Test2, IEnumerable<Test3>>>)parserResult.Expression;

            Func<Test2, IEnumerable<Test3>> func = resultFunc.Compile();

            var t2 = new Test2() { B = 10 };
            IEnumerable<Test3> result = func(t2);
            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public void TestMapGroup() {
            string xml = @"
                <Map>
                    <MapRule Type = 'MapRuleOnT2' />
                    <MapRule Type = 'MapRuleOnT2Add' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT2Add", "UnitTestProject2.MapRuleOnT2Add, UnitTestProject2");
            XElement source = _xDoc.Element("Map");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.MapBlock();
            var resultFunc = (Expression<Func<Test2, IEnumerable<Test3>>>)parserResult.Expression;

            Func<Test2, IEnumerable<Test3>> func = resultFunc.Compile();

            var t2 = new Test2() { B = 10 };
            IEnumerable<Test3> result = func(t2);
            Assert.AreEqual(13, result.Count());
        }

        [TestMethod]
        public void TestInitAndMap() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");

            XElement source = _xDoc.Element("Map");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.MapBlock();
            var resultFunc = (Expression<Func<Test1, IEnumerable<Test2>>>)parserResult.Expression;

            Func<Test1, IEnumerable<Test2>> func = resultFunc.Compile();

            var t1 = new Test1() { A = 10 };
            IEnumerable<Test2> result = func(t1);
            Assert.AreEqual(10, result.Count());
        }
        [TestMethod]
        public void TestInitAndMapWithWrongSequence() {
            string xml = @"
                <Map>
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <Rule Type = 'IninValueOnT1' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");

            XElement source = _xDoc.Element("Map");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            TokenBuffer buffer = new TokenBuffer(results);
            Parser parser = new Parser(buffer, ctx);
            var parserResult = parser.MapBlock();
            Assert.IsFalse(parserResult.IsSuccessed);
            Assert.IsInstanceOfType(parserResult.Error, typeof(SyntaxException));
            ;
        }
        //[TestMethod]
        //public void TestMapReduce() {
        //    string xml = @"
        //    <MapReduce>
        //        <Map>
        //            <MapRule Type = 'MapRuleOnT1IfTrue' />
        //        </Map>
        //     <Reduce>
        //            <ReduceRule Type = 'ReduceRuleOnT1' />
        //            <ReduceRule Type = 'AssignRuleOnT1' />
        //        </Reduce>
        //    </MapReduce>";
        //    XDocument _xDoc = _xDoc = XDocument.Parse(xml);
        //    Context ctx = new Context();
        //    ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
        //    ctx.Items.Add("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
        //    ctx.Items.Add("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");

        //    ITokenManagement tokenBuffer = new TokenManagement(_xDoc.Element("MapReduce"), ctx);
        //    MapReduceParser parser = new MapReduceParser(tokenBuffer);

        //    var parserResult = parser.Execute();
        //    var resultFunc = (Expression<Func<Test1, Test1>>)parserResult.Expression;

        //    Func<Test1, Test1> func = resultFunc.Compile();

        //    var t1 = new Test1() { A = 10};
        //    Test1 result = func(t1);
        //    Assert.AreEqual(10, result.Details.Count());
        //}
    }
}
