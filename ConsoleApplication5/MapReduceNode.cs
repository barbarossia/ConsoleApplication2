using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class MapReduceNode<T, R> : INode<T> {
        public INode<T, IEnumerable<R>> Map { get; private set; }
        public IReduceNode<IEnumerable<R>, T> Reduce { get; private set; }
        public string Name
        {
            get
            {
                return "Rule";
            }
        }

        public Func<T, T> Compile() {
            Func<T, IEnumerable<R>> mapFunc = Map.Compile();
            Func<IEnumerable<R>, T, T> reduceFunc = Reduce.Compile();
            return mapFunc.Reduce(reduceFunc);
        }
        public MapReduceNode(Token map, Token reduce) {
            var compile = (Compiler<T, R>)Utilities.CreateType(typeof(CompileImp<,>), map.SourceType, map.TargetType)
                .CreateInstance();
            Map = (INode<T, IEnumerable<R>>)map.Accept(compile);

            var reduceCompile = (Compiler<R, T>)Utilities.CreateType(typeof(CompileImp<,>), reduce.SourceType, reduce.TargetType)
                .CreateInstance();

            Reduce = (IReduceNode<IEnumerable<R>, T>)reduce.Accept(reduceCompile);
        }
    }
}
