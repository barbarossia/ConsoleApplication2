using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using ConsoleApplication5;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest5 {
        [TestMethod]
        public void ParseTest() {
            string xml = @"
<MapReduce>
    <Map>
        <Rule Type='IninValueOnT1'  SourceType ='ClassLibrary1.Test1, ClassLibrary1'/>
        <MapRule Type = 'MapRuleOnT1IfTrue' SourceType = 'ClassLibrary1.Test1, ClassLibrary1' TargetType = 'ClassLibrary1.Test2, ClassLibrary1' ></MapRule>
        <ForEach>
            <MapReduce>
                <Map>
                    <Rule Type = 'IninValueOnT2' SourceType = 'ClassLibrary1.Test2, ClassLibrary1' />
                    <MapRule Type = 'MapRuleOnT2' SourceType = 'ClassLibrary1.Test2, ClassLibrary1' TargetType = 'ClassLibrary1.Test3, ClassLibrary1' />
                    <ForEach>
                        <Rule Type = 'IninValueOnT3' SourceType = 'ClassLibrary1.Test3, ClassLibrary1' />
                    </ForEach>
                </Map>
                <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT2' SourceType = 'ClassLibrary1.Test3, ClassLibrary1' TargetType = 'ClassLibrary1.Test2, ClassLibrary1' />
                    <ReduceRule Type = 'AssignRuleOnT2' SourceType = 'ClassLibrary1.Test3, ClassLibrary1' TargetType = 'ClassLibrary1.Test2, ClassLibrary1' />
                </Reduce>
            </MapReduce>
        </ForEach>
    </Map>
    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1' SourceType = 'ClassLibrary1.Test2, ClassLibrary1' TargetType = 'ClassLibrary1.Test1, ClassLibrary1' />
        <ReduceRule Type = 'AssignRuleOnT1' SourceType = 'ClassLibrary1.Test2, ClassLibrary1' TargetType = 'ClassLibrary1.Test1, ClassLibrary1' />
    </Reduce>
</MapReduce> ";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("MapReduce"));
            Context ctx = new Context();
            var result = parser.Parse(ctx);
        }
        [TestMethod]
        public void CompileMapAndForEachTest() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <MapRule Type = 'MapRuleOnT2'  />
                    <ForEach>
                        <Rule Type = 'IninValueOnT3' />
                    </ForEach>
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("Map"));
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            ctx.Items.Add("IninValueOnT3", "ClassLibrary1.IninValueOnT3, ClassLibrary1");
            var mapToken = parser.Parse(ctx);
            CompileImp<Test2, Test3> compile = new CompileImp<Test2, Test3>();
            MapNode<Test2, Test3> node = (MapNode<Test2, Test3>)compile.Compile(mapToken);

            Func<Test2, IEnumerable<Test3>> func = node.Compile();

            var t2 = new Test2() { B = 10 };
            IEnumerable<Test3> result = func(t2);
            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public void CompileMapTest() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <MapRule Type = 'MapRuleOnT2' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("Map"));
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            var mapToken = parser.Parse(ctx);
            CompileImp<Test2, Test3> compile = new CompileImp<Test2, Test3>();
            MapNode<Test2, Test3> node = (MapNode<Test2, Test3>)compile.Compile(mapToken);

            Func<Test2, IEnumerable<Test3>> func = node.Compile();

            var t2 = new Test2() { B = 10 };
            IEnumerable<Test3> result = func(t2);
            Assert.AreEqual(10, result.Count());
        }
        [TestMethod]
        public void CompileReduceTest() {
            string xml = @"
                    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1' />
        <ReduceRule Type = 'AssignRuleOnT1' />
    </Reduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("Reduce"));
            Context ctx = new Context();
            ctx.Items.Add("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            var reduceToken = parser.Parse(ctx);
            CompileImp<Test2, Test1> compile = new CompileImp<Test2, Test1>();
            ReduceNode<Test2, Test1> node = (ReduceNode<Test2, Test1>)compile.Compile(reduceToken);

            Func<IEnumerable<Test2>, Test1, Test1> func = node.Compile();

            var t2List = new List<Test2>() { new Test2() { Result = 1 }, new Test2() { Result = 2 }, };
            var t1 = new Test1();
            Test1 result = func(t2List, t1);
            Assert.AreEqual(3, result.Result);
            Assert.AreEqual(2, result.Details.Count());
        }

        [TestMethod]
        public void CompileMapReduceTest() {
            string xml = @"
<MapReduce>
    <Map>
        <Rule Type='IninValueOnT1'/>
        <MapRule Type = 'MapRuleOnT1IfTrue'></MapRule>
        <ForEach>
            <Rule Type = 'IninValueOnT2' />
        </ForEach>
    </Map>
    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1'/>
        <ReduceRule Type = 'AssignRuleOnT1'/>
    </Reduce>
</MapReduce> ";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("MapReduce"));
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            ctx.Items.Add("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            var mdToken = parser.Parse(ctx);
            CompileImp<Test1, Test2> compile = new CompileImp<Test1, Test2>();
            MapReduceNode<Test1, Test2> node = (MapReduceNode<Test1, Test2>)compile.Compile(mdToken);
            Func<Test1, Test1> func = node.Compile();

            var t1 = new Test1() { A = 10};
            Test1 result = func(t1);
            Assert.AreEqual(10, result.Details.Count());
        }
        [TestMethod]
        public void CompileDoubleMapReduceTest() {

            string xml1 = @"
 <MapReduce name='MapReduceOfTest2'>
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <MapRule Type = 'MapRuleOnT2' />
                    <ForEach>
                        <Rule Type = 'IninValueOnT3'/>
                    </ForEach>
                </Map>
                <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT2' />
                    <ReduceRule Type = 'AssignRuleOnT2' />
                </Reduce>
</MapReduce>";
            string xml2 = @"
<MapReduce name='MapReduceOfTest1'>
    <Map>
        <Rule Type='IninValueOnT1'/>
        <MapRule Type = 'MapRuleOnT1IfTrue'></MapRule>
        <ForEach>
           <MapReduce ref='MapReduceOfTest2'/>
        </ForEach>
    </Map>
    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1'/>
        <ReduceRule Type = 'AssignRuleOnT1'/>
    </Reduce>
</MapReduce> ";
            XDocument _xDoc = XDocument.Parse(xml1);            
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            ctx.Items.Add("IninValueOnT3", "ClassLibrary1.IninValueOnT3, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            ctx.Items.Add("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            ctx.Items.Add("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");
            TokenParse parser = new TokenParse(_xDoc.Element("MapReduce"));
            var mdToken1 = parser.Parse(ctx);

            _xDoc = XDocument.Parse(xml2);
            parser = new TokenParse(_xDoc.Element("MapReduce"));
            var mdToken2 = parser.Parse(ctx);
            CompileImp<Test1, Test2> compile = new CompileImp<Test1, Test2>();
            MapReduceNode<Test1, Test2> node = (MapReduceNode<Test1, Test2>)compile.Compile(mdToken2);

            Func<Test1, Test1> func = node.Compile();

            var t1 = new Test1() { A = 10 };
            Test1 result = func(t1);
            Assert.AreEqual(220, result.Result);
            Assert.AreEqual(10, result.Details.Count());
        }

        [TestMethod]
        public void CompileNestedMapReduceTest() {
            string xml = @"
<MapReduce>
    <Map>
        <Rule Type='IninValueOnT1'/>
        <MapRule Type = 'MapRuleOnT1IfTrue'></MapRule>
        <ForEach>
           <MapReduce name='MapReduceOfTest2'>
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <MapRule Type = 'MapRuleOnT2' />
                    <ForEach>
                        <Rule Type = 'IninValueOnT3'/>
                    </ForEach>
                </Map>
                <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT2' />
                    <ReduceRule Type = 'AssignRuleOnT2' />
                </Reduce>
</MapReduce>
        </ForEach>
    </Map>
    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1'/>
        <ReduceRule Type = 'AssignRuleOnT1'/>
    </Reduce>
</MapReduce> ";
            XDocument _xDoc = XDocument.Parse(xml);
            Context ctx = new Context();
            ctx.Items.Add("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            ctx.Items.Add("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            ctx.Items.Add("IninValueOnT3", "ClassLibrary1.IninValueOnT3, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            ctx.Items.Add("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            ctx.Items.Add("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            ctx.Items.Add("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");
            TokenParse parser = new TokenParse(_xDoc.Element("MapReduce"));
            var mdToken = parser.Parse(ctx);
            CompileImp<Test1, Test2> compile = new CompileImp<Test1, Test2>();
            MapReduceNode<Test1, Test2> node = (MapReduceNode<Test1, Test2>)compile.Compile(mdToken);

            Func<Test1, Test1> func = node.Compile();

            var t1 = new Test1() { A = 10 };
            Test1 result = func(t1);
            Assert.AreEqual(220, result.Result);
            Assert.AreEqual(10, result.Details.Count());
        }

    }
}
