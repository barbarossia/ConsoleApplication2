using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class IninValueOnT2Add : RuleBase<Test2> {
        public override Test2 Execute(Test2 entity) {
            Console.WriteLine("Call me, IninValueOnT2Add");
            entity.Value +=1;
            return entity;
        }
    }
}
