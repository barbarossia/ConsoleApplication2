using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using ClassLibrary1;
using System.Linq;
using System.Collections.Generic;

namespace MapReduce.Parser.UnitTest {
    [TestClass]
    public class UnitTest5 {
        [TestMethod]
        public void PerfermentTest() {
            DateTime processingBeginDateTime = DateTime.UtcNow;
            var list = Enumerable.Range(1, 10000)
               .Select(t => new Test1() { A = t });

            Func<Test1, Test1> func = Build();
            doTests(list, func);
            DateTime processingEndDateTime = DateTime.UtcNow;
            double processingSeconds = ProcessTiming.DateDiff("s", processingEndDateTime, processingBeginDateTime);
            Console.WriteLine(processingSeconds);

        }
        private void doTests(IEnumerable<Test1> list, Func<Test1, Test1> func) {
            foreach(var t1 in list) {
                var result = func(t1);
                Assert.AreEqual(100, result.Details.Count());
                Assert.AreEqual(505000, result.Result);
            }
        }
        private Func<Test1, Test1> Build() {
            string xml1 = @"<MapReduce>
                            <Map>
                                <MapRule Type = 'MapRuleOnT2Test' />
                            </Map>
                            <Reduce>
                                <ReduceRule Type = 'ReduceRuleOnT2' />
                                <ReduceRule Type = 'AssignRuleOnT2' />
                            </Reduce>
                        </MapReduce>";
            Parser parser = xml1.CreateParser("MapReduce");
            parser.AddContext("MapRuleOnT2Test", "MapReduce.Parser.UnitTest.MapRuleOnT2Test, MapReduce.Parser.UnitTest");
            parser.AddContext("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            parser.AddContext("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");
            Assert.IsTrue(parser.Build());
            ParserResult t2result = parser.Result;

            string xml2 = @"
            <MapReduce>
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1Test' />
                    <ForEach>
                        <Rule ref='MapReduceOnT2TestRef' />
                    </ForEach>
                </Map>
                <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT1' />
                    <ReduceRule Type = 'AssignRuleOnT1' />
                </Reduce>
            </MapReduce>";
            parser = xml2.CreateParser("MapReduce");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1Test", "MapReduce.Parser.UnitTest.MapRuleOnT1Test, MapReduce.Parser.UnitTest");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            parser.AddResult("MapReduceOnT2TestRef", t2result);
            Assert.IsTrue(parser.Build());
            var parserResult = parser.Result.Expression;
            var resultFunc = (Expression<Func<Test1, Test1>>)parserResult;
            return resultFunc.Compile();
        }

    }
}
