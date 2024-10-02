﻿using SpaceGame2D.enviroment.physics;
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
using System.Reflection;
using SpaceGame2D.threads.Factory_Threads;

namespace SpaceGame2D.enviroment.blocks.machines
{
    public class GeneratorBlock_1 : IMachine
    {

        public bool links_changed { get; set; }
        public string idle_image => "blocks/machines/generator_1.png";

        public FactorySubThread source_factory { get; private set; }

        public string Name => "Generator Tier 1";
        public GeneratorBlock_1(BlockGrid grid, Point position)
        {
            default_init(grid, position);
            List<IMachine> linked_machines = grid_private.getAxies(
                    this.block_position)
                .Where(o =>
                    o.GetType()
                        .GetInterfaces()
                            .Contains(typeof(IMachine)))
                .Select(o => o as IMachine).ToList();
            IMachine result = linked_machines.Where(o => o.source_factory != source_factory).FirstOrDefault();
            if (!(result == null)) { this.source_factory = result.source_factory; }
            if (this.source_factory == null)
            {
                this.source_factory = new FactorySubThread(this);
            }
            Console.WriteLine((this.source_factory == null).ToString());
        }

        public string UniqueIdentifier => "SpaceGame2D:Generator_1";

        public void Iterate()
        {

            Console.WriteLine("Consuming fuel!");
        }




        /// <summary>
        /// everything from here till "Stop" needs to be copy pasted to every block type because interface bullshit. - @989onan
        /// </summary>
        /// 

        private void setGrid(BlockGrid grid)
        {

            if (!(grid == null))
            {
                if (!(grid_private == grid))
                {
                    grid_private = grid;
                    grid_private.setTileLocation(this, internal_block_positon);
                    links_changed = true;
                }
            }

        }

        private void setBlockPosition(Point position_physics)
        {
            grid.deleteTileLocation(internal_block_positon);
            internal_block_positon = position_physics;
            links_changed = true;
        }

        private BlockGrid grid_private;

        
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

        public List<IMachine> Links()
        {
            if(grid_private != null)
            {
                return grid_private.getAxies(
                    this.block_position)
                .Where(o =>
                    o.GetType()
                        .GetInterfaces()
                            .Contains(typeof(IMachine)))
                .Select(o => o as IMachine).ToList();
            }
            return new List<IMachine>();

            
        }



        //Stop

    }
}
