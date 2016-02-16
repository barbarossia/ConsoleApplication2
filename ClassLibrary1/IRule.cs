using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public interface IRule<T> : IRule<T, T> {
    }
    public interface IRule {
        string RuleKind { get; }
    }
    public interface IRule<T, R> : IRule {
        R Execute(T entity);
    }
}
