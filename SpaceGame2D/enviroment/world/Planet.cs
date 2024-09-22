using SpaceGame2D.enviroment.blocks;
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

        public Vector2 gravity { get; }


        public Planet(string name, Point size, Vector2 gravity)
        {
            this.BlockGrid = new BlockGrid(size);
            this.gravity = gravity;
            this.Name = name;

            
        }
    }
}
