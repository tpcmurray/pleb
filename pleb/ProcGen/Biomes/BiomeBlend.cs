using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen.Biomes
{
    public class BiomeBlend
    {
        public BiomeBlend(float blend, int blendTop, int blendBottom, float split)
        {
            Blend = blend;
            BlendTop = blendTop;
            BlendBottom = blendBottom;
            Split = split;
        }

        public float Blend { get; }
        public int BlendTop { get; }
        public int BlendBottom { get; }
        public float Split { get; }
    }
}
