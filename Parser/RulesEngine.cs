using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class RulesEngine {
        private Parser _parser;
        public RulesEngine(Parser parser) {
            _parser = parser;
        }
        public Func<T, T> Build<T>() {
            var result = _parser.Result;
            if(result.IsSuccessed) {
                var resultFunc = (Expression<Func<T, T>>)result.Expression;
                return resultFunc.Compile();
            }
            throw new ApplicationException("");
        }
    }
}
