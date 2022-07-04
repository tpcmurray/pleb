using MiscUtil;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen
{
    public class PercRangeFloat
    {
        public PercRangeFloat(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float GetValueFromPercent(float percent)
        {
            return ((Max - Min) * percent) + Min;
        }

        public float Min { get; }
        public float Max { get; }
    }

    public class PercRangeUshort
    {
        public PercRangeUshort(ushort min, ushort max)
        {
            Min = min;
            Max = max;
        }

        public bool IsLocationWithinPercent(ushort value, float perc)
        {
            double valueLocation = (double)(value - Min) / (double)(Max - Min);
            return valueLocation <= perc;
        }

        public ushort Min { get; }
        public ushort Max { get; }
    }
}
