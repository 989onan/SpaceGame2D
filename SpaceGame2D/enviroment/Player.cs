using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.species;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
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
    public class Player: IRenderableObject, IActivePhysicsObject
    {

        public int id;

        public bool IsActive { get; set; }

        public Vector2 player_position { get => Collider.bottom_middle; set => Collider.bottom_middle = value; }

        public Vector2 velocity { get; set; }

        public Vector2 position { get => Collider.Center; set => Collider.Center = value; }

        public Vector2 GraphicCenterPosition { get => new Vector2(player_position.X, player_position.Y+(graphic_size.Y/2)); }

        public ISpecies species;


        public Vector2 graphic_size { get; private set; }

        public AABB Collider { get; }
        public IRenderableGraphic graphic { get; private set; }

        public bool HasCollision { get; set; }

        public Player(Vector2 position, ISpecies species) {

            Collider = AABB.Size_To_AABB(position, new Vector2(.8f, 2));
            this.graphic_size = new Vector2(1f,2f);
            this.position = position;
            this.species = species;


            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default");
            Main_PhysicsThread.active_physics_objects.Add(this);
            OnGround = true;
        }
        public bool OnGround { get; set; }

        private Vector2 internal_new_velocity = new Vector2(0,0);

        private void SetInternal(Vector2 value)
        {
            if(internal_new_velocity == Vector2.Zero)
            {
                internal_new_velocity = value;
            }
        }


        public Vector2 newVelocityImpulse { get => internal_new_velocity; set => SetInternal(value); }

        public void DisposeGraphic()
        {

        }
        ~Player()
        {
            GraphicsRegistry.deregisterRenderGraphic(this.graphic);
        }

        public TextureTile UpdateCurrentImage()
        {
            return Atlas.getTexture(species.standing_image);
        }
    }
}
