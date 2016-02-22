using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Lexer {
    public static class Patterns {
        public static Pattern Rule = new Pattern(@"^Rule$", TokenType.RULE);
        public static Pattern MapRule = new Pattern("^MapRule$", TokenType.MAPRULE);
        public static Pattern ReduceRule = new Pattern("^ReduceRule$", TokenType.REDUCERULE);
        public static Pattern Map = new Pattern("^Map$", TokenType.MAP);
        public static Pattern Reduce = new Pattern("^Reduce$", TokenType.REDUCE);
        public static Pattern MapReduce = new Pattern("^MapReduce$", TokenType.MAPREDUCE);
        public static Pattern ForEach = new Pattern("^ForEach$", TokenType.FOREACH);


    }
}
