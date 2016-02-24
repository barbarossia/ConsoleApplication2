using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using ConsoleApplication5;
using ClassLibrary1;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest6 {
        [TestMethod]
        public void TestIf() {
            string xml = @"
<If>
    <If.Condition Type='ConditionRuleOnT1'/>
    <If.Then>
        <MapRule Type = 'MapRuleOnT1IfTrue' ></MapRule >
    </If.Then>
</If> ";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("If"));
            Context ctx = new Context();
            ctx.Items.Add("ConditionRuleOnT1", "ClassLibrary1.ConditionRuleOnT1, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            var token = parser.Parse(ctx);
            CompileImp<Test1, Test2> compile = new CompileImp<Test1, Test2>();
            var node = (MapRuleNode<Test1, Test2>)compile.Compile(token);

            var func = node.Compile();

            var t1 = new Test1() { A = 10};
            var result = func(t1);
            Assert.AreEqual(10, result.Count());
        }

        [TestMethod]
        public void TestIfElse() {
            string xml = @"
<If>
    <If.Condition Type='ConditionRuleOnT1False' />
    <If.Then>
        <MapRule Type = 'MapRuleOnT1IfTrue' ></MapRule>
    </If.Then>
    <If.Else>
        <MapRule Type = 'MapRuleOnT1IfFalse' ></MapRule>
    </If.Else>
</If> ";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("If"));
            Context ctx = new Context();
            ctx.Items.Add("ConditionRuleOnT1False", "UnitTestProject1.ConditionRuleOnT1False, UnitTestProject1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfFalse", "ClassLibrary1.MapRuleOnT1IfFalse, ClassLibrary1");
            var token = parser.Parse(ctx);
            CompileImp<Test1, Test2> compile = new CompileImp<Test1, Test2>();
            var node = (MapRuleNode<Test1, Test2>)compile.Compile(token);

            var func = node.Compile();

            var t1 = new Test1() { A = 10 };
            var result = func(t1);
            Assert.AreEqual(5, result.Count());
        }

        [TestMethod]
        public void TestIfNot() {
            string xml = @"
<If>
    <If.Condition Type='ConditionRuleOnT1False'/>
    <If.Then>
        <MapRule Type = 'MapRuleOnT1IfTrue' ></MapRule >
    </If.Then>
</If> ";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("If"));
            Context ctx = new Context();
            ctx.Items.Add("ConditionRuleOnT1False", "UnitTestProject1.ConditionRuleOnT1False, UnitTestProject1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            var token = parser.Parse(ctx);

            Assert.IsNull(token);
        }

        [TestMethod]
        public void TestNestedIf() {
            string xml = @"
<If>
    <If.Condition Type='ConditionRuleOnT1' />
    <If.Then>
        <If>
            <If.Condition Type='ConditionRuleOnT1False'/>
            <If.Then>
                <MapRule Type = 'MapRuleOnT1IfTrue' ></MapRule >
            </If.Then>
            <If.Else>
                <MapRule Type = 'MapRuleOnT1IfFalse' ></MapRule>
            </If.Else>
        </If> 
    </If.Then>
</If> ";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            TokenParse parser = new TokenParse(_xDoc.Element("If"));
            Context ctx = new Context();
            ctx.Items.Add("ConditionRuleOnT1", "ClassLibrary1.ConditionRuleOnT1, ClassLibrary1");
            ctx.Items.Add("ConditionRuleOnT1False", "UnitTestProject1.ConditionRuleOnT1False, UnitTestProject1");
            ctx.Items.Add("MapRuleOnT1IfTrue", "ClassLibrary1.MapRuleOnT1IfTrue, ClassLibrary1");
            ctx.Items.Add("MapRuleOnT1IfFalse", "ClassLibrary1.MapRuleOnT1IfFalse, ClassLibrary1");
            var token = parser.Parse(ctx);
            CompileImp<Test1, Test2> compile = new CompileImp<Test1, Test2>();
            var node = (MapRuleNode<Test1, Test2>)compile.Compile(token);

            var func = node.Compile();

            var t1 = new Test1() { A = 10 };
            var result = func(t1);
            Assert.AreEqual(5, result.Count());
        }
    }

    class ConditionRuleOnT1False : IQulification {
        public bool IsQualified() {
            return false; 
        }
    }
}
