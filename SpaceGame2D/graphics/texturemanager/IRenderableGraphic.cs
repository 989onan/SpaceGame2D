using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public interface IRenderableGraphic
    {

        void LoadGraphic();

        TextureTile image { get; }

        void DisposeGraphic();

        bool DrawImage(float zoom, Vector2 offset, float window_height, float window_width);

    }
}
