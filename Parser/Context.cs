using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class Context {
        private Dictionary<string, object> items = new Dictionary<string, object>();
        public TokenBuffer TokenBuffer { get; set; }
        public Context(TokenBuffer buffer) {
            TokenBuffer = buffer;
        }
        public T Get<T>(string key) {
            if(key == null) throw new System.ArgumentNullException("key");
            object result;
            if(items.TryGetValue(key, out result)) {
                return (T)result;
            }

            return default(T);
        }

        public void Set<T>(string key, T value) {
            if(key == null) throw new System.ArgumentNullException("key");
            items[key] = value;
        }
    }
}
