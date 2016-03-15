using MapReduce.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public sealed class EngineConext {
        private static volatile EngineConext _instance = null;
        private static Object _lock = new Object();
        private static RulesEngine _engine;
        private static InnerContext _ctx;
        private EngineConext() {
            InitCache();
        }
        public static void SetContext(InnerContext inner) {
            _ctx = inner;
        }
        public static EngineConext Current
        {
            get
            {
                if(null == _instance || _ctx.RandomNum == 50) {
                    lock (_lock) {
                        if(null == _instance || _ctx.RandomNum == 50) {
                            _instance = new EngineConext();
                        }
                    }
                }
                return _instance;
            }
        }
        public RulesEngine Engine
        {
            get
            {
                return _engine;
            }
        }
        private static void InitCache() {
            string configKey = @"TestRule3.Config";
            RulesEngineConfigurationXmlProvider provider = new RulesEngineConfigurationXmlProvider(configKey);
            _engine = provider.GetRulesEngine();
        }
    }

    public sealed class Context {

        [ThreadStatic]
        private static InnerContext _ctx;
        private static Object _lock = new Object();
        public static EngineConext EngineConext
        {
            get
            {
                return EngineConext.Current;
            }
        }
        public static InnerContext InnerContext
        {
            get
            {
                return _ctx;
            }
            set
            {
                _ctx = value;
                EngineConext.SetContext(_ctx);
            }
        }

    }
}
