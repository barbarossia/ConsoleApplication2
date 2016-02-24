using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace MapReduce.Parser.UnitTest {
    [TestClass]
    public class UnitTest3 {
        [TestMethod]
        public void CondtionTest() {
            string xml = @"
                    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1' />
        <ReduceRule Type = 'AssignRuleOnT1' Condition='ConditionOnRuleTest1' />
    </Reduce>";
            Parser parser = xml.CreateParser("Reduce");
            parser.AddContext("ReduceRuleOnT1", "ClassLibrary1.ReduceRuleOnT1, ClassLibrary1");
            parser.AddContext("AssignRuleOnT1", "ClassLibrary1.AssignRuleOnT1, ClassLibrary1");
            parser.AddContext("ConditionOnRuleTest1", "MapReduce.Parser.UnitTest.ConditionOnRuleTest1, MapReduce.Parser.UnitTest");
            Assert.IsTrue(parser.ReduceBlock());
            PrivateObject po = new PrivateObject(parser);
            ParserResult parserResult = (ParserResult)po.GetField("currentReduceResult");
            var resultFunc = (Expression<Func<IEnumerable<Test2>, Test1, Test1>>)parserResult.Expression;

            Func<IEnumerable<Test2>, Test1, Test1> func = resultFunc.Compile();

            var t2List = new List<Test2>() { new Test2() { Result = 1 }, new Test2() { Result = 2 }, };
            var t1 = new Test1();
            Test1 result = func(t2List, t1);
            Assert.AreEqual(3, result.Result);
            Assert.IsNull(result.Details);
        }
    }
}
