using SpaceGame2D.enviroment.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment
{
    public interface IBlock: IStaticPhysicsObject
    {
        TileGrid source_grid { get; }
        IMaterial surfaceProperties { get; }

        


    }
}
