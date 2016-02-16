using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public static class RuleHelper {
        public static Func<T, T> Reduce<T, T2>(this Func<T, IEnumerable<T2>> map, Func<IEnumerable<T2>, T, T> reduce) {
            return input => reduce(map(input), input);
        }
        public static Func<T, T> ConcatRule<T>(this Func<T, T> rule, Func<T, T> other) {
            return input => other(rule(input));
        }
        public static Func<T, IEnumerable<TResult>> ConcatRuleMap<T, TResult>(this Func<T, T> rule, Func<T, IEnumerable<TResult>> map) {
            return input => map(rule(input));
        }
        public static Func<T, IEnumerable<TResult>> ConcatMap<T, TResult>(this Func<T, IEnumerable<TResult>> map, Func<T, IEnumerable<TResult>> other) {
            return input => map(input).Concat(other(input));
        }

        public static Func<T, IEnumerable<TResult>> ConcatForEach<T, TResult>(this Func<T, IEnumerable<TResult>> map, Func<IEnumerable<TResult>, IEnumerable<TResult>> other) {
            return input => other(map(input));
        }

        public static Func<IEnumerable<T>, IEnumerable<TResult>> Enumerate<T, TResult>(this Func<T, TResult> func) {
            return inputList => inputList.Select(i => func(i));
        }
        public static Func<T, TResult, TResult> ConcactReduce<T, TResult>(this Func<T, TResult, TResult> func, Func<T, TResult, TResult> other) {
            return (list, v) => {
                v = func(list, v);
                v = other(list, v);
                return v;
            };
        }
    }
}
