using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world.generators
{
    public class WorldGeneratorTerra
    {

        public int[] surface;

        public int sharpness = 0;

        public int seed;
        public BlockGrid grid;

        public float ore_frequency = 3;

        public WorldGeneratorTerra(int seed, BlockGrid grid)
        {
            this.seed = seed;

            this.grid = grid;
        }

        public void generate()
        {
            PerlinNoise noiser = new PerlinNoise(seed);

            surface = new int[grid.size.X];


            //initial ground pass
            for (int i = 0; i < grid.size.X; i++)
            {
                double i_doub = i;
                surface[i] = (int)(Math.Abs(noiser.Noise(i_doub / 100, i_doub / 100, i_doub / 100) * 40) + grid.size.Y / 10);

                if (surface[i] > grid.size.Y - 1)
                {
                    surface[i] = grid.size.Y - 1;
                }
                //Console.WriteLine(surface[i]);

                new Earth_GrassBlock(grid, new Point(i, surface[i]));

                for (int j = surface[i] - 1; j > 0; j--)
                {

                    new Earth_DirtBlock(grid, new Point(i, j));
                }
            }


            PerlinNoise noiser_ore = new PerlinNoise(seed + 10);



            for (int i = 0; i < grid.size.X; i++)
            {
                int ore_level = surface[i] - 5;
                if (ore_level > 0)
                {
                    for (int y = 0; y < ore_level; y++)
                    {

                        //Console.WriteLine(surface[i]);
                        double i_doub = i;
                        double y_doub = y;
                        if (Math.Abs(noiser_ore.Noise(i_doub / 10, y_doub / 10, 0) * ore_frequency) > 1)
                        {
                            new Ore_Copper(grid, new Point(i, y));
                        }
                        else
                        {
                            new StoneGeneric(grid, new Point(i, y));
                        }
                    }
                }


            }

        }
    }
}
