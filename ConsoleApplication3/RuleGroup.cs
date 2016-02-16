using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public class RuleGroup {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public RuleGroup Child { get; set; }
        public RuleGroup() {
        }


        private T Get<T>(string key) {
            if(key == null) throw new System.ArgumentNullException("key");
            object result;
            if(_values.TryGetValue(key, out result)) {
                return (T)result;
            }

            //throw new KeyNotFoundException(key);
            return default(T);
        }

        public Func<T, T> Compile<T>() {
            var sourceType = Get(RegistryKeys.SourceType);
            var targetType = Get(RegistryKeys.TargetType);
            if(targetType == null)
                targetType = sourceType;
            var invoker =  (IRuleGroupInvoker<T>)Utilities.CreateType(typeof(RuleGroupInvoker<,>), sourceType, targetType)
                                                                .CreateInstance(this);
            return invoker.Compile();
        }


        public Func<T, R> CompileRule<T, R>(IFluentScopeKey<IRule> key) {
            var rules = GetRules<T, R>(key.Key);
            return ConcatRules(rules);
        }

        public Func<T, R, R> CompileReduceRule<T, R>(IFluentScopeKey<IRule> key) {
            var rules = GetRules<T, Func<R, R>>(key.Key);
            return ConcatRules(rules);
        }

        private Func<T, R> ConcatRules<T, R>(IEnumerable<IRule<T, R>> rules) {
            if (rules == null || rules.Count() == 0) return null;
            RuleInvoker<T, R> invoker = CreateInvoker<T, R>(typeof(T), typeof(R));
            var ruleChain = rules.Select(r => invoker.Invoke(r));
            return ruleChain.Aggregate((curr, next) => curr.Concact(next));
        }

        private Func<T, R, R> ConcatRules<T, R>(IEnumerable<IRule<T, Func<R, R>>> rules) {
            if (rules == null || rules.Count() == 0) return null;
            RuleInvoker<T, Func<R, R>> invoker = CreateInvoker<T, Func<R, R>>(typeof(T), typeof(Func<R, R>));
            var ruleChain = rules.Select(r => invoker.Invoke(r)).Select(v => v.UnCurrey());

            return ruleChain.Aggregate((curr, next) => curr.Concact(next));

        }


        public T Get<T>(IFluentScopeKey<T> key) {
            return Get<T>(key.Key);
        }

        private RuleInvoker<T, TResult> CreateInvoker<T, TResult>(Type sourceType, Type targetType) {
            return (RuleInvoker<T, TResult>)Utilities.CreateType(typeof(RuleInvoker<, >), sourceType, targetType)
                                                                .CreateInstance();
        }
        private RuleInvoker<T,T> CreateInvoker<T>(Type sourceType) {
            return CreateInvoker<T, T>(sourceType, sourceType);
        }

        private List<IRule<T, R>> GetRules<T, R>(string key) {
            if (key == null) throw new System.ArgumentNullException("key");
            object result;
            if (_values.TryGetValue(key, out result)) {
                return (List<IRule<T, R>>)result;
            }

            //throw new KeyNotFoundException(key);
            return null;
        }
        public void SetRule<T, R>(IFluentScopeKey<IRule> key, IRule<T, R> value) {
            if(key == null) throw new System.ArgumentNullException("key");
            List<IRule<T, R>> rules = GetRules<T, R>(key.Key);
            if (rules == null) {
                rules = new List<IRule<T, R>>();
                _values[key.Key] = rules;
            }
            rules.Add(value);
           
        }

        public void Set<T>(IFluentScopeKey<T> key, T value) {
            if(key == null) throw new System.ArgumentNullException("key");
            _values[key.Key] = value;
        }
    }
}
