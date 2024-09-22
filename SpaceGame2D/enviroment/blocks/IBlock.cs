using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.species;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blocks
{
    public interface IBlock: IStaticPhysicsObject, IRenderableObject
    {

        string idle_image { get; }

        public BlockGrid grid { get; set; }





        public void destruct();


        public Point block_position { get; set; }
        

        //void LoadBlockTextures(Dictionary<string, TextureTile> atlasList); //this can be called more than once.
    }
}
