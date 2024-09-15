using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using SpaceGame2D.enviroment.Entities;

namespace SpaceGame2D.enviroment
{
    public interface IMaterial
    {
        float friction {  get; }

        float weight { get; }

        float bouyancy { get; }

        float bouncyness { get; }

        void onContact(IMoveableEntity contacted_character, IBlock self);

        bool collision { get; }
    }
}
