using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.storage;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world.actors
{
    public class PhysicalItem : IActivePhysicsObject, IRenderableObject
    {
        public Vector2 graphic_size => Collider.size;

        public Vector2 GraphicCenterPosition => this.position_physics;

        public IRenderableWorldGraphic graphic { get; private set; }

        public int amount { get; set; }

        private bool isactive_private = true;

        public bool IsActive { get => getActive(); set => isactive_private = value; }

        private bool getActive()
        {
            if (ground != null)
            {
                return false;
            }
            return isactive_private;
        }

        public Vector2 position_physics { get => Collider.Center; set => Collider.Center = value; }
        public Vector2 velocity { get; set; }



        public ICollideable ground { get; set; }

        public AABB Collider { get; }

        public bool HasCollision { get; set; }

        public int count { get ; set; }
        IItemSource source_block { get; set; }

        public PhysicalItem(Vector2 start_position, IItemSource source_block)
        {
            Random vel = new Random();
            this.IsActive = true;
            this.velocity = new Vector2((float)vel.NextDouble()*.2f, (float)vel.NextDouble() * .2f);
            Collider = AABB.Size_To_AABB(start_position,new Vector2(.25f,.25f));
            this.position_physics = start_position;
            HasCollision = true;
            
            this.source_block = source_block;
            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", 0);
            this.count = 1;
            Main_PhysicsThread.active_physics_objects.Add(this);
        }

        public void destruct()
        {
            
            HasCollision = false;
            this.count = 0;
            this.IsActive = false;
            this.source_block = null;
        }

        public TextureTileFrame UpdateCurrentImage(float animation_time)
        {
            if (source_block != null)
            {
                return source_block.UpdateCurrentImage(animation_time);
            }
            return Atlas.getTextureAnimated("null.png",0);
        }

        public void TriggerCollideEvent(IActivePhysicsObject physicsObject, Vector2 normal)
        {
            if (physicsObject.GetType().GetInterfaces().Contains(typeof(IStorageObject)))
            {
                IStorageObject storageObject = physicsObject as IStorageObject;
                ItemSlot ourContents = new ItemSlot(null);
                ourContents.count = count;
                ourContents.stored = source_block;
                storageObject.inventory.AddToAny(ourContents);
                if (ourContents.count == 0)
                {
                    this.destruct();
                }
            }
        }
    }
}
