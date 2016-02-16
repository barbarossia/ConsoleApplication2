using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApplication3 {
    public class RulesEngineConfigurationXmlProvider<TCandidate> {
        private XDocument _xDoc;
        private RuleTypeDictionary ruleTypes;
        private RuleConditionDictionary ruleConditions;
        public void Load(XmlDocument xmlDocument) {
            //Validate(xmlDocument.OuterXml);
            XmlNodeReader xnr = new XmlNodeReader(xmlDocument);
            _xDoc = XDocument.Load(xnr);
        }

        public void LoadXml(string xml) {
            //Validate(xml);
            _xDoc = XDocument.Parse(xml);
        }
        public RulesEngine<T, R> GetRulesEngine<T, R>()  {
            var engine = new RulesEngine<T, R>();
            GetRulesEngine(engine);
            return engine;
        }
        private RulesEngine<T, R> GetRulesEngine<T, R>(RulesEngine<T, R> rulesEngine)  {
            XElement engineElement = GetRulesEngineElement();
            XElement ruleTypesElement = GetRuleTypesElement(engineElement);
            ruleTypes = GetRuleTypesFromConfig(ruleTypesElement);
            rulesEngine.RuleTypes = ruleTypes;

            XElement conditionTypesElement = GetRuleConditionsElement(engineElement);
            ruleConditions = GetConditionTypesFromConfig(conditionTypesElement);
            rulesEngine.ConditionTypes = ruleConditions;

            XElement ruleGroupElement = engineElement.Elements().ElementAt(2);

            rulesEngine.Root = ParseFactory.CreateNodeParse(ruleGroupElement).Parse();

            return rulesEngine;
        }

        protected XElement GetRulesEngineElement() {
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
        protected XElement GetRuleConditionsElement(XElement engineElement) {
            IEnumerable<XElement> condtionTypes =
                from el in engineElement.Elements("CondtionTypes")
                select el;

            XElement ruleTypesElement = condtionTypes.First();

            return ruleTypesElement;
        }

        protected RuleConditionDictionary GetConditionTypesFromConfig(XElement ruleTypesElement) {
            RuleConditionDictionary typeDictionary = new RuleConditionDictionary();

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
                    //if(!(Activator.CreateInstance(type) is IRule<TCandidate>))
                    //    throw new RuleConfigurationException(String.Format("Could not get type for configured RuleType [{0}].  Type does not implement IRule<{1}>", typeName, typeof(TCandidate).ToString()));

                    typeDictionary.Add(key, type);
                }

            } catch(RuleConfigurationException rcex) {
                throw rcex;
            } catch(Exception ex) {
                string msg = String.Format("Could not get RuleTypeDictionary object based on configured values.");
                throw new RuleConfigurationException(msg, ex);
            }

            return typeDictionary;
        }
        protected XElement GetRuleTypesElement(XElement engineElement) {
            IEnumerable<XElement> ruleTypesList =
                from el in engineElement.Elements("RuleTypes")
                select el;

            XElement ruleTypesElement = ruleTypesList.First();

            return ruleTypesElement;
        }

        protected RuleTypeDictionary GetRuleTypesFromConfig(XElement ruleTypesElement) {
            RuleTypeDictionary typeDictionary = new RuleTypeDictionary();

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
                    //if(!(Activator.CreateInstance(type) is IRule<TCandidate>))
                    //    throw new RuleConfigurationException(String.Format("Could not get type for configured RuleType [{0}].  Type does not implement IRule<{1}>", typeName, typeof(TCandidate).ToString()));

                    typeDictionary.Add(key, type);
                }

            } catch(RuleConfigurationException rcex) {
                throw rcex;
            } catch(Exception ex) {
                string msg = String.Format("Could not get RuleTypeDictionary object based on configured values.");
                throw new RuleConfigurationException(msg, ex);
            }

            return typeDictionary;
        }
    }
}
