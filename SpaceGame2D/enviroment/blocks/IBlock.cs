using SpaceGame2D.enviroment.species;
using SpaceGame2D.enviroment.storage;
using SpaceGame2D.enviroment.world;
using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.utilities.math;
using SpaceGame2D.utilities.ThreadSafePhysicsSolver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.blocks
{
    public interface IBlock: IStaticPhysicsObject, IRenderableObject, IItemSource
    {
        string idle_image { get; }

        public BlockGrid grid { get; set; }







        public Point block_position { get; set; }


        public PhysicalItem Mine();

        //void LoadBlockTextures(Dictionary<string, TextureTile> atlasList); //this can be called more than once.
    }
}
