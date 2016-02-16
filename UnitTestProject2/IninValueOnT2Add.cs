using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject2 {
    public class IninValueOnT2Add : RuleBase<Test2> {
        public override Test2 Execute(Test2 entity) {
            entity.Value +=1;
            return entity;
        }
    }
}
