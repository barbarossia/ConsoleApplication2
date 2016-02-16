using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public interface IRuleGroupInvoker<T> {
        Func<T, T> Compile();
    }
    public class RuleGroupInvoker<T, TResult> : IRuleGroupInvoker<T> {
        private RuleGroup group;
        public RuleGroupInvoker(RuleGroup ruleGroup) {
            group = ruleGroup;
        }
        public Func<T, T> Compile() {
            var func1 = group.CompileRule<T,T>(RegistryKeys.Rule);
            var func2 = CompileResult();
            if(func2 == null) return func1;


            var func3 = group.CompileReduceRule<IEnumerable<TResult>, T>(RegistryKeys.ReduceRule);

            return func1.Compose(func2).Reduce(func3);
                

        }

        private Func<T, IEnumerable<TResult>> CompileResult() {
            Func<T, IEnumerable<TResult>> map = group.CompileRule<T, IEnumerable<TResult>>(RegistryKeys.MapRule);
            if (map == null) return null;
            Func<IEnumerable<TResult>, IEnumerable<TResult>> nestedGroup = CompileGroup();
            if (nestedGroup != null)
                return map.Compose(nestedGroup);
            return map;
        }

        private Func<TResult, TResult> CompileGroupResult() {
            if (group.Child == null) return null;
            return group.Child.Compile<TResult>();
        }

        private Func<IEnumerable<TResult>, IEnumerable<TResult>> CompileGroup() {
            var result = CompileGroupResult();
            if (result == null) return null;
            return result.Enumerate();
        }

    }
}
