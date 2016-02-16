using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
    public interface IResult<T, R> {
        Func<T, R> Value { get; set; }
        IRulesEngine Scanner { get; set; }

    }

    public class Result<T, R> : IResult<T, R> {
        public Func<T, R> Value { get; set; }
        public IRulesEngine Scanner { get; set; }
        public Result(Func<T, R> value, IRulesEngine scanner) {
            Value = value;
            Scanner = scanner;
        }
    }
}
