using SpaceGame2D.graphics.compiledshaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.renderables
{
    public interface IRenderableWorldGraphic: IRenderable
    {

        //void LoadGraphic();

        

        //TextureTileFrame image { get; }

        //void DisposeGraphic();

        bool DrawImage(float zoom, Vector2 offset, Vector2 window_size, float animation_time);

    }
}
