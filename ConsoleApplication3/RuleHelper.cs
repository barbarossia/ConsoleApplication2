using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public class RuleHelper {
        public static Func<T, IEnumerable<TResult>> Map<T, TResult>(IMapRule<T, TResult> rule) {
            return t => rule.Execute(t);
        }

        public static Func<IEnumerable<T>, TResult, TResult> Reduce<T, TResult>(IReduceRule<T, TResult> rule) {
            return (list, t) => rule.ExecuteCore(list, t);
        }

        public static Func<T, T> Spin<T>(IRule<T> rule) {
            return (t) => rule.Execute(t);
        }
    }
}
