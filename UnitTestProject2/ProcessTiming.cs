using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapReduce.Parser.UnitTest {
    public class ProcessTiming {

        public static double DateDiff(string howtocompare, System.DateTime startDate, System.DateTime endDate) {
            double diff = 0;
            try {
                System.TimeSpan TS = new System.TimeSpan(startDate.Ticks - endDate.Ticks);
                #region conversion options
                switch(howtocompare.ToLower()) {
                    case "m":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "s":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                    case "t":
                        diff = Convert.ToDouble(TS.Ticks);
                        break;
                    case "mm":
                        diff = Convert.ToDouble(TS.TotalMilliseconds);
                        break;
                    case "yyyy":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "q":
                        diff = Convert.ToDouble((TS.TotalDays / 365) / 4);
                        break;
                    default:
                        //d
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                }
                #endregion
            } catch {
                diff = -1;
            }
            return diff;
        }

    }
}
