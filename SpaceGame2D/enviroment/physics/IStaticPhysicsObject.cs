using SpaceGame2D.graphics.renderables;
using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.physics
{
    public interface IStaticPhysicsObject: ICollideable
    {

        Vector2 position_physics { get;}


        void destruct();

        
    }
}
