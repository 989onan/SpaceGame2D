using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.threads.PhysicsThread;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.threads.GraphicsThread;
using SpaceGame2D.utilities.ThreadSafePhysicsSolver;
using SpaceGame2D.enviroment.world;
using SpaceGame2D.enviroment.resources;

namespace SpaceGame2D.enviroment.blocks
{
    internal class Earth_DirtBlock: IBlock
    {

        public string idle_image => "blocks/dirt.png";

        public string UniqueIdentifier => "SpaceGame2D:EarthDirt";
        public Earth_DirtBlock(BlockGrid grid, Point position)
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

        public Item asItem()
        {
            return new Item(this.UniqueIdentifier, this.idle_image, this.graphic.shader);
        }

        private void setBlockPosition(Point position_physics)
        {
            grid.deleteTileLocation(internal_block_positon);
            internal_block_positon = position_physics;
        }
        public BlockGrid grid { get => grid_private; set => setGrid(value); }

        public IRenderableWorldGraphic graphic { get; private set; }

        private void default_init(BlockGrid grid, Point position)
        {
            HasCollision = true;
            internal_block_positon = position;
            this.grid = grid;
            Main_PhysicsThread.solver.QueueAddStatic(this);
            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", 0);

        }

        public Vector2 position_physics { get => new Vector2(((float)internal_block_positon.X * BlockGrid.size_grid), ((float)internal_block_positon.Y * BlockGrid.size_grid)); set => this.block_position = new Point((int)value.X, (int)value.Y); } //this allows us to render the block dynamically on screen from a position.
        public Vector2 graphic_size => new Vector2(BlockGrid.size_grid, BlockGrid.size_grid);

        public AABB Collider { get => AABB.Size_To_AABB(position_physics, graphic_size); }
        public bool HasCollision { get; set; }

        private Point internal_block_positon { get; set; }
        public Point block_position { get => internal_block_positon; set => setBlockPosition(value); }

        public Vector2 GraphicCenterPosition => position_physics;

        public TextureTileFrame UpdateCurrentImage(float animation_time)
        {
            return Atlas.getTextureAnimated(idle_image, animation_time);
        }

        public void destruct()
        {
            Main_GraphicsThread._worldGraphicObjects.Remove(this.graphic);
            Main_PhysicsThread.solver.QueueRemoveStatic(this);
            HasCollision = false;
            grid.deleteTileLocation(this.internal_block_positon);
            this.grid_private = null;
        }

        public PhysicalItem Mine()
        {
            Vector2 last_pos = this.position_physics;
            destruct();
            return new PhysicalItem(last_pos, asItem());

        }

        public void TriggerCollideEvent(IActivePhysicsObject physicsObject, Vector2 normal)
        {
            //do nothing.
        }

        //Stop
    }
}
