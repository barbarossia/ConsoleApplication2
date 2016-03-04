using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public static class ExpressionHelper {
        public static Expression<Func<IEnumerable<T>, TResult, TResult>> Concat<T, TResult>(this Expression<Func<IEnumerable<T>, TResult, TResult>> expr1, Expression<Func<IEnumerable<T>, TResult, TResult>> expr2) {
            ParameterExpression result = expr1.Parameters[1];
            ParameterExpression input = expr1.Parameters[0];

            ParameterExpression inputPar = Expression.Parameter(typeof(IEnumerable<T>));
            ParameterExpression inputPar2 = Expression.Parameter(typeof(TResult));

            BinaryExpression asn1 = Expression.Assign(inputPar, input);
            BinaryExpression asn2 = Expression.Assign(inputPar2, result);
            var r1 = Expression.Invoke(expr1, asn1, asn2);
            var r2 = Expression.Invoke(expr2, asn1, asn2);

            BlockExpression block = Expression.Block(
                new ParameterExpression[] { inputPar, inputPar2 },
                asn1,
                asn2,
                r1,
                r2
                );
            return Expression.Lambda<Func<IEnumerable<T>, TResult, TResult>>(block, input, result);
        }
        public static Expression<Func<T, T>> Concat<T>(this Expression<Func<T, T>> expr1, Expression<Func<T, T>> expr2) {
            ParameterExpression input = expr1.Parameters[0];

            ParameterExpression inputPar = Expression.Parameter(typeof(T));

            BinaryExpression asn1 = Expression.Assign(inputPar, input);
            var r1 = Expression.Invoke(expr1, asn1);
            var r2 = Expression.Invoke(expr2, asn1);

            BlockExpression block = Expression.Block(
                new ParameterExpression[] { inputPar },
                asn1,
                r1,
                r2
                );
            return Expression.Lambda<Func<T, T>>(block, input);
        }

        public static Expression<Func<T, IEnumerable<TResult>>> Concat<T, TResult>(this Expression<Func<T, IEnumerable<TResult>>> mapLeftExpr, Expression<Func<T, IEnumerable<TResult>>> mapRightExpr) {
            //var input = mapLeftExpr.Parameters[0];
            //var mapLeftFunc = mapLeftExpr.Compile();
            //var MaprightFunc = mapRightExpr.Compile();
            //Expression<Func<T, IEnumerable<TResult>>> block = (l) => mapLeftFunc(l).Concat(MaprightFunc(l));
            var input = mapLeftExpr.Parameters[0];
            LabelTarget labelTarget = Expression.Label(typeof(IEnumerable<TResult>));
            var loc = Expression.Variable(typeof(IEnumerable<TResult>));
            List<TResult> list = new List<TResult>();
            var para1 = Expression.Parameter(typeof(T));
            var asn = Expression.Assign(para1, input);
            Expression<Action<IEnumerable<TResult>>> expr = (l) => list.AddRange(l);
            var r2 = Expression.Invoke(expr, Expression.Invoke(mapLeftExpr, asn));
            var r4 = Expression.Invoke(expr, Expression.Invoke(mapRightExpr, asn));
            Expression<Func<IEnumerable<TResult>>> assing1 = () => list;
            var i = Expression.Invoke(assing1);
            var asn1 = Expression.Assign(loc, i);
            GotoExpression ret = Expression.Return(labelTarget, asn1);
            LabelExpression lbl = Expression.Label(labelTarget, Expression.Constant(new List<TResult>()));
            BlockExpression block = Expression.Block(
                new ParameterExpression[] { loc, para1 },
                asn,
                r2,
                r4,
                assing1,
                i,
                asn1,
                ret,
                lbl
                );
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(block, input);
        }

        public static Expression<Func<T, IEnumerable<TResult>>> Concat<T, TResult>(this Expression<Func<T, T>> expr1, Expression<Func<T, IEnumerable<TResult>>> expr2) {
            var input = expr1.Parameters[0];
            var invoker = Expression.Invoke(expr2, Expression.Invoke(expr1, input));

            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(invoker, input);
        }

        public static Expression<Func<T, T>> Concat<T, TResult>(this Expression<Func<T, IEnumerable<TResult>>> map, Expression<Func<IEnumerable<TResult>, T, T>> reduce) {
            var input = map.Parameters[0];
            var invoker = Expression.Invoke(reduce, Expression.Invoke(map, input), input);

            return Expression.Lambda<Func<T, T>>(invoker, input);
        }

        public static Expression<Func<IEnumerable<T>, IEnumerable<TResult>>> Enumerate<T, TResult>(this Expression<Func<T, TResult>> expr) {
            var func = expr.Compile();
            Expression<Func<IEnumerable<T>, IEnumerable<TResult>>> expr1 = (list) => list.Select(l => func(l));
            return expr1;
        }

        public static Expression<Func<T, IEnumerable<TResult>>> Concat<T, TMid, TResult>(this Expression<Func<T, IEnumerable<TMid>>> expr1, Expression<Func<IEnumerable<TMid>, IEnumerable<TResult>>> expr2) {
            var input = expr1.Parameters[0];
            var invoker = Expression.Invoke(expr2, Expression.Invoke(expr1, input));
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(invoker, input);
        }
    }
}
