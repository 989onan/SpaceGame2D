using SpaceGame2D.enviroment.Entities.Species;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    public class Player : LivingEntity
    {


        
        public Player(ISpecies species, IWorld current_world, string name = "989onan", string UUID = "5358f3cc-8b59-400c-8d69-82b55fc6e667") : base(species, current_world, name, UUID)
        {

        }
    }
}
