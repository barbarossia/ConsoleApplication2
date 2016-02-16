using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ForEachToken : Token {
        public Token Body { get; set; }
        public ForEachToken(Token body) {
            Body = body;
            SourceType = body.SourceType;
            TargetType = body.TargetType;
        }
        public override INode Accept<T,R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }
    }
}
