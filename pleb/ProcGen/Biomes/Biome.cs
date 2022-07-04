using System;
using System.Collections.Generic;
using System.Text;

namespace pleb.ProcGen.Biomes
{
    public interface Biome
    {
        Terrain GetTerrain(float z);
    }
}
