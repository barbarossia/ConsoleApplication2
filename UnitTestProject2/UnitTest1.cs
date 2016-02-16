using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using Parser;
using System.Linq.Expressions;

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
            MapReduceParser parser = new MapReduceParser(ctx);
            var parserResult = parser.ReduceParser(_xDoc.Element("Reduce"));
            var resultFunc = (Expression<Func<IEnumerable<Test2>, Test1, Test1>>)parserResult.Expression;

            Func<IEnumerable<Test2>, Test1, Test1> func = resultFunc.Compile();

            var t2List = new List<Test2>() { new Test2() { Result = 1 }, new Test2() { Result = 2 }, };
            var t1 = new Test1();
            Test1 result = func(t2List, t1);
            Assert.AreEqual(3, result.Result);
            Assert.AreEqual(2, result.Details.Count());
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

            MapReduceParser parser = new MapReduceParser(ctx);

            var parserResult = parser.MapParser(_xDoc.Element("Map"));
            var resultFunc = (Expression<Func<Test2, Test2>>)parserResult.Expression;

            Func<Test2, Test2> func = resultFunc.Compile();

            var t2 = new Test2() { B = 10 };
            Test2 result = func(t2);
            Assert.AreEqual(2, result.Value);
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

            MapReduceParser parser = new MapReduceParser(ctx);

            var parserResult = parser.MapParser(_xDoc.Element("Map"));
            var resultFunc = (Expression<Func<Test2, Test2>>)parserResult.Expression;

            Func<Test2, Test2> func = resultFunc.Compile();

            var t2 = new Test2() { B = 10 };
            Test2 result = func(t2);
            Assert.AreEqual(3, result.Value);
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

            MapReduceParser parser = new MapReduceParser(ctx);

            var parserResult = parser.MapParser(_xDoc.Element("Map"));
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
            MapReduceParser parser = new MapReduceParser(ctx);

            var parserResult = parser.MapParser(_xDoc.Element("Map"));
            var resultFunc = (Expression<Func<Test2, IEnumerable<Test3>>>)parserResult.Expression;

            Func<Test2, IEnumerable<Test3>> func = resultFunc.Compile();

            var t2 = new Test2() { B = 10 };
            IEnumerable<Test3> result = func(t2);
            Assert.AreEqual(13, result.Count());
        }
    }
}
