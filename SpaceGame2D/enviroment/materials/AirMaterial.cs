using SpaceGame2D.enviroment.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.materials
{
    public class AirMaterial : IMaterial
    {
        public float friction => 0f;

        public float weight => 0f;

        public float bouyancy => float.PositiveInfinity;

        public float bouncyness => 0f;

        public void onContact(IMoveableEntity contacted_character, IBlock self)
        {
            
        }

        public bool collision => false;
    }
}
