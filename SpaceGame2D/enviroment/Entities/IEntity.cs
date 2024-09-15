using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    public interface IEntity: IStaticPhysicsObject
    {
        string name { get; set; }
        string UUID { get;}

        


    }
}
