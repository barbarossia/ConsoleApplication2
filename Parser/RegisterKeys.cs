using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public interface IRegisterKeys<T> {
        string Key { get; }
    }
    public static class RegisterKeys {
        public const string Rule = "Rule";
        public const string MapRule = "MapRule";
        public const string ReduceRule = "ReduceRule";
        public const string ForEach = "ForEach";
        //public const string Reduce = "Reduce";
        //public const string MapReduce = "MapReduce";
        private struct PropertyBagKey<T> : IRegisterKeys<T> {
            public string Key { get; set; }
        }

        private static IRegisterKeys<T> Key<T>(string key) {
            var result = new PropertyBagKey<T>() { Key = key };
            return result;
        }
    }
}
