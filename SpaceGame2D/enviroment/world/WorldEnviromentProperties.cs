using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world
{
    public class WorldEnviromentProperties
    {
        public Vector2 gravity { get; }
        public float air_resistance_factor { get; set; }


        public WorldEnviromentProperties(Vector2 gravity, float air_resistance_factor)
        {
            this.gravity = gravity;
            this.air_resistance_factor = air_resistance_factor;
        }



    }
}
