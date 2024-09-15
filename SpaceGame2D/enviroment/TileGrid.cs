using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using SpaceGame2D.enviroment.blockTypes;
using System.Collections;

namespace SpaceGame2D.enviroment
{
    public class TileGrid
    {

        private IBlock[,] Tiles;

        private Size size;

        public IWorld world;



        public TileGrid(Size size, IWorld source_world)
        {
            Tiles = new Air[size.Width * 2, size.Height * 2];
            for (int i = 0; i < (size.Width * 2); i++)
            {
                for(int j = 0; j < (size.Height * 2); j++)
                {
                    Tiles[i, j] = new Air(new Point(i,j), this);
                }
            }
            world = source_world;


            this.size = size;
        }


        private Point convert_point(Point position)
        {
            return new Point((position.X + this.size.Width) , (position.Y + this.size.Height));
        }

        public IBlock getTile(Point position)
        {
            if((Math.Abs(position.X) < this.size.Width) && (Math.Abs(position.Y) < this.size.Height))
            {
                Point actualPosition = convert_point(position);

                return Tiles[actualPosition.X, actualPosition.Y];
            }
            else
            {
                throw new IndexOutOfRangeException("Get position is outside of grid tile size! Size: " + size.ToString() + " Position: "+ position.ToString() + " Keep in mind, Position is converted from a negative<->positive to a purely positive range here.");
            }
            
        }


        public void setTile(Point position, IBlock block)
        {
            if ((Math.Abs(position.X) < this.size.Width) && (Math.Abs(position.Y) < this.size.Height))
            {
                Point actualPosition = convert_point(position);

                Tiles[actualPosition.X, actualPosition.Y] = block;
            }
            else
            {
                throw new IndexOutOfRangeException("Set position is outside of grid tile size! Size: " + size.ToString() + " Position: " + position.ToString() + " Keep in mind, Position is converted from a negative<->positive to a purely positive range here.");
            }
        }
    }
}
