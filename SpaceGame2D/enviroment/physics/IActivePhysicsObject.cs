using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.physics
{
    public interface IActivePhysicsObject: IStaticPhysicsObject
    {

        bool IsActive { get; set; }
        new Vector2 position { get; set; }

        Vector2 newVelocityImpulse { get; set; }

        public bool OnGround { get; set; }

        Vector2 velocity { get; set; }



    }
}
