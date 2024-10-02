using SpaceGame2D.graphics.compiledshaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.renderables
{
    public interface IRenderable
    {
        IShader shader { get; }

        void destruct();
        int order { get; }
        public void Draw(float animationtime, Vector2 game_window_size);
    }
}
