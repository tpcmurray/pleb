using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen.Biomes
{
    public class Frozen : Biome
    {
        private readonly Terrain ice;
        private readonly Terrain snow;
        private readonly Terrain mountain;
        private readonly Terrain mountainSnow;

        public Frozen(PercRangeFloat range)
        {
            ice = new Terrain(TerrainEnum.Ice, range, 0.40f, new Color(133, 133, 197));
            snow = new Terrain(TerrainEnum.Snow, range, 0.80f, new Color(237, 242, 255));
            mountain = new Terrain(TerrainEnum.Mountain, range, 0.93f, new Color(144, 144, 144));
            mountainSnow = new Terrain(TerrainEnum.Snow, range, 1.00f, new Color(178, 216, 222));
        }

        public Terrain GetTerrain(float z)
        {
            Color c = snow.Color;
            if (z < ice.HeightLimit) {
                return ice;
            } else if (z < snow.HeightLimit) {
                return snow;
            } else if (z < mountain.HeightLimit) {
                return mountain;
            } else {
                return mountainSnow;
            }
        }
    }
}
