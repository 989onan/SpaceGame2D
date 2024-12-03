using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world
{
    public class Planet : IWorld
    {
        public string Name { get; }

        public BlockGrid BlockGrid { get; }

        public bool isPlanet => true;

        public WorldEnviromentProperties enviro { get; private set;}

        

        public Planet(string name, Point size, WorldEnviromentProperties enviro)
        {
            this.BlockGrid = new BlockGrid(size);
            
            this.Name = name;
            this.enviro = enviro;


        }
    }
}
