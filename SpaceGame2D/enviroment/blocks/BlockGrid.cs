using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blocks
{
    public class BlockGrid
    {

        private IBlock[,] _blocks;

        public readonly Point size;

        public BlockGrid(Point size)
        {
            _blocks = new IBlock[size.X, size.Y];
            this.size = size;
            RenderOffset = new Vector2(0, 0);
        }

        public Vector2 RenderOffset { get; set; }


        public Point getTileLocation(IBlock tile)
        {
            if(tile == null)
            {
                return new Point(0, 0);
            }
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    if (tile == _blocks[x, y])
                    {
                        return new Point(x, y);
                    }
                }
            }
            return new Point(-1, -1);
        }

        public void moveTileLocation(IBlock tile, Point value)
        {
            if (_blocks[value.X, value.Y] != null)
            {
                _blocks[value.X, value.Y].destruct();
                _blocks[value.X, value.Y] = new AirBlock(this, value);
            }
        }

        public void setTileLocation(IBlock tile, Point value)
        {
            if (_blocks[value.X, value.Y] != null)
            {
                _blocks[value.X, value.Y].destruct();
            }
            tile.grid = this;
            _blocks[value.X, value.Y] = tile;
        }

        public void deleteTileLocation(Point value)
        {
            if (_blocks[value.X, value.Y] != null)
            {
                _blocks[value.X, value.Y].destruct();
                _blocks[value.X, value.Y] = new AirBlock(this, value);
            }
        }

        public void moveGridsTileLocation(Point value)
        {
            if (_blocks[value.X, value.Y] != null)
            {
                _blocks[value.X, value.Y] = new AirBlock(this,value);
            }

        }
    }
}
