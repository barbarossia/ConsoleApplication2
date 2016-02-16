using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public interface IRuleRegister {
        void RegisterRule(IRule type);
    }
    public class RuleRegister<T, TResult> : IRuleRegister {
        private RuleGroup group;
        public RuleRegister(RuleGroup ruleGroup) {
            group = ruleGroup;
        }
        public void RegisterRule(IRule rule) {
            switch (rule.RuleKind) {
                case RuleKinds.Rule:
                    Register((IRule<T>)rule);
                    break;
                case RuleKinds.MapRule:
                    Register((IMapRule<T, TResult>)rule);
                    break;
                case RuleKinds.ReduceRule:
                    Register((IReduceRule<TResult, T>)rule);
                    break;
            }
        }

        private void Register(IRule<T> rule) {
            group.SetRule(RegistryKeys.Rule, rule);
        }
        private void Register(IMapRule<T, TResult> rule) {
            group.SetRule(RegistryKeys.MapRule, rule);
        }
        private void Register(IReduceRule<TResult, T> rule) {
            group.SetRule(RegistryKeys.ReduceRule, rule);
        }
    }
}
