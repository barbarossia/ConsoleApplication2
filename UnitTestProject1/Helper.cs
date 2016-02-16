using ConsoleApplication3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1 {
    public static class ExtentionMethods {

        public static ParseFunc<T, R> AsParser<T, R>(this Token token) {
            return scanner => {
                var ruleType = scanner.RuleTypes[token.Key.Key];
                var rule = (IRule<T, R>)ruleType.CreateInstance();

                return new Result<T, R>((t => rule.Execute(t)), scanner);
            };
        }
        public static ParseFunc<IEnumerable<T>, IEnumerable<R>> AsForEachParser<T, R>(this Token token) {
            return scanner => {
                var ruleType = scanner.RuleTypes[token.Key.Key];
                var rule = (IRule<T, R>)ruleType.CreateInstance();
                Func<T, R> f = t => rule.Execute(t);
                var ff = f.Enumerate();
                return new Result<IEnumerable<T>, IEnumerable<R>>(ff, scanner);
            };
        }
        public static ParseFunc<T, R> AsReduceParser<T, R>(this Token token) {
            return scanner => {
                var ruleType = scanner.RuleTypes[token.Key.Key];
                var rule = (IRule<T, R>)ruleType.CreateInstance();

                return new Result<T, R>((t => rule.Execute(t)), scanner);
            };
        }

        public static ParseFunc<T, R> SelectMany<T, M, R>(this ParseFunc<T, M> parser,
            Func<Func<T, M>, ParseFunc<M, R>> parserSelector, Func<Func<T, M>, Func<M, R>, Func<T, R>> resultSelector) {

            return scanner => {
                var result1 = parser(scanner);

                if(result1 == null) return null;

                var parser2 = parserSelector(result1.Value);

                var result2 = parser2(result1.Scanner);

                if(result2 == null) return null;

                return new Result<T, R>(resultSelector(result1.Value, result2.Value), result2.Scanner);
            };
        }



    }




}
