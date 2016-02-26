using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser {
    public class RuleConfigurationException : ApplicationException {
        public RuleConfigurationException() : base() { }
        public RuleConfigurationException(string Message) : base(Message) { }
        public RuleConfigurationException(string Message, Exception Inner) : base(Message, Inner) { }

    }
}
