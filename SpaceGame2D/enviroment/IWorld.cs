using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment
{
    public interface IWorld
    {
        
        TileGrid enviroment { get; }



        string name { get; }
        Vector2 gravity { get; }



    }
}
