using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.utilities.math;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.Entities
{
    public interface IStaticPhysicsObject
    {
        AABB bounding_box { get; set; }

        ReaderWriterLockSlim _physlock { get; }
        IWorld World { get; }

        IRenderableTile graphic { get; }
    }
}
