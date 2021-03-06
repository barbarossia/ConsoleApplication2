﻿using System;
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
        /// <summary>
        /// concat map and map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="mapLeftExpr"></param>
        /// <param name="mapRightExpr"></param>
        /// <returns></returns>
        public static Expression<Func<T, IEnumerable<TResult>>> Concat<T, TResult>(this Expression<Func<T, IEnumerable<TResult>>> mapLeftExpr, Expression<Func<T, IEnumerable<TResult>>> mapRightExpr) {
            var input = mapLeftExpr.Parameters[0];
            LabelTarget labelTarget = Expression.Label(typeof(IEnumerable<TResult>));
            var loc = Expression.Variable(typeof(IEnumerable<TResult>));
            var list2 = Expression.Variable(typeof(List<TResult>));
            var para1 = Expression.Parameter(typeof(T));
            var asn = Expression.Assign(para1, input);

            var ctor2 = typeof(List<TResult>).GetConstructor(new Type[] { typeof(IEnumerable<TResult>) });
            var ass = Expression.Assign(list2, Expression.New(ctor2, Expression.Invoke(mapLeftExpr, asn)));
            var call = Expression.Call(list2, typeof(List<TResult>).GetMethod("AddRange"), Expression.Invoke(mapRightExpr, asn));
            var asn1 = Expression.Assign(loc, list2);
            GotoExpression ret = Expression.Return(labelTarget, asn1);
            LabelExpression lbl = Expression.Label(labelTarget, Expression.Constant(new List<TResult>()));
            BlockExpression block = Expression.Block(
                new ParameterExpression[] { loc, para1, list2 },
                list2,
                asn,
                ass,
                call,
                asn1,
                ret,
                lbl
                );
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(block, input);
        }
        /// <summary>
        /// concat rule and map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, IEnumerable<TResult>>> Concat<T, TResult>(this Expression<Func<T, T>> expr1, Expression<Func<T, IEnumerable<TResult>>> expr2) {
            var input = expr1.Parameters[0];
            var invoker = Expression.Invoke(expr2, Expression.Invoke(expr1, input));

            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(invoker, input);
        }
        /// <summary>
        /// concat map and reduce
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="map"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        public static Expression<Func<T, T>> Concat<T, TResult>(this Expression<Func<T, IEnumerable<TResult>>> map, Expression<Func<IEnumerable<TResult>, T, T>> reduce) {
            var input = map.Parameters[0];
            var invoker = Expression.Invoke(reduce, Expression.Invoke(map, input), input);

            return Expression.Lambda<Func<T, T>>(invoker, input);
        }

        public static Expression<Func<IEnumerable<T>, IEnumerable<TResult>>> Enumerate<T, TResult>(this Expression<Func<T, TResult>> expr) {
            var list = Expression.Parameter(typeof(IEnumerable<T>));
            var res = Expression.Call(typeof(Enumerable), "Select", new Type[] { typeof(T), typeof(TResult) }, list, expr);
            return Expression.Lambda<Func<IEnumerable<T>, IEnumerable<TResult>>>(res, list);
        }
        /// <summary>
        /// concat map and foreach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMid"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="map"></param>
        /// <param name="forach"></param>
        /// <returns></returns>
        public static Expression<Func<T, IEnumerable<TResult>>> Concat<T, TMid, TResult>(this Expression<Func<T, IEnumerable<TMid>>> map, Expression<Func<IEnumerable<TMid>, IEnumerable<TResult>>> forach) {
            var input = map.Parameters[0];
            LabelTarget labelTarget = Expression.Label(typeof(IEnumerable<TResult>));
            var loc = Expression.Variable(typeof(IEnumerable<TResult>));
            var para1 = Expression.Parameter(typeof(T));
            var asn = Expression.Assign(para1, input);
            var list = Expression.Variable(typeof(List<TMid>));
            var list2 = Expression.Variable(typeof(List<TResult>));

            var ctor = typeof(List<TMid>).GetConstructor(new Type[] { typeof(IEnumerable<TMid>) });
            var ctor2 = typeof(List<TResult>).GetConstructor(new Type[] { typeof(IEnumerable<TResult>) });
            var ass = Expression.Assign(list, Expression.New(ctor, Expression.Invoke(map, asn)));
            var i = Expression.Assign(list2, Expression.New(ctor2, Expression.Invoke(forach, list)));

            BlockExpression block = Expression.Block(
                new ParameterExpression[] { loc, para1, list, list2 },
                asn,
                ass,
                i);
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(block, input);
        }
    }
}
