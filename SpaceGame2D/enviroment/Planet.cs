using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.compiledshaders;

namespace SpaceGame2D.enviroment
{
    public class Planet: IWorld
    {
        public readonly Universe ParentUniverse;
        public string name { get; }

        public Vector2 gravity { get; }

        public TileGrid enviroment { get; }

        public Planet(Universe ParentUniverse, Size size, Vector2 gravity, string name="NULL") {
            this.ParentUniverse = ParentUniverse;
            this.name = name;

            this.gravity = gravity;

            this.enviroment = new TileGrid(size, this);
        }
    }
}
