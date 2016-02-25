using MapReduce.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class TokenBuffer {
        private List<Token> items;
        private int index = 0;
        private Stack<int> snapshot =new Stack<int>();
        public Token Current {
            get {
                if (IsEnd()) {
                    return null;
                }
                return items[index];
            }
        }
        public void Consume() {
            index++;
        }
        public void Rollback() {
            index = snapshot.Pop();
        }
        public Token Backup() {
            snapshot.Push(index);
            return Current;
        }
        public void Commit() {
            snapshot.Pop();
        }
        public bool IsEnd() {
            return index == items.Count;
        }
        public TokenBuffer(IEnumerable<Token> tokens) {
            items = new List<Token>(tokens);
        }
    }
}
