using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public abstract class Token {
        public Type SourceType { get; set; }
        public Type TargetType { get; set; }
        public abstract INode Accept<T, R>(Compiler<T, R> compiler);
    }
}
