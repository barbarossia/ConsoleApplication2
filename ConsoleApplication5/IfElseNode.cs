using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class IfElseNode<T, R> : INode<T, R> {
        private string realName;
        public INode IfPart { get; private set; }
        public INode ElsePart { get; private set; }
        public IConditionNode Condition { get; private set; }
        public string Name
        {
            get
            {
                return realName;
            }
        }

        public Func<T, R> Compile() {
            var f1 = Condition.Compile();
            if (f1()) {
                realName = IfPart.Name;
                return ((INode<T, R>)IfPart).Compile();
            }
            return null;
        }
        //public IfElseNode(IConditionNode condition, Token ifPart, Token elsePart) {
        //    Condition = condition;

        //    var compile = (Compiler<T, R>)Utilities.CreateType(typeof(CompileImp<,>), ifPart.SourceType, ifPart.TargetType)
        //                   .CreateInstance();
        //    IfPart = (INode<T, R>)ifPart.Accept(compile);
        //     = ifPart;
        //    ElsePart = elsePart;
        //}

        public IfElseNode(IConditionNode condition, INode ifPart, INode elsePart) {
            Condition = condition;
            IfPart = ifPart;
            ElsePart = elsePart;
        }
    }
}
