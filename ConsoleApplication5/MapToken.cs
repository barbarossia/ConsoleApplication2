using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class MapToken : Token {
        private List<Token> _innerToken;
        private IReadOnlyList<Token> _readOnlyToken;
        public IReadOnlyList<Token> InnerToken { get { return _readOnlyToken; } }
        public MapToken(IEnumerable<Token> list) {
            _innerToken = new List<Token>(list);
            _readOnlyToken = _innerToken.AsReadOnly();

            this.SourceType = _innerToken.First().SourceType;
            this.TargetType = _innerToken.Last().TargetType;
        }
        public override INode Accept<T, R>(Compiler<T, R> compiler) {
            return compiler.Compile(this);
        }
    }
}
