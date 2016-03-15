using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class TestConditionTrue : RuleBase<Test1> {
        public override Test1 Execute(Test1 entity) {
            Console.WriteLine("TestConditionTrue{0}", Context.InnerContext.RandomNum);
            return entity;
        }
    }

    public class TestConditionFalse : RuleBase<Test1> {
        public override Test1 Execute(Test1 entity) {
            Console.WriteLine("TestConditionFalse{0}", Context.InnerContext.RandomNum);
            return entity;
        }
    }
}
