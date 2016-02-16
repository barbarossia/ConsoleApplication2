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
        public XElement TokenBuffer { get; set; }
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public Result(XElement tokenBuffer, bool isSuccessed, LambdaExpression expr, Type sourceType =  null, Type targetType = null) {
            IsSuccessed = isSuccessed;
            Expression = expr;
            TokenBuffer = tokenBuffer;
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}
