using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapReduce.Parser;
using System.Linq.Expressions;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;

namespace MapReduce.Parser.UnitTest {
    [TestClass]
    public class UnitTest2 {
        [TestMethod]
        public void ForEachNoItems() {
            string xml = @"
                <Map>
                    <MapRule Type = 'MapRuleOnT2' />
                    <ForEach>
                    </ForEach>
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            Assert.IsFalse(parser.MapBlock());
            Assert.IsInstanceOfType(parser.Result.Error, typeof(SyntaxException));
            Assert.AreEqual("Map has encurred the error!", parser.Result.Error.Message);
        }

        [TestMethod]
        public void ForEachOnlyRule() {
            string xml = @"
                <Map>
                    <MapRule Type = 'MapRuleOnT2' />
                    <ForEach>
                      <Rule Type = 'IninValueOnT3'/>
                    </ForEach>
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            parser.AddContext("IninValueOnT3", "ClassLibrary1.IninValueOnT3, ClassLibrary1");
            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test2, IEnumerable<Test3>>>)parserResult;

            Func<Test2, IEnumerable<Test3>> func = resultFunc.Compile();

            var t2 = new Test2() { B = 2 };
            IEnumerable<Test3> result = func(t2);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void ForEachMapRule() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                      <MapRule Type = 'MapRuleOnT2' />
                    </ForEach>
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");

            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, IEnumerable<IEnumerable<Test3>>>>)parserResult;

            Func<Test1, IEnumerable<IEnumerable<Test3>>> func = resultFunc.Compile();

            var t1 = new Test1() { A = 2 };
            IEnumerable<IEnumerable<Test3>> result1 = func(t1);
            IEnumerable<Test3> result2 = result1.SelectMany(r => r.Select(l => l));
            Assert.AreEqual(2, result1.Count());
            Assert.AreEqual(3, result2.Count());
        }

        [TestMethod]
        public void MapReduceForEachMapReduce() {
            string xml = @"
            <MapReduce>
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                        <MapReduce>
                            <Map>
                                <MapRule Type = 'MapRuleOnT2' />
                            </Map>
                            <Reduce>
                                <ReduceRule Type = 'ReduceRuleOnT2' />
                                <ReduceRule Type = 'AssignRuleOnT2' />
                            </Reduce>
                        </MapReduce>
                    </ForEach>
                </Map>
                <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT1' />
                    <ReduceRule Type = 'AssignRuleOnT1' />
                </Reduce>
            </MapReduce>";
            Parser parser = xml.CreateParser("MapReduce");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            parser.AddContext("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");

            Assert.IsTrue(parser.Build());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, Test1>>)parserResult;

            Func<Test1, Test1> func = resultFunc.Compile();

            var t1 = new Test1() { A = 2 };
            Test1 result = func(t1);
            Assert.AreEqual(2, result.Details.Count());
        }
        [TestMethod]
        public void ForEachMapReduce() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                        <MapReduce>
                            <Map>
                                <MapRule Type = 'MapRuleOnT2' />
                            </Map>
                            <Reduce>
                                <ReduceRule Type = 'ReduceRuleOnT2' />
                                <ReduceRule Type = 'AssignRuleOnT2' />
                            </Reduce>
                        </MapReduce>
                    </ForEach>
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            parser.AddContext("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");

            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, IEnumerable<Test2>>>)parserResult;

            Func<Test1, IEnumerable<Test2>> func = resultFunc.Compile();

            var t1 = new Test1() { A = 10 };
            List<Test2> result = func(t1).ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(55, result[9].Result);
            Assert.AreEqual(10, result[9].Details.Count());
        }

        [TestMethod]
        public void ForEachMapGroupReduce() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                        <MapReduce>
                            <Map>
                                <MapRule Type = 'MapRuleOnT2Test' />
                                <MapRule Type = 'MapRuleOnT2Add' />
                            </Map>
                            <Reduce>
                                <ReduceRule Type = 'ReduceRuleOnT2' />
                                <ReduceRule Type = 'AssignRuleOnT2' />
                            </Reduce>
                        </MapReduce>
                    </ForEach>
                </Map>";
            Parser parser = xml.CreateParser("Map");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("MapRuleOnT2Test", "MapReduce.Parser.UnitTest.MapRuleOnT2Test, MapReduce.Parser.UnitTest");
            parser.AddContext("MapRuleOnT2Add", "MapReduce.Parser.UnitTest.MapRuleOnT2Add, MapReduce.Parser.UnitTest");
            parser.AddContext("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            parser.AddContext("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");

            Assert.IsTrue(parser.MapBlock());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, IEnumerable<Test2>>>)parserResult;

            Func<Test1, IEnumerable<Test2>> func = resultFunc.Compile();

            var t1 = new Test1() { A = 10 };
            List<Test2> result = func(t1).ToList();
            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(103, result[9].Details.Count());
            Assert.AreEqual(103, result[0].Details.Count());
        }

        [TestMethod]
        public void NestedForEachMapReduce() {
            string xml = @"
            <MapReduce>
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                        <MapReduce>
                            <Map>
                                <Rule Type = 'IninValueOnT2' />
                                <MapRule Type = 'MapRuleOnT2' />
                                <ForEach>
                                    <Rule Type = 'IninValueOnT3' />
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
                    <ReduceRule Type = 'ReduceRuleOnT1' />
                    <ReduceRule Type = 'AssignRuleOnT1' />
                </Reduce>
            </MapReduce>";
            Parser parser = xml.CreateParser("MapReduce");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("IninValueOnT2", "ClassLibrary1.IninValueOnT2, ClassLibrary1");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            parser.AddContext("IninValueOnT3", "ClassLibrary1.IninValueOnT3, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            parser.AddContext("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");

            Assert.IsTrue(parser.Build());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, Test1>>)parserResult;

            Func<Test1, Test1> func = resultFunc.Compile();

            var t1 = new Test1() { A = 10 };
            Test1 result = func(t1);
            Assert.AreEqual(10, result.Details.Count());
            Assert.AreEqual(220, result.Result);
        }
    }
}
