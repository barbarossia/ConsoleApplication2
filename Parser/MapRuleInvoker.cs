using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class MapRuleInvoker<T, TResult> : IInvoker {
        public LambdaExpression Invoke(Type ruleType) {
            var rule = (IMapRule<T, TResult>)ruleType.CreateInstance();
            var para1 = Expression.Parameter(typeof(T));
            Expression<Func<T, IEnumerable<TResult>>> n = (t) => rule.Execute(t);
            var i = Expression.Invoke(n, para1);
            return Expression.Lambda<Func<T, IEnumerable<TResult>>>(i, para1);
        }
    }
    public abstract class GroupInvokerBase : IGroupInvoker {
        public GroupInvokerBase(IEnumerable<LambdaExpression> exprs) {
            Exprs = exprs;
        }

        public IEnumerable<LambdaExpression> Exprs { get; private set; }
        public abstract LambdaExpression Invoke();
    }
    public class MapGroupInvoker<T, TResult> : GroupInvokerBase {
        public MapGroupInvoker(IEnumerable<LambdaExpression> exprs) : base(exprs) {
        }

        public override LambdaExpression Invoke() {
            var converted = Exprs.Select(e => (Expression<Func<T, IEnumerable<TResult>>>)e);
            var result = converted.Aggregate((curr, next) => curr.Concat(next));
            return result;
        }
    }
    public class InitMapInvoker<T, TResult> : IGroupInvoker {
        private Expression<Func<T, T>> _init;
        private Expression<Func<T, IEnumerable<TResult>>> _map;


        public InitMapInvoker(LambdaExpression init, LambdaExpression map)  {
            _init = (Expression<Func<T, T>>)init;
            _map = (Expression<Func<T, IEnumerable<TResult>>>)map;
        }

        public LambdaExpression Invoke() {
            return _init.Concat(_map);
        }
    }
    public class MapReduceInvoker<T, TResult> : IGroupInvoker {
        private Expression<Func<T, IEnumerable<TResult>>> _map;
        private Expression<Func<IEnumerable<TResult>, T, T>> _reduce;


        public MapReduceInvoker(LambdaExpression map, LambdaExpression reduce) {
            _map = (Expression<Func<T, IEnumerable<TResult>>>)map;
            _reduce = (Expression<Func<IEnumerable<TResult>, T, T>>)reduce;
        }

        public LambdaExpression Invoke() {
            return _map.Concat(_reduce);
        }
    }
}
