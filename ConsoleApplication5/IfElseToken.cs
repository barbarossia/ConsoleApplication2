using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class IfElseToken : Token {
        public Token IfPart { get; set; }
        public Token ElsePart { get; set; }
        public ConditionToken Condition { get; set; }
        public override INode Accept<T, R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }
        public IfElseToken(ConditionToken condition, Token ifPart, Token elsePart) {
            Condition = condition;
            IfPart = ifPart;
            ElsePart = elsePart;
            SourceType = ifPart.SourceType;
            TargetType = ifPart.TargetType;
        }
    }
}
