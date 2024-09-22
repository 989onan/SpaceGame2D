using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world
{
    public class WorldGenerator
    {

        public WorldGenerator(int seed, BlockGrid grid) {

            PerlinNoise noiser = new PerlinNoise(seed);

            for(int i = 0; i < grid.size.X; i++)
            {
                new Earth_GrassBlock(grid, new Point(i, (int)(noiser.Noise(i/10, i / 10, i / 10) * 100)));
            }
        }
    }
}
