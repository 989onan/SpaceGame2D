using SpaceGame2D.enviroment.Entities.Species;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.utilities.math;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    public class LivingEntity : ILivingEntity, IMoveableEntity
    {
        public AABB bounding_box { get; set; }
        public IWorld World { get; }
        public LivingEntity(ISpecies species, IWorld current_world, string name = "989onan", string UUID = "5358f3cc-8b59-400c-8d69-82b55fc6e667")
        {
            this.name = name;
            this.UUID = UUID;
            this.species = species;
            this.World = current_world;
            this.bounding_box = AABB.Size_To_AABB(new Vector2(0, 0), size);
            this.velocity = new Vector2 (0, 0);

            this.graphic = new RenderQuadGraphic(this, "SpaceGame2D:default", species.standing);


            this._physlock = new ReaderWriterLockSlim();
        }

        public bool is_crouching { get; set; }

        public bool is_running { get; set; }

        public int Health = 100;
        public int Armor { get => getCurArmor(); }
        public int MagicArmor { get; set; }

        public Vector2 position => this.bounding_box.Center;

        public Vector2 size { get => getPlayerSize(); }

        private Vector2 getPlayerSize()
        {
            return this.species.stand_size;
        }

        private int getCurArmor()
        {
            return this.species.base_armor;
        }

        public Vector2 velocity { get; }

        public ISpecies species {  get; }

        int IDamageableEntity.Health { get; set; }
        public string name { set; get; }

        private string uuid = ""; //this gets set in the constructor. this is to ignore warnings basically.
        public string UUID { 
            get {
                return this.uuid;
            } 
            set => setUUID(value);
        }

        public ReaderWriterLockSlim _physlock { get; }
        

        public IRenderableTile graphic { get; set; }

        public bool applyForceImpulse(Vector2 velocity)
        {
            return true;
        }

        public int Damage(int health)
        {
            this.Health = Math.Max(health-this.Armor, 0);

            

            return this.Health;
        }

        public int Heal(int health)
        {
            this.Health = Math.Max(health + this.Health, this.species.max_health);

            return this.Health;
        }

        //TODO: Check for duplicate UUID's in Game in it's entirety
        private bool setUUID(string uuid)
        {
            this.uuid = uuid;
            return true;
        }
    }
}
