using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class Context {
        private Dictionary<string, object> items = new Dictionary<string, object>();
        public Dictionary<string, object> Items { get { return items; } }
    }
}
