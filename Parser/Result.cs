using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parser {
    public class Result {
        public bool IsSuccessed { get; set; }
        public LambdaExpression Expression { get; set; }
        public Token Token { get; set; }
        public SyntaxException Error { get; set; }
        public Result(Token token, bool isSuccessed, LambdaExpression expr, SyntaxException error = null) {
            IsSuccessed = isSuccessed;
            Expression = expr;
            Token = token;
            Error = error;
        }
    }
}
