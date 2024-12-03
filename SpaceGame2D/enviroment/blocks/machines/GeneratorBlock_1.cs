using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.graphics.renderables;
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
using System.Reflection;
using SpaceGame2D.threads.Factory_Threads;
using SpaceGame2D.threads.GraphicsThread;
using SpaceGame2D.utilities.ThreadSafePhysicsSolver;
using SpaceGame2D.enviroment.world;
using SpaceGame2D.utilities.math;
using SpaceGame2D.enviroment.resources;

namespace SpaceGame2D.enviroment.blocks.machines
{
    public class GeneratorBlock_1 : IMachine
    {

        public string idle_image => "blocks/machines/generator_1.png";

        public FactorySubThread source_factory { get; set; }

        public GeneratorBlock_1(BlockGrid grid, Point position)
        {
            default_init(grid, position);
            recalc_factory_thread();
        }

        public string UniqueIdentifier => "SpaceGame2D:Generator_1";

        public List<ModifiableTuple2<Dictionary<string, int>>> ProcessingTypes => new List<ModifiableTuple2<Dictionary<string, int>>>() {
            new(
                new() { { "SpaceGame2D:Coal", 1 } },
                new() { { "SpaceGame2D:Power", 20} }
                )
        };
        /// <summary>
        /// everything from here till "Stop" needs to be copy pasted to every block type because interface bullshit. - @989onan
        /// </summary>
        /// 

        public Item asItem()
        {
            return new Item(this.UniqueIdentifier, this.idle_image,this.graphic.shader);
        }

        private int priv_selected_recipe = 0;
        public int selected_recipe
        {
            get => priv_selected_recipe; set
            {

                
                priv_selected_recipe = value;
            }
        }
        
        private void recalc_factory_thread()
        {
            IEnumerable<FactorySubThread> threads = this.grid.getAxies(this.block_position).OfType<IMachine>().Select(x => x.source_factory);
            if (threads.Count() > 0)
            {
                source_factory = threads.First();
            }
            else
            {
                source_factory = new FactorySubThread(this);
            }
        }
        private void setGrid(BlockGrid grid)
        {

            if (!(grid == null))
            {
                if (!(grid_private == grid))
                {
                    grid_private = grid;
                    grid_private.setTileLocation(this, internal_block_positon);
                    recalc_factory_thread();
                }
            }

        }

        private void setBlockPosition(Point position_physics)
        {
            grid.deleteTileLocation(internal_block_positon);
            internal_block_positon = position_physics;
            recalc_factory_thread();
        }

        private BlockGrid grid_private;

        
        public BlockGrid grid { get => grid_private; set => setGrid(value); }

        public IRenderableWorldGraphic graphic { get; private set; }

        private void default_init(BlockGrid grid, Point position)
        {

            internal_block_positon = position;
            this.grid = grid;
            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", 0);
            HasCollision = true;
            
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
            HasCollision = false;
            grid.deleteTileLocation(this.internal_block_positon);
            this.grid_private = null;
            this.source_factory.Recalculate();
            this.source_factory = null;
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
