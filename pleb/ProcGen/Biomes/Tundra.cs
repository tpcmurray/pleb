using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen.Biomes
{
    public class Tundra : Biome
    {
        private readonly Terrain ocean;
        private readonly Terrain shallows;
        private readonly Terrain shoreLine;
        private readonly Terrain tundra;
        private readonly Terrain mountainTrees;
        private readonly Terrain mountain;
        private readonly Terrain snow; // mountain tops

        public Tundra(PercRangeFloat range)
        {
            ocean = new Terrain(TerrainEnum.Ocean, range, 0.36f, new Color(46, 46, 166));
            shallows = new Terrain(TerrainEnum.Shallows, range, 0.40f, new Color(51, 104, 199));
            shoreLine = new Terrain(TerrainEnum.Shoreline, range, 0.42f, new Color(125, 115, 87));
            tundra = new Terrain(TerrainEnum.Tundra, range, 0.70f, new Color(139, 158, 105));
            mountainTrees = new Terrain(TerrainEnum.MountainTrees, range, 0.80f, new Color(102, 119, 96));
            mountain = new Terrain(TerrainEnum.Mountain, range, 0.93f, new Color(104, 92, 79));
            snow = new Terrain(TerrainEnum.Snow, range, 1.00f, new Color(178, 216, 222));
        }

        public Terrain GetTerrain(float z)
        {
            Color c = snow.Color;
            if (z < ocean.HeightLimit) {
                return ocean;
            } else if (z < shallows.HeightLimit) {
                return shallows;
            } else if (z < shoreLine.HeightLimit) {
                return shoreLine;
            } else if (z < tundra.HeightLimit) {
                return tundra;
            } else if (z < mountainTrees.HeightLimit) {
                return mountainTrees;
            } else if (z < mountain.HeightLimit) {
                return mountain;
            } else {
                return snow;
            }
        }
    }
}
