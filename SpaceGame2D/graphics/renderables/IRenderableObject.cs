using SpaceGame2D.graphics.texturemanager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.renderables
{
    public interface IRenderableObject
    {


        Vector2 graphic_size { get; }

        Vector2 GraphicCenterPosition { get; }

        TextureTileFrame UpdateCurrentImage(float animation_time);
        IRenderableWorldGraphic graphic { get; }

    }
}
