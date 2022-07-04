using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen.Biomes
{
    public class Jungle : Biome
    {
        private readonly Terrain ocean;
        private readonly Terrain shallows;
        private readonly Terrain shoreLine;
        private readonly Terrain grass;
        private readonly Terrain jungle;
        private readonly Terrain mountainTrees;
        private readonly Terrain mountain;
        private readonly Terrain snow; // mountain tops

        public Jungle(PercRangeFloat range)
        {
            ocean = new Terrain(TerrainEnum.Ocean, range, 0.36f, new Color(32, 172, 255));
            shallows = new Terrain(TerrainEnum.Shallows, range, 0.40f, new Color(65, 227, 255));
            shoreLine = new Terrain(TerrainEnum.Shoreline, range, 0.42f, new Color(250, 246, 243));
            grass = new Terrain(TerrainEnum.Grass, range, 0.50f, new Color(18, 188, 0));
            jungle = new Terrain(TerrainEnum.Jungle, range, 0.70f, new Color(36, 144, 65));
            mountainTrees = new Terrain(TerrainEnum.MountainTrees, range, 0.80f, new Color(29, 114, 51));
            mountain = new Terrain(TerrainEnum.Mountain, range, 0.93f, new Color(67, 67, 67));
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
            } else if (z < grass.HeightLimit) {
                return grass;
            } else if (z < jungle.HeightLimit) {
                return jungle;
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
