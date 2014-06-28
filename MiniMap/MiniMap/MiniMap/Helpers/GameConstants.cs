using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator.Helpers
{
    class GameConstants
    {
        public const int RESOLUTION_X = 1366;
        public const int RESOLUTION_Y = 768;

        public const int MINI_MAP_LONG = 741 / 2;
        public const int MINI_MAP_SHORT = 335 / 2;

        public static TimeSpan MATCH_TIME = new TimeSpan(0, 2, 30);
        public static TimeSpan AUTO_TIME = TimeSpan.FromSeconds(10);
    }
}
