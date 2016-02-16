using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApplication3 {
    public static class Helper {

        public static Func<T, TResult> Compose<T, TMid, TResult>(this Func<T, TMid> func, Func<TMid, TResult> other) {
            return inputParam => other(func(inputParam));
        }

        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> func) {
            return x => y => func(x, y);
        }

        public static Func<T1, T2, T3> UnCurrey<T1, T2, T3>(this Func<T1, Func<T2, T3>> func) {
            return (x, y) => func(x)(y);
        }

        public static Func<T, T> Reduce<T, T2>(this Func<T, IEnumerable<T2>> map, Func<IEnumerable<T2>, T, T> reduce) {
            return input => reduce(map(input), input);
        }

        public static Func<IEnumerable<T>, R, R> FoldL<T, R>(Func<R, T, R> acc) {
            return (list, v) => { foreach (var item in list) v = acc(v, item); return v; };
        }

        public static Func<T, TResult, TResult> Concact<T, TResult>(this Func<T, TResult, TResult> func, Func<T, TResult, TResult> other) {
            return (list, v) => {
                v = func(list, v);
                v = other(list, v);
                return v;
            };
        }

        public static Func<T, TResult> Concact<T, TResult>(this Func<T, TResult> func, Func<T, TResult> other) {
            return (t) => {
                var v = func(t);
                v = other(t);
                return v;
            };
        }

        public static Func<IEnumerable<T>, IEnumerable<TResult>> Enumerate<T, TResult>(this Func<T, TResult> func) {
            return inputList => inputList.Select(i => func(i));
        }

        public static Func<IEnumerable<T>, IEnumerable<T>> Filter<T>(Func<T, bool> func) {
            return inputList => inputList.Where(i => func(i));
        }

        public static Func<T, IEnumerable<TResult>> Concat<T, TResult>(this Func<T, IEnumerable<TResult>> map, Func<IEnumerable<TResult>, IEnumerable<TResult>> other) {
            return input => other(map(input));
        }

        public static Func<T, IEnumerable<TResult>> ConcatMap<T, TResult>(this Func<T, IEnumerable<TResult>> map, Func<T, IEnumerable<TResult>> other) {
            return input => map(input).Concat(other(input));
        }

        public static bool IsRule(this XElement node) {
            return node.Name == "Rule";
        }
        public static bool IsMapRule(this XElement node) {
            return node.Name == "MapRule";
        }
        public static bool IsReduceRule(this XElement node) {
            return node.Name == "ReduceRule";
        }

        public static bool IsSequence(this XElement node) {
            return node.Name == "Sequence";
        }
        public static bool IsIf(this XElement node) {
            return node.Name == "If";
        }
        public static bool IsForEach(this XElement node) {
            return node.Name == "ForEach";
        }
    }
}
