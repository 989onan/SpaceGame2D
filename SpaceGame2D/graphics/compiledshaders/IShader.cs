using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.compiledshaders
{
    public interface IShader
    {

        int Handle { get; }
        void Use();
        void Dispose();

        void Load();
    }
}
