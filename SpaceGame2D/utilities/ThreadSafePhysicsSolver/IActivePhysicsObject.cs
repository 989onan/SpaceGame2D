using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.ThreadSafePhysicsSolver
{
    public interface IActivePhysicsObject : IStaticPhysicsObject
    {
        //public bool destroying { get ;}
        bool IsActive { get; set; }

        new Vector2 position_physics { get; set; }

        public ICollideable ground { get; set; }

        Vector2 velocity { get; set; }



    }
}
