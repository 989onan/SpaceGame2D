using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.graphics.renderables;
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
using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.storage;
using SpaceGame2D.graphics.ui;
using SpaceGame2D.graphics.ui.storage;
using SpaceGame2D.enviroment.blocks.machines;
using SpaceGame2D.threads.Factory_Threads;
using SpaceGame2D.threads.GraphicsThread;

namespace SpaceGame2D.enviroment.blocks
{
    public class CopperCrate: IStorageObject, IBlock, IMachine
    {

        

        public string idle_image => "blocks/copper_crate.png";

        public string Name { get; set; }
        public CopperCrate(BlockGrid grid, Point position)
        {
            default_init(grid, position);
            this.Name = "Copper Crate";
            this.inventory = new StorageContainer(36);
            this.storageScreen = new GridInventoryScreen("SpaceGame2D:default", this, 12);
            links_changed = false;
        }

        
        

        public void Iterate()
        {
            //do nothing, since we're just a copper crate.
        }

        


        public string UniqueIdentifier => "SpaceGame2D:CopperCrate";

        /// <summary>
        /// everything from here till "Stop" needs to be copy pasted to every block type because interface bullshit. - @989onan
        /// </summary>
        /// 
        public bool links_changed { get; set;}
        public StorageContainer inventory { get; }

        public IStorageScreen storageScreen { get; }

        private BlockGrid grid_private;

        private void setGrid(BlockGrid grid)
        {
            if (!(grid_private == grid))
            {
                if (grid_private != null)
                {
                    grid_private.deleteTileLocation(internal_block_positon);
                }
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
            HasCollision = false;
            internal_block_positon = position;
            this.grid = grid;
            Main_PhysicsThread.static_physics_objects.Add(this);
            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", 0);

        }

        public Vector2 position_physics { get => new Vector2(((float)internal_block_positon.X * BlockGrid.size_grid), ((float)internal_block_positon.Y * BlockGrid.size_grid)); set => this.block_position = new Point((int)value.X, (int)value.Y); } //this allows us to render the block dynamically on screen from a position.
        public Vector2 graphic_size => new Vector2(BlockGrid.size_grid, BlockGrid.size_grid);

        public AABB Collider { get => AABB.Size_To_AABB(position_physics, graphic_size); }
        public bool HasCollision { get; set; }

        private Point internal_block_positon { get; set; }
        public Point block_position { get => internal_block_positon; set => setBlockPosition(value); }

        public Vector2 GraphicCenterPosition => position_physics;

        public FactorySubThread source_factory => null;

        public TextureTileFrame UpdateCurrentImage(float animation_time)
        {
            return Atlas.getTextureAnimated(idle_image, animation_time);
        }

        public void destruct()
        {
            Main_GraphicsThread._worldGraphicObjects.Remove(this.graphic);
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

        //we don't try to interact with machines around us, since we're just a copper crate.
        //To people who look at the source code, yes this can be used to link 2 machine groups without them iterating at the same time.
        public List<IMachine> Links() => new List<IMachine>();


        //Stop

    }
}
