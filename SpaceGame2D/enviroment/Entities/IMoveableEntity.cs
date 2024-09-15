using SpaceGame2D.graphics.texturemanager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    public interface IMoveableEntity: IEntity
    {
        Vector2 velocity { get; }

        bool applyForceImpulse(Vector2 velocity);


    }
}
