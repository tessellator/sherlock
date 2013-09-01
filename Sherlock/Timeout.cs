using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
    internal static class TimeOut
    {
        public static TimeSpan Indefinite { get { return new TimeSpan(0, 0, 0, 0, -1); } }
    }
}
