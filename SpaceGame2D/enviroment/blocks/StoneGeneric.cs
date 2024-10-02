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
using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.enviroment.physics;

namespace SpaceGame2D.enviroment.blocks
{
    internal class StoneGeneric : IBlock
    {
        public string Name => "Stone";
        public string idle_image => "blocks/stone_generic.png";

        public string UniqueIdentifier => "SpaceGame2D:StoneGeneric";
        public StoneGeneric(BlockGrid grid, Point position)
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
            grid.deleteTileLocation(internal_block_positon);
            internal_block_positon = position_physics;
        }
        public BlockGrid grid { get => grid_private; set => setGrid(value); }

        public IRenderableWorldGraphic graphic { get; private set; }

        private void default_init(BlockGrid grid, Point position)
        {

            internal_block_positon = position;
            this.grid = grid;
            Main_PhysicsThread.static_physics_objects.Add(this);
            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", 0);
            HasCollision = true;
        }

        private Vector2 getGridPosition()
        {
            if (grid == null)
            {
                return new Vector2(-10000000000, -1000000000);

            }
            return grid.RenderOffset;
        }

        public Vector2 position_physics { get => new Vector2(((float)internal_block_positon.X * .5f) + getGridPosition().X, ((float)internal_block_positon.Y * .5f) + getGridPosition().Y); set => this.block_position = new Point((int)value.X, (int)value.Y); } //this allows us to render the block dynamically on screen from a position.
        public Vector2 graphic_size => new Vector2(.5f, .5f);

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
            GraphicsRegistry.deregisterWorldRenderGraphic(this.graphic);
            Main_PhysicsThread.static_physics_objects.Remove(this);
            HasCollision = false;
            grid.deleteTileLocation(this.internal_block_positon);
            this.grid_private = null;
        }

        public PhysicalItem Mine()
        {
            Vector2 last_pos = this.position_physics;
            destruct();
            return new PhysicalItem(last_pos, this);

        }

        public void TriggerCollideEvent(IActivePhysicsObject physicsObject, Vector2 normal)
        {
            //do nothing.
        }

        //Stop
    }
}
