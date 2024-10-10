using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.species;
using SpaceGame2D.enviroment.storage;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.graphics.ui;
using SpaceGame2D.graphics.ui.storage;
using SpaceGame2D.threads.GraphicsThread;
using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment
{
    public class Player: IRenderableObject, IActivePhysicsObject, IStorageObject
    {

        public static readonly float JetPackForce = 10;
        public StorageContainer inventory { get;}

        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool JetPackOn { get; set; }


        public Player(Vector2 position, ISpecies species) {

            Collider = AABB.Size_To_AABB(position, new Vector2(.8f, 2));
            this.graphic_size = new Vector2(1f,2f);
            this.position_physics = position;
            this.species = species;
            this.velocity = Vector2.Zero;


            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", 3);
            Main_PhysicsThread.active_physics_objects.Add(this);
            this.IsActive = true;
            inventory = new StorageContainer(48); //4X12
            this.storageScreen = new GridInventoryScreen("SpaceGame2D:default", this, 12);
        }

        public ISpecies species;



        //what we should be displaying to grapics based on state.
        public TextureTileFrame UpdateCurrentImage(float animation_time)
        {
            if ((int)(this.velocity.X * 10) > 0)
            {
                TextureTileFrame image = Atlas.getTextureAnimated(species.walking_image, animation_time);
                image.flip_x = true;
                return image;
            }
            else if ((int)(this.velocity.X * 10) < 0)
            {
                return Atlas.getTextureAnimated(species.walking_image, animation_time);
            }
            return Atlas.getTextureAnimated(species.standing_image, animation_time);

        }


        public int id;

        /// <summary>
        /// up until stop, this is required for physics and rendering. not very interesting - @989onan
        /// </summary>
        /// 

        public ICollideable ground { get; set; }
        public void DisposeGraphic()
        {
            Main_GraphicsThread._worldGraphicObjects.Remove(this.graphic);
        }

        public void TriggerCollideEvent(IActivePhysicsObject physicsObject, Vector2 normal)
        {
            Console.WriteLine("Player collision event!?");
        }

        public bool IsActive { get; set; }
        public Vector2 player_position { get => Collider.bottom_middle; set => Collider.bottom_middle = value; }

        public Vector2 velocity { get; set; }

        public Vector2 position_physics { get => Collider.Center; set => Collider.Center = value; }

        public Vector2 GraphicCenterPosition { get => position_physics; }

        

        public Vector2 graphic_size { get; private set; }

        public IRenderableWorldGraphic graphic { get; private set; }
        public AABB Collider { get; }
        public bool HasCollision { get; set; }

        public IStorageScreen storageScreen { get; }
        

        public void destruct()
        {
            Main_GraphicsThread._worldGraphicObjects.Remove(this.graphic);
            Main_PhysicsThread.active_physics_objects.Remove(this);
            HasCollision = false;
        }


        //stop


    }
}
