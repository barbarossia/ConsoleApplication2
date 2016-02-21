using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Lexer {
    public enum TokenType {
        MAPREDUCE,
        MAP,
        REDUCE,
        MAPRULE,
        REDUCERULE,
        FOREACH,
        EOF,
        RULE,
    }
}
