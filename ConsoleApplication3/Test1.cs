using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {

    public class Test1 {
        public int A { get; set; }
        public int Result { get; set; }
        public int Value { get; set; }

        public IEnumerable<Test2> Details { get; set; }


    }

    public class Test2  {

        public int B { get; set; }
        public int Result { get; set; }
        public int Value { get; set; }
        public IEnumerable<Test3> Details { get; set; }


    }

    public class Test3  {
        public int C { get; set; }
        public int Value { get; set; }
    }
}
