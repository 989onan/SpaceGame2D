using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.species;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
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

        public ISpecies species;


        public Vector2 size { get; private set; }

        public AABB Collider { get; }
        IRenderableGraphic graphic;

        public TextureTile currentTexture { get => UpdateCurrentImage();  }
        public bool HasCollision { get; set; }

        public Player(Vector2 position, ISpecies species) {

            Collider = AABB.Size_To_AABB(new Vector2(0, 0), new Vector2(1, 2));
            this.size = new Vector2(1.2f,1.2f);
            this.position = position;
            this.species = species;


            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default");
        }


        public void DisposeGraphic()
        {

        }
        ~Player()
        {
            GraphicsRegistry.deregisterRenderGraphic(this.graphic);
        }

        public TextureTile UpdateCurrentImage()
        {
            return Atlas.Tiles.GetValueOrDefault(species.standing_image);
        }
    }
}
