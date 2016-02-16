using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public delegate IResult<T, R> ParseFunc<T, R>(IRulesEngine scanner); 
}
