using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public interface IReduceRule<T, TResult> { 
        TResult Execute(IEnumerable<T> list, TResult result);
    }
}
