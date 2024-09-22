using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SpaceGame2D.enviroment.blocks
{
    internal class Ore_Copper: IBlock
    {

        public string idle_image => "blocks/ore_copper.png";
        public Ore_Copper(BlockGrid grid, Point position)
        {
            default_init(grid, position);
        }



        /// <summary>
        /// everything from here till "Stop" needs to be copy pasted to every block type because interface bullshit. - @989onan
        /// </summary>
        /// 

        private BlockGrid grid_private;

        private void setGrid(BlockGrid grid)
        {
            if (!(grid_private == grid))
            {
                grid_private = grid;
                grid_private.setTileLocation(this, internal_block_positon);
            }
        }

        private void setBlockPosition(Point position_physics)
        {
            internal_block_positon = position_physics;
            grid.moveTileLocation(this, position_physics);
        }
        public BlockGrid grid { get => grid_private; set => setGrid(value); }

        public IRenderableGraphic graphic { get; private set; }

        private void default_init(BlockGrid grid, Point position)
        {

            internal_block_positon = position;
            this.grid = grid;
            Main_PhysicsThread.static_physics_objects.Add(this);
            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default");
            HasCollision = true;
        }

        public Vector2 position_physics { get => new Vector2(((float)internal_block_positon.X * .5f) + grid.RenderOffset.X, ((float)internal_block_positon.Y * .5f) + grid.RenderOffset.Y); set => this.block_position = new Point((int)value.X, (int)value.Y); } //this allows us to render the block dynamically on screen from a position.
        public Vector2 graphic_size => new Vector2(.5f, .5f);

        public AABB Collider { get => AABB.Size_To_AABB(position_physics, graphic_size); }
        public bool HasCollision { get; set; }

        private Point internal_block_positon { get; set; }
        public Point block_position { get => internal_block_positon; set => setBlockPosition(value); }

        public Vector2 GraphicCenterPosition => position_physics;

        public TextureTile UpdateCurrentImage()
        {
            return Atlas.getTexture(idle_image);
        }

        public void destruct()
        {
            GraphicsRegistry.deregisterRenderGraphic(this.graphic);
            Main_PhysicsThread.static_physics_objects.Remove(this);
        }

        //Stop
    }
}
