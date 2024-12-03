using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.renderables
{
    public interface IPositionedRenderable: IRenderable
    {
        bool DrawImage(Vector2 position,Vector2 size, Vector2 window_size, float animation_time);
    }
}
