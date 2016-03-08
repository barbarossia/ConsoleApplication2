﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public class IninValueOnT1 : RuleBase<Test1> {
        public override Test1 Execute(Test1 entity) {
            //Console.WriteLine("Call me, IninValueOnT1");
            entity.Value = 1;
            return entity;
        }
    }

    public class IninValueOnT2 : RuleBase<Test2> {
        public override Test2 Execute(Test2 entity) {
            //Console.WriteLine("Call me, IninValueOnT2");
            entity.Value = 2;
            return entity;
        }
    }

    public class IninValueOnT3 : RuleBase<Test3> {
        public override Test3 Execute(Test3 entity) {
            //Console.WriteLine("Call me, IninValueOnT3");
            entity.Value = 1;
            return entity;
        }
    }
}
