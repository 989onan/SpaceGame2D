using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
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
        void SetMatrix4(string name, Matrix4 mat);
        int Handle { get; }
        void Use();
        void Dispose();

        void Load();
    }
}
