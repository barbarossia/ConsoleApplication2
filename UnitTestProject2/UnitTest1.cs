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
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestReduce() {
            string xml = @"
                    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1' />
        <ReduceRule Type = 'AssignRuleOnT1' />
    </Reduce>";
            Parser parser = xml.CreateParser("Reduce");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            Assert.IsTrue(parser.ReduceBlock());
            PrivateObject po = new PrivateObject(parser);
            ParserResult parserResult = (ParserResult)po.GetField("currentReduceResult");
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
                <Reduce></Reduce>";
            Parser parser = xml.CreateParser("Reduce");
            Assert.IsFalse(parser.ReduceBlock());
            Assert.IsInstanceOfType(parser.Result.Error, typeof(SyntaxException));
            Assert.AreEqual("Reduce has encurred the error!", parser.Result.Error.Message);
            Assert.AreEqual(xml.Trim(), parser.Result.Error.Source.Value.ToString());
        }
        [TestMethod]
        public void TestReduceWithNotExpected() {
            string xml = @"<Reduce>
                <Rule Type = 'IninValueOnT2' />
            </Reduce>";
            Parser parser = xml.CreateParser("Reduce");
            parser.AddContext("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            Assert.IsFalse(parser.ReduceBlock());
            Assert.IsInstanceOfType(parser.Result.Error, typeof(SyntaxException));
            Assert.AreEqual("Reduce has encurred the error!", parser.Result.Error.Message);
        }

        [TestMethod]
        public void TestInit() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");

            Assert.IsFalse(parser.MapBlock());
            Assert.IsInstanceOfType(parser.Result.Error, typeof(SyntaxException));
            Assert.AreEqual("Map has encurred the error!", parser.Result.Error.Message);
        }

        [TestMethod]
        public void TestInitGroup() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <Rule Type = 'IninValueOnT2Add' />
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            parser.AddContext("IninValueOnT2Add", "UnitTestProject2.IninValueOnT2Add, UnitTestProject2");
            Assert.IsFalse(parser.MapBlock());
            Assert.IsInstanceOfType(parser.Result.Error, typeof(SyntaxException));
            Assert.AreEqual("Map has encurred the error!", parser.Result.Error.Message);
        }

        [TestMethod]
        public void TestMap() {
            string xml = @"
                <Map>
                    <MapRule Type = 'MapRuleOnT2' />
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");

            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test2, IEnumerable<Test3>>>)parserResult;

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
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            parser.AddContext("MapRuleOnT2Add", "UnitTestProject2.MapRuleOnT2Add, UnitTestProject2");

            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test2, IEnumerable<Test3>>>)parserResult;

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
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");

            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, IEnumerable<Test2>>>)parserResult;

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
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");

            Assert.IsFalse(parser.MapBlock());
            Assert.IsInstanceOfType(parser.Result.Error, typeof(SyntaxException));
            Assert.AreEqual("Map has encurred the error!", parser.Result.Error.Message);
            ;
        }
        [TestMethod]
        public void TestMapReduce() {
            string xml = @"
            <MapReduce>
                <Map>
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                </Map>
             <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT1' />
                    <ReduceRule Type = 'AssignRuleOnT1' />
                </Reduce>
            </MapReduce>";
            Parser parser = xml.CreateParser("MapReduce");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");

            Assert.IsTrue(parser.Build());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, Test1>>)parserResult;

            Func<Test1, Test1> func = resultFunc.Compile();

            var t1 = new Test1() { A = 10 };
            Test1 result = func(t1);
            Assert.AreEqual(10, result.Details.Count());
        }
    }
}
