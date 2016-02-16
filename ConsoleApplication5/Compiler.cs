using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    //public interface Compiler<T, R> compiler {
    //    INode<T, IEnumerable<R>> Compile(MapRuleToken token);
    //    INode<T, IEnumerable<R>> Compile(MapToken token);
    //    INode<T, R> Compile(ReduceRuleToken token);
    //    INode<R> Compile(ForEachToken token);
    //    INode<T> Compile(RuleToken token);
    //    INode Compile(Token token);
    //}
    public abstract class Compiler<T, R>  {
        public abstract INode<T> Compile(RuleToken token);
        //public abstract INode Compile(ConditionToken token);
        public abstract INode<T, IEnumerable<R>> Compile(MapRuleToken token);
        public abstract INode<T, IEnumerable<R>> Compile(MapToken token);
        public abstract IReduceNode<IEnumerable<T>, R> Compile(ReduceRuleToken token);
        public abstract IReduceNode<IEnumerable<T>, R> Compile(ReduceToken token);
        //public abstract INode<T, R> Compile(IfElseToken token);
        public abstract INode<R> Compile(ForEachToken token);
        public abstract INode<T> Compile(MapReduceToken token);
        public INode Compile(Token token) {
            return token.Accept(this);
        }

    }
}
