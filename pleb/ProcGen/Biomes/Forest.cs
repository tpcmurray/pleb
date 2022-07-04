using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen.Biomes
{
    public class Forest : Biome
    {
        private readonly Terrain ocean;
        private readonly Terrain shallows;
        private readonly Terrain shoreLine;
        private readonly Terrain grass;
        private readonly Terrain forest;
        private readonly Terrain mountainTrees;
        private readonly Terrain mountain;
        private readonly Terrain snow; // mountain tops

        public Forest(PercRangeFloat range)
        {
            ocean = new Terrain(TerrainEnum.Ocean, range, 0.36f, new Color(53, 53, 234));
            shallows = new Terrain(TerrainEnum.Shallows, range, 0.40f, new Color(65, 133, 255));
            shoreLine = new Terrain(TerrainEnum.Shoreline, range, 0.42f, new Color(220, 199, 174));
            grass = new Terrain(TerrainEnum.Grass, range, 0.50f, new Color(7, 158, 7));
            forest = new Terrain(TerrainEnum.Forest, range, 0.70f, new Color(9, 114, 9));
            mountainTrees = new Terrain(TerrainEnum.MountainTrees, range, 0.80f, new Color(44, 85, 63));
            mountain = new Terrain(TerrainEnum.Mountain, range, 0.93f, new Color(101, 101, 101));
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
            } else if (z < forest.HeightLimit) {
                return forest;
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
