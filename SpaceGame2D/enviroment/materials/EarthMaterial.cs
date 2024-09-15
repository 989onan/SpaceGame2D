using SpaceGame2D.enviroment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.materials
{
    internal class EarthMaterial : IMaterial
    {
        public float friction => 10f;

        public float weight => 1000f;

        public float bouyancy => 2f;

        public float bouncyness => 0f;

        public bool collision => true;

        public void onContact(IMoveableEntity contacted_character, IBlock self)
        {
            
        }
    }
}
