using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment
{
    public class BlockGrid
    {

        private Block[,] _blocks;

        public readonly Point size;

        public BlockGrid(Point size)
        {
            _blocks = new Block[size.X,size.Y];
            this.size = size;
        }

        public Vector2 RenderOffset { get; set; }


        public Point getTileLocation(Block tile)
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    if(tile == _blocks[x, y])
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

        public void moveTileLocation(Block tile, Point value)
        {
            throw new NotImplementedException();
        }

        public void setTileLocation(Block tile, Point value)
        {

        }
    }
}
