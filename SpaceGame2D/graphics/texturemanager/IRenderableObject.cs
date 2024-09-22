using SpaceGame2D.enviroment.physics;
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


        Vector2 graphic_size { get; }

        Vector2 GraphicCenterPosition { get; }

        TextureTile UpdateCurrentImage();
        IRenderableGraphic graphic { get; }

    }
}
