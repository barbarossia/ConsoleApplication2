using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class ForEachNode<R> : INode<R> {
        public INode<R> Body { get; private set; }
        public string Name
        {
            get
            {
                return "ForEachRule";
            }

        }

        public Func<R, R> Compile() {
            return Body.Compile();
        }
        public ForEachNode(INode<R> body)  {
            Body = body;
        }
    }
}
