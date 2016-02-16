using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public interface INode {
        string Name { get; }
    }
    public interface INode<T> : INode {
        Func<T, T> Compile();
    }
    public interface INode<T, R> : INode {

        Func<T, R> Compile();
    }
    public interface IReduceNode<T, R> : INode {

        Func<T, R, R> Compile();
    }
    public interface IConditionNode : INode {
        Func<bool> Compile();
    }
    //public interface IMapReduceNode<T, R> : INode {

    //    Func<T, T> Compile(IRulesEngine engine);
    //}
}
