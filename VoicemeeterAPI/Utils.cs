using System;
using System.Collections.Generic;
using System.Text;

namespace VoicemeeterAPI
{
    public static class Utils
    {
        public static float Remap(float value, float fromLow, float fromHigh, float toLow, float toHigh)
        {
            if (value < fromLow) value = fromLow;
            if (value > fromHigh) value = fromHigh;

            return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        }

    }
}
