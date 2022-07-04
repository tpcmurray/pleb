using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using pleb.ProcGen.Biomes;

namespace pleb.ProcGen
{
    [DebuggerDisplay("({x},{y},{z}) Rise: {Rise} Run: {Run} Terrain: {Terrain.TerrainEnum}")]
    public class PointInfo
    {
        public PointInfo(int x, int y, float z, Map map)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            Map = map;
        }

        public PointInfo North()
        {
            int newY = y == 0 ? 0 : y - 1;
            return Map[x, newY];
        }

        public PointInfo NorthEast()
        {
            int newX = x == Map.Width - 1 ? x : x + 1;
            int newY = y == 0 ? 0 : y - 1;
            return Map[newX, newY];
        }

        public PointInfo East()
        {
            int newX = x == Map.Width - 1 ? x : x + 1;
            return Map[newX, y];
        }

        public PointInfo SouthEast()
        {
            int newX = x == Map.Width - 1 ? x : x + 1;
            int newY = y == Map.Height - 1 ? y : y + 1;
            return Map[newX, newY];
        }
        public PointInfo South()
        {
            int newY = y == Map.Height - 1 ? y : y + 1;
            return Map[x, newY];
        }

        public PointInfo SouthWest()
        {
            int newX = x == 0 ? 0 : x - 1;
            int newY = y == Map.Height - 1 ? y : y + 1;
            return Map[newX, newY];
        }
        public PointInfo West()
        {
            int newX = x == 0 ? 0 : x - 1;
            return Map[newX, y];
        }
        public PointInfo NorthWest()
        {
            int newX = x == 0 ? 0 : x - 1;
            int newY = y == 0 ? 0 : y - 1;
            return Map[newX, newY];
        }

        public HashSet<PointInfo> GetSurroundingPoints()
        {
            var points = new HashSet<PointInfo>();
            points.Add(North());
            points.Add(NorthEast());
            points.Add(East());
            points.Add(SouthEast());
            points.Add(South());
            points.Add(SouthWest());
            points.Add(West());
            points.Add(NorthWest());
            return points;
        }

        public int x { get; set; }
        public int y { get; set; }
        public float z { get; set; }
        public Map Map { get; }
        public short Rise { get; set; }
        public short Run { get; set; }
        public Biome Biome { get; set; }
        public Terrain Terrain { get; set; }
        public Feature Feature { get; set; }
    }
}
