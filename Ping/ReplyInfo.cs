using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ping
{
    internal struct ReplyInfo
    {
        public int Send { get; set; }
        public int Lost { get; set; }
        public int Received { get; set; }
        public double PrctLost => GetPrctLost();

        private double GetPrctLost()
        {
            try { return Send / Lost * 100; }
            catch { return 0; }
        }
    }
}
