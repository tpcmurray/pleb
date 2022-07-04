using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen
{
    public class Feature
    {
        public Feature(float probability, Color color)
        {
            Probability = probability;
            Color = color;
            Locations = new List<Tuple<int, int>>();
        }

        public void Add(int x, int y)
        {
            Locations.Add(new Tuple<int, int>(x, y));
        }

        public List<Tuple<int, int>> Locations { get; }
        public float Probability { get; }
        public Color Color { get; }
    }
}
