﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Generic
{
    public static class TimeOut
    {
        public static TimeSpan Indefinate { get { return new TimeSpan(0, 0, 0, 0, -1); } }
    }
}