﻿using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
    public static class Expression2 {
        public static Expression<Func<T, TResult, TResult>> Create<T, TResult>(this Expression<Func<T, TResult, TResult>> expr1, Expression<Func<T, TResult, TResult>> expr2) {
            ParameterExpression result = expr1.Parameters[1];
            ParameterExpression input = expr1.Parameters[0];

            ParameterExpression inputPar = Expression.Parameter(typeof(T));
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
            return Expression.Lambda<Func<T, TResult, TResult>>(block, input, result);
        }

        public static Expression<Func<T, IEnumerable<TResult>>> AddRange<T, TResult>(this Expression<Func<T, IEnumerable<TResult>>> expr1, Expression<Func<T, IEnumerable<TResult>>> expr2) where TResult : new() {
            var input = expr1.Parameters[0];
            LabelTarget labelTarget = Expression.Label(typeof(IEnumerable<TResult>));
            var loc = Expression.Variable(typeof(IEnumerable<TResult>));
            List<TResult> list = new List<TResult>();
            var para1 = Expression.Parameter(typeof(T));
            var asn = Expression.Assign(para1, input);
            Expression<Action<IEnumerable<TResult>>> expr = (l) => list.AddRange(l);
            var r2 = Expression.Invoke(expr, Expression.Invoke(expr1, asn));
            var r4 = Expression.Invoke(expr, Expression.Invoke(expr2, asn));
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

        public static Expression<Func<IEnumerable<T>, IEnumerable<TResult>>> Enumerate<T, TResult>(this Expression<Func<T, TResult>> expr) {
            var list = Expression.Parameter(typeof(IEnumerable<T>));
            var res = Expression.Call(typeof(Enumerable), "Select", new Type[] { typeof(T), typeof(TResult)}, list, expr);
            return Expression.Lambda<Func<IEnumerable<T>, IEnumerable<TResult>>>(res, list);
        }

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
            var r2 = Expression.New(ctor, Expression.Invoke(map, asn));
            var ass = Expression.Assign(list, r2);
            var r3 = Expression.New(ctor2, Expression.Invoke(forach, list));
            var i = Expression.Assign(list2, r3);
            var asn1 = Expression.Assign(loc, i);


            GotoExpression ret = Expression.Return(labelTarget, asn1);
            LabelExpression lbl = Expression.Label(labelTarget, Expression.Constant(new List<TResult>()));
            BlockExpression block = Expression.Block(
                new ParameterExpression[] { loc, para1, list, list2 },
                asn,
                list,
                list2,
                r2,
                ass,
                r3,
                i,
                asn1,
                ret,
                lbl
                );
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(block, input);
        }
    }

}
