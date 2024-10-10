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
    public class WorldGeneratorAsteroids
    {

        public int[] surface;

        public int sharpness = 0;

        public int seed;
        public BlockGrid grid;

        public float ore_frequency = 5;

        public WorldGeneratorAsteroids(int seed, BlockGrid grid) {
            this.seed = seed;

            this.grid = grid;
        }

        public void generate()
        {
            PerlinNoise noiser = new PerlinNoise(seed);



            PerlinNoise noiser_ore = new PerlinNoise(seed+10);



            for (int i = 0; i < grid.size.X; i++)
            {
                for (int y = 0; y < grid.size.Y; y++)
                {

                    //Console.WriteLine(surface[i]);
                    double i_doub = ((double)(i));
                    double y_doub = ((double)(y));
                    if ((Math.Abs(noiser_ore.Noise(i_doub / 10, y_doub / 10, 0) * ore_frequency)) > 1)
                    {
                        new StoneGeneric(grid, new Point(i, y));
                    }
                }
            }

            
        }
    }
}
