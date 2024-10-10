using OpenTK.Graphics.OpenGL;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.renderables
{
    public class GraphicsRegistry
    {

        public static int VertexArrayObject_Quad;
        public static int ElementBufferObject_Quad;
        public static int VertexBufferObject_Quad;
        public static byte[] indices_Quad = {0,1,2, // first triangle (bottom left - top left - top right)
                     0,2,3};

        


        //buffers
        public static void LoadBuffers()
        {
            GraphicsRegistry.ElementBufferObject_Quad = GL.GenBuffer();
            GraphicsRegistry.VertexArrayObject_Quad = GL.GenVertexArray();
            GraphicsRegistry.VertexBufferObject_Quad = GL.GenBuffer();
        }

        public static void DisposeBuffers()
        {
            //Console.WriteLine("disposing graphic");
            GL.DeleteBuffer(GraphicsRegistry.VertexBufferObject_Quad);
            GL.DeleteVertexArray(GraphicsRegistry.VertexArrayObject_Quad);
            GL.DeleteVertexArray(GraphicsRegistry.ElementBufferObject_Quad);
        }

        
    }
}
