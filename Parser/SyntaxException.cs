﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser {
    public class SyntaxException : Exception {
        public SyntaxException(string message) : base(message) {
        }
    }

}
