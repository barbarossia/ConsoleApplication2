using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {
    [TestClass]
    public class UnitTest4 {
        [TestMethod]
        public void TestMethod1() {
            //Func<int, string> v = i => i.ToString();
            //var result = new Result<int>(v);
        }



        class Result<T> {
            public Func<T> Value { get; private set; }
            public Result(Func<T> value) {
                Value = value;
            }
        }
    }
}
