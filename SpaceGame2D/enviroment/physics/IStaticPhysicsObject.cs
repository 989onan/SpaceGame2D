using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.physics
{
    public interface IStaticPhysicsObject
    {

        Vector2 position { get;}

        AABB Collider { get; }




        bool HasCollision { get; set; }
    }
}
