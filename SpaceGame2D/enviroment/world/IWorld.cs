﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world
{
    public interface IWorld
    {
        string Name { get; }
        BlockGrid BlockGrid { get; }

        
        bool isPlanet { get; }

        WorldEnviromentProperties enviro { get;}
    }
}
