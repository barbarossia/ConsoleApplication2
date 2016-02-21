using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MapReduce.Parser {
    public class ParserResult {
        public bool IsSuccessed { get; set; }
        public LambdaExpression Expression { get; set; }
        public TokenInfo Token { get; set; }
        public SyntaxException Error { get; set; }
        public ParserResult(TokenInfo token, bool isSuccessed, LambdaExpression expr, SyntaxException error = null) {
            IsSuccessed = isSuccessed;
            Expression = expr;
            Token = token;
            Error = error;
        }
    }
}
