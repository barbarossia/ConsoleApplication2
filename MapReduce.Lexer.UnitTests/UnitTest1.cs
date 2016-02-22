using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace MapReduce.Lexer.UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestRule() {
            string xml = @"
 <Rule Type = 'IninValueOnT2' />";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element("Rule");
            IMatcher rule = new TerminalRuleMatcher(new Pattern("Rule", TokenType.RULE));
            List<Token> results = new List<Token>();
            var t = new Tokenizer(source, results, rule);
            t.Parse();
            Assert.IsTrue(rule.IsMatch(t));

            Assert.AreEqual(TokenType.RULE, results[0].TokenType);
        }

        [TestMethod]
        public void TestReduce() {
            string xml = @"
                    <Reduce>
        <ReduceRule Type = 'ReduceRuleOnT1' />
        <ReduceRule Type = 'AssignRuleOnT1' />
    </Reduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element("Reduce");
            IMatcher reduceRule = new TerminalRuleMatcher(new Pattern("ReduceRule", TokenType.REDUCERULE));
            IMatcher reduce = new ProductionMatcher(new Pattern("Reduce", TokenType.REDUCE));
            List<Token> results = new List<Token>();
            var t = new Tokenizer(source, results, reduceRule, reduce);
            t.Parse();
            Assert.AreEqual(TokenType.REDUCE, results[0].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[1].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[2].TokenType);
            Assert.AreEqual(TokenType.EOF, results[3].TokenType);
        }

        [TestMethod]
        public void TestMapRulesOnlyRule() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml); XElement source = _xDoc.Element("Map");
            IMatcher rule = new TerminalRuleMatcher(new Pattern("Rule", TokenType.RULE));
            IMatcher map = new ProductionMatcher(new Pattern("Map", TokenType.MAP));
            List<Token> results = new List<Token>();
            var t = new Tokenizer(source, results, rule, map);
            t.Parse();
            Assert.AreEqual(TokenType.MAP, results[0].TokenType);
            Assert.AreEqual(TokenType.RULE, results[1].TokenType);
            Assert.AreEqual(TokenType.EOF, results[2].TokenType);
        }
        [TestMethod]
        public void TestMapRulesTwoRules() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT2' />
                    <Rule Type = 'IninValueOnT2Add' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml); XElement source = _xDoc.Element("Map");
            IMatcher rule = new TerminalRuleMatcher(new Pattern("Rule", TokenType.RULE));
            IMatcher map = new ProductionMatcher(new Pattern("Map", TokenType.MAP));
            List<Token> results = new List<Token>();
            var t = new Tokenizer(source, results, rule, map);
            t.Parse();
            Assert.AreEqual(TokenType.MAP, results[0].TokenType);
            Assert.AreEqual(TokenType.RULE, results[1].TokenType);
            Assert.AreEqual(TokenType.RULE, results[2].TokenType);
            Assert.AreEqual(TokenType.EOF, results[3].TokenType);
        }
        [TestMethod]
        public void TestMapRulesRuleAndMapRule() {
            string xml = @"
                <Map>
                    <Rule Type = 'IninValueOnT1' />
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                </Map>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element("Map");
            IMatcher rule = new TerminalRuleMatcher(new Pattern("Rule", TokenType.RULE));
            IMatcher mapRule = new TerminalRuleMatcher(new Pattern("MapRule", TokenType.MAPRULE));
            IMatcher map = new ProductionMatcher(new Pattern("Map", TokenType.MAP));
            List<Token> results = new List<Token>();
            var t = new Tokenizer(source, results, rule, mapRule, map);
            t.Parse();
            Assert.AreEqual(TokenType.MAP, results[0].TokenType);
            Assert.AreEqual(TokenType.RULE, results[1].TokenType);
            Assert.AreEqual(TokenType.MAPRULE, results[2].TokenType);
            Assert.AreEqual(TokenType.EOF, results[3].TokenType);
        }
        [TestMethod]
        public void TestMapAndReduce() {
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
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element("MapReduce");
            IMatcher rule = new TerminalRuleMatcher(new Pattern("Rule", TokenType.RULE));
            IMatcher mapRule = new TerminalRuleMatcher(new Pattern("MapRule", TokenType.MAPRULE));
            IMatcher reduceRule = new TerminalRuleMatcher(new Pattern("ReduceRule", TokenType.REDUCERULE));
            IMatcher map = new ProductionMatcher(new Pattern("Map", TokenType.MAP));
            IMatcher reduce = new ProductionMatcher(new Pattern("Reduce", TokenType.REDUCE));
            IMatcher mapReduce = new ProductionMatcher(new Pattern("MapReduce", TokenType.MAPREDUCE));

            List<Token> results = new List<Token>();
            var t = new Tokenizer(source, results, rule, mapRule, reduceRule, map, reduce, mapReduce);
            t.Parse();
            Assert.AreEqual(TokenType.MAPREDUCE, results[0].TokenType);
            Assert.AreEqual(TokenType.MAP, results[1].TokenType);
            Assert.AreEqual(TokenType.MAPRULE, results[2].TokenType);
            Assert.AreEqual(TokenType.EOF, results[3].TokenType);
            Assert.AreEqual(TokenType.REDUCE, results[4].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[5].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[6].TokenType);
            Assert.AreEqual(TokenType.EOF, results[7].TokenType);
            Assert.AreEqual(TokenType.EOF, results[8].TokenType);
        }

        [TestMethod]
        public void TestLexer() {
            string xml = @"
            <MapReduce>
                <Map>
                    <MapRule Type = 'MapRuleOnT1IfTrue' />
                    <ForEach>
                      <Rule Type = 'IninValueOnT3'/>
                    </ForEach>
                </Map>
             <Reduce>
                    <ReduceRule Type = 'ReduceRuleOnT1' />
                    <ReduceRule Type = 'AssignRuleOnT1' />
                </Reduce>
            </MapReduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element("MapReduce");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            Assert.AreEqual(TokenType.MAPREDUCE, results[0].TokenType);
            Assert.AreEqual(TokenType.MAP, results[1].TokenType);
            Assert.AreEqual(TokenType.MAPRULE, results[2].TokenType);
            Assert.AreEqual(TokenType.FOREACH, results[3].TokenType);
            Assert.AreEqual(TokenType.RULE, results[4].TokenType);
            Assert.AreEqual(TokenType.EOF, results[5].TokenType);
            Assert.AreEqual(TokenType.EOF, results[6].TokenType);
            Assert.AreEqual(TokenType.REDUCE, results[7].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[8].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[9].TokenType);
            Assert.AreEqual(TokenType.EOF, results[10].TokenType);
            Assert.AreEqual(TokenType.EOF, results[11].TokenType);
        }
        [TestMethod]
        public void TestIgnorCase() {
            string xml = @"
            <mapreduce>
                <map>
                    <maprule type = 'mapruleont1iftrue' />
                    <foreach>
                      <rule type = 'ininvalueont3'/>
                    </foreach>
                </map>
             <reduce>
                    <reducerule type = 'reduceruleont1' />
                    <reducerule type = 'assignruleont1' />
                </reduce>
            </mapreduce>";
            XDocument _xDoc = _xDoc = XDocument.Parse(xml);
            XElement source = _xDoc.Element("mapreduce");
            var lexer = new Lexer(source);
            var results = lexer.Lex().ToList();
            Assert.AreEqual(TokenType.MAPREDUCE, results[0].TokenType);
            Assert.AreEqual(TokenType.MAP, results[1].TokenType);
            Assert.AreEqual(TokenType.MAPRULE, results[2].TokenType);
            Assert.AreEqual(TokenType.FOREACH, results[3].TokenType);
            Assert.AreEqual(TokenType.RULE, results[4].TokenType);
            Assert.AreEqual(TokenType.EOF, results[5].TokenType);
            Assert.AreEqual(TokenType.EOF, results[6].TokenType);
            Assert.AreEqual(TokenType.REDUCE, results[7].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[8].TokenType);
            Assert.AreEqual(TokenType.REDUCERULE, results[9].TokenType);
            Assert.AreEqual(TokenType.EOF, results[10].TokenType);
            Assert.AreEqual(TokenType.EOF, results[11].TokenType);
        }
    }
}
