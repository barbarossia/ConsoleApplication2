using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
    public class Expression1 {
        public Expression<Func<T, T>> Create<T>(Type ruleType) {
            //var method = ruleType.GetMethod("Execute");
            //var par1 = Expression.Parameter(typeof(T));
            //var instance = Expression.New(ruleType);
            //var inExp = Expression.Call(instance, method, par1);
            //return Expression.Lambda<Func<T,T>>(inExp, par1);
            var rule = (IRule<T>)ruleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, T>> n = t => rule.Execute(t);
            var i = Expression.Invoke(n, para1);
            return Expression.Lambda<Func<T, T>>(i, para1);
        }
        public LambdaExpression Create2(Type ruleType) {
            var method = ruleType.GetMethod("Execute");
            var par1 = Expression.Parameter(method.GetParameters()[0].ParameterType);
            var instance = Expression.New(ruleType);
            var inExp = Expression.Call(instance, method, par1);
            return Expression.Lambda(inExp, par1);
        }

    }
}
