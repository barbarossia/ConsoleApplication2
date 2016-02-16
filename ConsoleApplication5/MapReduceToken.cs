using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class MapReduceToken : Token {
        public Token MapPart { get; set; }
        public Token ReducePart { get; set; }
        public Type MidType { get; set; }
        public override INode Accept<T, R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }
        public MapReduceToken(Token map, Token reduce) {
            MapPart = map;
            ReducePart = reduce;
            SourceType = TargetType = map.SourceType;
            MidType = map.TargetType;
        }
    }
}
