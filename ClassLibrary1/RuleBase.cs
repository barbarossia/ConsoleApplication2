using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public abstract class RuleBase<T> : IRule<T> {
        public string RuleKind
        {
            get
            {
                return RuleKinds.Rule;
            }
        }
        public abstract T Execute(T entity);
    }
}
