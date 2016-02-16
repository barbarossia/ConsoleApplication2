using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
//using System.Linq;

namespace UnitTestProject1 {
    public delegate Result ParserFunc(string result);
    public delegate ParserFunc Future(string value);
    public class Result {
        public string Value { get; private set; }
        public Result(string value) {
            Value = value;
        }
    }
    public class Scanner {
        private List<string> innerList;
        private int current;
        public Scanner(IEnumerable<string> list) {
            innerList = new List<string>(list);
            current = 0;
        }
        public string Next() {
            return innerList[current++];
        }
    }
    public class ParserReference {

    }
    [TestClass]
    public class UnitTest3 {
        [TestMethod]
        public void TestMethod1() {
            //List<int> la = new List<int>() { 1, 2, 3 };
            //List<string> lb = new List<string>() { "a", "b", "c" };
            //List<int> lc = new List<int>() { 4, 5, 6 };
            int la = 1;
            string lb = "a";
            string lc = "4";

            //ParserFunc r = null;
            //r = from a in la.AsParser()
            //        from b in lb.AsParser()
            //        //from c in lc.AsParser()
            //        select Combine(a, b);
            //select Combine(a, "");
            //var r = la.AsParser().SelectMany(a => 
            //lb.AsParser().SelectMany(b => lc.AsParser(), (x, y) => Combine(x, y))
            //, (x, y) => Combine(x, y));
            //var result = r(lb);
        }

        private string Combine(string a, string b) {
            return a + b;
        }
        //private string Combine(string b, int c) {
        //    return b + c;
        //}


        [TestMethod]
        public void TestMethod2() {
            List<int> la = new List<int>() { 1, 2, 3 };
            List<string> lb = new List<string>() { "a", "b", "c" };

            var r = la.SelectMany(a => lb.SelectMany(b => a + b));
        }
    }

    internal static class MyLinq {
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) {
            //if(source == null) throw Error.ArgumentNull("source");
            //if(selector == null) throw Error.ArgumentNull("selector");
            return SelectManyIterator<TSource, TResult>(source, selector);
        }

        static IEnumerable<TResult> SelectManyIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector) {
            foreach(TSource element in source) {
                foreach(TResult subElement in selector(element)) {
                    yield return subElement;
                }
            }
        }

        //public static TResult SelectMany<TSource, TCollection, TResult>(this TSource source, Func<TSource, TCollection> collectionSelector, Func<TSource, TCollection, TResult> resultSelector) {
        //    TCollection subElement = collectionSelector(source);
        //    return resultSelector(source, subElement);
        //}
        public static ParserFunc SelectMany(this ParserFunc source, Func<string, ParserFunc> selector, Func<string, string, string> resultSelector) {
            return (result) => {
                var result1 = source(result);
                if(result1 == null) return null;
                var parser2 = selector(result1.Value);
                var result2 = parser2(result);
                return new Result(resultSelector(result1.Value, result2.Value));
            };
        }

        public static ParserFunc AsParser(this string a) {
            return (result) => { return new Result(a); };
        }
    }
}
