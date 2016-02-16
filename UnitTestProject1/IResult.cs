//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace UnitTestProject1 {
//    public delegate IResult<T> ParserFunc<out T>(ForkableScanner scanner);

//    public interface IResult<out T> {
//        T Value { get; }
//        ForkableScanner ReturnedScanner { get; }
//    }
//    public class Result<T> : IResult<T> {
//        public T Value { get; private set; }
//        public ForkableScanner ReturnedScanner { get; private set; }

//        public Result(T value, ForkableScanner returnedScanner) {
//            Value = value;
//            ReturnedScanner = returnedScanner;
//        }
//    }

//}
