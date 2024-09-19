using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public interface IRenderableObject
    {

        Vector2 position { get;}
        Vector2 graphic_size { get; }

        TextureTile currentTexture { get; }

    }
}
