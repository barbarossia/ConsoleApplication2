using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public sealed class InnerContext {
        public int RandomNum { get; set; }
        public InnerContext(int rand) {
            RandomNum = rand;
        }
    }
}
