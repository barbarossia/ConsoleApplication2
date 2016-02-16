using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1 {
    public interface ICondition {
        bool Is();
    }

    public class ConditionRuleOnT1 : ICondition {
        public bool Is() {
            return true;
        }
    }
}
