using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Linq;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;
using MapReduce.Parser;
using System.Linq.Expressions;
using MapReduce.Lexer;
using System;

namespace MapReduce.Parser.UnitTest {
    [TestClass]
    public class UnitTest4 {
        [TestMethod]
        public void ReferenceMapReduce() {
            string xml1 = @"<MapReduce>
                            <Map>
                                <MapRule Type = 'MapRuleOnT2' />
                            </Map>
                            <Reduce>
                                <ReduceRule Type = 'ReduceRuleOnT2' />
                                <ReduceRule Type = 'AssignRuleOnT2' />
                            </Reduce>
                        </MapReduce>";
            Parser parser = xml1.CreateParser("MapReduce");
            parser.AddContext("MapRuleOnT2", "ClassLibrary1.MapRuleOnT2, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT2", "ClassLibrary1.ReduceRuleOnT2, ClassLibrary1");
            parser.AddContext("AssignRuleOnT2", "ClassLibrary1.AssignRuleOnT2, ClassLibrary1");
            Assert.IsTrue(parser.Execute());
            ParserResult t2result = parser.Result;
            
            string xml2 = @"
            <MapReduce>
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                        <Rule ref='MapReduceOnT2' />
                    </ForEach>
                </Map>
                <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT1' />
                    <ReduceRule Type = 'AssignRuleOnT1' />
                </Reduce>
            </MapReduce>";
            parser = xml2.CreateParser("MapReduce");
            parser.AddContext("IninValueOnT1", "ClassLibrary1.IninValueOnT1, ClassLibrary1");
            parser.AddContext("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            parser.AddResult("MapReduceOnT2", t2result);
            Assert.IsTrue(parser.Execute());
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
