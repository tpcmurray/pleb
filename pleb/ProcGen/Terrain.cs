using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen
{
    public class Terrain
    {
        public Terrain(TerrainEnum terrainEnum, PercRangeFloat range, float percentUpperLimit, Color color)
        {
            HeightLimit = range.GetValueFromPercent(percentUpperLimit);
            TerrainEnum = terrainEnum;
            Color = color;
        }

        public float HeightLimit { get; }
        public TerrainEnum TerrainEnum { get; set; }
        public Color Color { get; }
    }

    public enum TerrainEnum
    {
        Ocean,
        Shallows,
        Shoreline,
        Grass,
        Forest,
        Tundra,
        Ice,
        MountainTrees,
        Mountain,
        Snow,
        Jungle,
        Desert,
        River,
        Lake,
    }
}
