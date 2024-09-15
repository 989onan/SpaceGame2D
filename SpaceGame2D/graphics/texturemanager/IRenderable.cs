using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public interface IRenderableTile
    {

        void LoadGraphic();

        void DisposeGraphic();

        bool DrawImage(float zoom, Vector2 offset);
    }
}
