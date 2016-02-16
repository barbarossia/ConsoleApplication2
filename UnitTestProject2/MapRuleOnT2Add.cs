﻿using ClassLibrary1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject2 {
    public class MapRuleOnT2Add : IMapRule<Test2, Test3> {
        public IEnumerable<Test3> Execute(Test2 t2) {
            return Enumerable.Range(1, 3)
                .Select(t => new Test3() { C = t })
                .ToList();
        }
        public string RuleKind
        {
            get
            {
                return RuleKinds.MapRule;
            }
        }
    }
}