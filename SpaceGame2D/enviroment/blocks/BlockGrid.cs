using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


#nullable disable
namespace SpaceGame2D.enviroment.blocks
{
    public class BlockGrid
    {

        public static readonly float size_grid = .5f;

        //private List<AABBVoxelBlocks> voxel_grid = new List<AABBVoxelBlocks>();

        private IBlock[,] _blocks;

        public readonly Point size;

        public BlockGrid(Point size)
        {
            _blocks = new IBlock[size.X, size.Y];
            this.size = size;
        }


        public IBlock[] getFlatBlocks()
        {
            IBlock[] new_blocks = new IBlock[(size.X)*(size.Y)];
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    new_blocks[x * y] = _blocks[x, y];
                }
            }
            return new_blocks;
        }

        public void RecalculateVoxelSimplification()
        {
            /*IBlock[] blocks = getFlatBlocks();
            Main_PhysicsThread.static_physics_objects.RemoveAll(o => blocks.Contains(o));
            Main_PhysicsThread.static_physics_objects.AddRange(blocks);*/
        }

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

        public IBlock getTileAt(Point position)
        {
            return _blocks[position.X%size.X, position.Y%size.Y];
        }

        public IBlock getTileAt(int x, int y)
        {
            if (x >= this.size.X || y >= this.size.Y)
            {
                return null;
            }
            return _blocks[x, y];
            
        }

        public List<IBlock> gatherArea(Point Min, Point Max)
        {
            Point trueMin = new Point(Math.Min(Min.X, Max.X), Math.Min(Min.Y, Max.Y));
            Point trueMax = new Point(Math.Max(Min.X, Max.X), Math.Max(Min.Y, Max.Y));

            List<IBlock> blocks = new List<IBlock>();
            for (int i = trueMin.X; i < trueMax.X; i++)
            {
                for(int j = trueMin.Y; j < trueMax.Y; j++)
                {
                    blocks.Add(getTileAt(i,j));
                }
            }
            return blocks;
        }

        public List<IBlock> getAxies(Point position, int distance = 1, Direction directions = Direction.Up | Direction.Down | Direction.Right | Direction.Left)
        {
            List<IBlock> blocks = new List<IBlock>();

            

            for(int i = 0; i < distance; i++)
            {
                blocks.Add(getTileAt(position+((Size)DirectionMethods.Direct(directions)))); // TODO: THIS IS WRONG! - @989onan89
            }

            return blocks;
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
                
                _blocks[value.X, value.Y] = null;
                
            }
        }
    }
}
