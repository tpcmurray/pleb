using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen
{
    public class Map
    {
        private PointInfo[,] mi;

        public Map(int width, int height)
        {
            mi = new PointInfo[width, height];
            Width = width;
            Height = height;
        }

        public PointInfo this[int x, int y]
        {
            get { return mi[x, y]; }
            set { mi[x, y] = value; }
        }

        public int Width { get; }
        public int Height { get; }
    }
}
