using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1 {
    public static class DelegateHelper {
        const string InvokeMethod = "Invoke";
        public static MethodInfo MethodInfoFromDelegateType(Type delegateType) {
            //Contract.Requires<ArgumentException>(
            //    delegateType.IsSubclassOf(typeof(MulticastDelegate)),
            //    "Given type should be a delegate.");

            return delegateType.GetMethod(InvokeMethod);
        }
        public static Expression<T> CreateCompatibleDelegate<T>(object instance, MethodInfo method) {
            MethodInfo delegateInfo = MethodInfoFromDelegateType(typeof(T));

            var methodTypes = method.GetParameters().Select(m => m.ParameterType);
            var delegateTypes = delegateInfo.GetParameters().Select(d => d.ParameterType);

            // Convert the arguments from the delegate argument type
            // to the method argument type when necessary.
            var arguments = methodTypes.Zip(delegateTypes, (methodType, delegateType) =>
            {
                ParameterExpression delegateArgument = Expression.Parameter(delegateType);
                return new {
                    DelegateArgument = delegateArgument,
                    ConvertedArgument = methodType != delegateType
                                    ? (Expression)Expression.Convert(delegateArgument, methodType)
                                    : delegateArgument
                };
            }).ToArray();

            // Create method call.;
            MethodCallExpression methodCall = Expression.Call(
                instance == null ? null : Expression.Constant(instance),
                method,
                arguments.Select(a => a.ConvertedArgument)
                );

            // Convert return type when necessary.
            Expression convertedMethodCall = delegateInfo.ReturnType == method.ReturnType
                                        ? (Expression)methodCall
                                        : Expression.Convert(methodCall, delegateInfo.ReturnType);

            //return Expression.Lambda<T>(
            //    convertedMethodCall,
            //    arguments.Select(a => a.DelegateArgument)
            //    ).Compile();
            return Expression.Lambda<T>(
                convertedMethodCall,
                arguments.Select(a => a.DelegateArgument)
                );
        }
    }
}
