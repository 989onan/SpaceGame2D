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
        Vector2 position { get; set; }
        Vector2 size { get; }

        ImageResult currentimage { get;  }
        ReaderWriterLockSlim _physlock { get; }
        IWorld World { get; }
    }
}
