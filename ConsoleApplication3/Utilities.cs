using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public static class Utilities {
        public static Type CreateType(Type type, Type genericArgument) {
            if(genericArgument == null) throw new System.ArgumentNullException("genericArgument");
            return CreateType(type, new[] { genericArgument });
        }
        public static Type CreateType(Type type, params Type[] genericArguments) {
            if(genericArguments == null) throw new System.ArgumentNullException("genericArguments");
            if(type == null) throw new System.ArgumentNullException("type");
            var result = type.MakeGenericType(genericArguments);
            return result;
        }

        public static object CreateInstance(this Type type) {
            return CreateInstance(type, new object[0]);
        }

        public static object CreateInstance(this Type type, object constructorArg) {
            if(constructorArg == null) throw new System.ArgumentNullException("constructorArg");
            return CreateInstance(type, new object[] { constructorArg });
        }

        public static object CreateInstance(this Type type, params object[] constructorArgs) {
            if(constructorArgs == null) throw new System.ArgumentNullException("constructorArgs");
            if(type == null) throw new System.ArgumentNullException("type");
            return Activator.CreateInstance(type, constructorArgs);
        }
    }
}
