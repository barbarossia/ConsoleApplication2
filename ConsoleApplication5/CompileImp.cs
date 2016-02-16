using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication5 {
    public class CompileImp<T, R> : Compiler<T, R> {
        public override INode<T, IEnumerable<R>> Compile(MapToken token) {
            var result = (MapNode<T, R>)Utilities.CreateType(typeof(MapNode<,>), token.SourceType, token.TargetType)
                                        .CreateInstance();

            foreach(var t in token.InnerToken) {
                var child = t.Accept(this);
                result.SetRule(child);
            }
            return result;
        }

        public override INode<R> Compile(ForEachToken token) {
            var bodyToken = token.Body;
            var compile = (Compiler<R, R>)Utilities.CreateType(typeof(CompileImp<,>), token.SourceType, token.SourceType)
                            .CreateInstance();
            var body = (INode<R>)bodyToken.Accept(compile);
            return (ForEachNode<R>)Utilities.CreateType(typeof(ForEachNode<>), token.SourceType)
                            .CreateInstance(body);
        }

        public override IReduceNode<IEnumerable<T>, R> Compile(ReduceRuleToken token) {
            return (IReduceNode<IEnumerable<T>, R>)Utilities.CreateType(typeof(ReduceRuleNode<,>), token.SourceType, token.TargetType)
                .CreateInstance(token.RuleType);
        }

        //public override INode Compile(ConditionToken token) {
        //    return new ConditionNode(token.RuleType);
        //}

        //public override INode<T, R> Compile(IfElseToken token) {
        //    IConditionNode condition = (IConditionNode)token.Condition.Accept(this);
        //    var compile = (Compiler<T, R>)Utilities.CreateType(typeof(CompileImp<,>), typeof(T), typeof(R))
        //                   .CreateInstance();
        //    INode ifPart = token.IfPart.Accept(compile);

        //    INode elsePart = null;
        //    if (token.ElsePart != null) {
        //        elsePart = token.ElsePart.Accept(compile);
        //    }
        //    return (INode<T, R>)Utilities.CreateType(typeof(IfElseNode<,>), typeof(T), typeof(R))
        //       .CreateInstance(condition, ifPart, elsePart);
        //}

        public override IReduceNode<IEnumerable<T>, R> Compile(ReduceToken token) {
            List<IReduceNode<IEnumerable<T>, R>> list = new List<IReduceNode<IEnumerable<T>, R>>();
            foreach(var c in token.InnerToken)
                list.Add((IReduceNode<IEnumerable<T>, R>)c.Accept(this));
            return (IReduceNode<IEnumerable<T>, R>)Utilities.CreateType(typeof(ReduceNode<,>), token.SourceType, token.TargetType)
                .CreateInstance(list);
        }
        public override INode<T, IEnumerable<R>> Compile(MapRuleToken token) {
            return (INode<T, IEnumerable<R>>)Utilities.CreateType(typeof(MapRuleNode<,>), token.SourceType, token.TargetType)
                            .CreateInstance(token.RuleType);
        }

        public override INode<T> Compile(RuleToken token) {
            return (INode<T>)Utilities.CreateType(typeof(RuleNode<>), token.SourceType)
                                        .CreateInstance(token.RuleType);
        }
        public override INode<T> Compile(MapReduceToken token) {
            return (INode<T>)Utilities.CreateType(typeof(MapReduceNode<, >), token.SourceType, token.MidType)
                            .CreateInstance(token.MapPart, token.ReducePart);
        }
    }
}
