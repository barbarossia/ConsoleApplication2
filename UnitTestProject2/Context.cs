using MapReduce.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class Context {
        private static Context _instance = null;
        private static Object _lock = new Object();
        private RulesEngine _engine;
        private static readonly ICustomCache _cache = new CustomCache();
        public Context() {

        }

        public static Context Current
        {
            get
            {
                if (null == _instance) {
                    lock (_lock) {
                        if(null == _instance) {
                            string cacheKey = "CAPContext";
                            if(_cache.IsCached(cacheKey)) {
                                _instance = _cache.Load<Context>(cacheKey);
                            } else {
                                _instance = Create();
                            }
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
            set
            {
                _engine = value;
            }
        }
        public static void SetEngine(RulesEngine engine) {
            //check if context exists
            Current.Engine = engine;
        }
        public static Context Create() {
            var ctx = new Context();
            string cacheKey = "CAPContext";
            _cache.Insert(cacheKey, ctx);
            return ctx;
        }
    }
}
