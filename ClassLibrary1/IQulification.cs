using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public interface IQulification {
        bool IsQualified();
    }

    public class ConditionRuleOnT1 : IQulification {
        public bool IsQualified() {
            return true;
        }
    }
}
