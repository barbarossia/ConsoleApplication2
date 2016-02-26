using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class RulesEngineConfigurationXmlProvider {
        private Context ctx = new Context();
        private Parser parser;
        private XDocument _xDoc;
        public RulesEngineConfigurationXmlProvider(string configKey) {
            _xDoc = XDocument.Load(configKey);
        }
        public RulesEngine GetRulesEngine() {
            XElement engineElement = GetRulesEngineElement();
            GetRuleTypesElement(engineElement);
            GetParser(engineElement);
            RulesEngine engine = new RulesEngine(parser);
            return engine;
        }

        private XElement GetRulesEngineElement() {
            IEnumerable<XElement> engineList =
                from el in _xDoc.Descendants("RulesEngine")
                select el;

            if(!engineList.Any())
                throw new RuleConfigurationException("Xml Document does not contain a RulesEngine Element");
            if(engineList.Count() > 1)
                throw new RuleConfigurationException("Xml Document must not contain more than one RulesEngine Element");

            XElement engineElement = engineList.First();

            return engineElement;
        }
        private void GetParser(XElement engineElement) {
            IEnumerable<XElement> parserList =
                from el in engineElement.Elements("MapReduce")
                select el;

            foreach(var el in parserList) {
                CreateParser(el);
            }
        }

        private void CreateParser(XElement parserElement) {
            string key = string.Empty;
            if (parserElement.Attribute("Name")!=null)
                key = parserElement.Attribute("Name").Value.Trim();

            parser = parserElement.CreateParser(ctx);
            if (!parser.Build()) throw new RuleConfigurationException("");
            ParserResult result = parser.Result;
            if(!string.IsNullOrWhiteSpace(key)) {
                ctx.Set(key, result);
            }
        }

        private void GetRuleTypesElement(XElement engineElement) {
            IEnumerable<XElement> ruleTypesList =
                from el in engineElement.Elements("RuleTypes")
                select el;

            GetRuleTypesFromConfig(ruleTypesList.First());
        }

        private void GetRuleTypesFromConfig(XElement ruleTypesElement) {
            try {
                IEnumerable<XElement> typeList =
                    from typeElement in ruleTypesElement.Elements("add")
                    select typeElement;

                string key, typeName;

                foreach(XElement addTypeElement in typeList) {
                    key = addTypeElement.Attribute("Key").Value.Trim();
                    typeName = addTypeElement.Attribute("Type").Value.Trim();

                    Type type = Type.GetType(typeName);
                    if(type == null)
                        throw new RuleConfigurationException(String.Format("Could not get type for configured RuleType [{0}]", typeName));
                    ctx.Set(key, type);
                }

            } catch(RuleConfigurationException rcex) {
                throw rcex;
            } catch(Exception ex) {
                string msg = String.Format("Could not get RuleTypeDictionary object based on configured values.");
                throw new RuleConfigurationException(msg, ex);
            }
        }
    }
}
