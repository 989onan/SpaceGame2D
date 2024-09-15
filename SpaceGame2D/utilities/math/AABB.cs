using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceGame2D.enviroment.Entities;
using SpaceGame2D.graphics.compiledshaders;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public class AABB
    {
        public readonly Vector2 BottomRight;
        public readonly Vector2 TopLeft;


        //origin is top left of screen.
        public float x_max => BottomRight.X;
        public float y_max => BottomRight.Y;

        public float x_min => TopLeft.X;

        public float y_min => TopLeft.Y;

        public Vector2 size => new Vector2(x_max - x_min, y_max - y_min);

        //get center of box.
        public Vector2 Center => new Vector2(((x_max - x_min) / 2)+ x_min, ((y_max - y_min) / 2) + y_min);




        public AABB(Vector2 BottomRight, Vector2 TopLeft)
        {
            this.BottomRight = BottomRight;
            this.TopLeft = TopLeft;
        }

        public static AABB Size_To_AABB(Vector2 center, Vector2 size)
        {
            Vector2 BottomRight = new Vector2(center.X + (size.X / 2), center.Y + (size.Y / 2));
            Vector2 TopLeft = new Vector2(center.X - (size.X / 2), center.Y - (size.Y / 2));
            return new AABB(BottomRight, TopLeft);
        }

        public bool DrawImage_Self(IShader shader_program, ImageResult image, float zoom)
        {
            
            float[] vertices = {
                x_min * zoom, y_max * zoom, 0.0f,// 0.0f, 0.0f,  // lower-left corner  
                x_min * zoom, y_min * zoom, 0.0f,// 0.0f, 0.0f,  // top-left corner
                x_max * zoom, y_min * zoom, 0.0f,// 0.0f, 1.0f,   // top-right corner
                x_max * zoom, y_max * zoom, 0.0f//, 1.0f, 0.0f  // lower-right corner
                
            };

            uint[] indices = {0,1,2, // first triangle (bottom left - top left - top right)
                     0,2,3};
            float[] vertices1 =
            {
                x_min * zoom, y_max * zoom, 0.0f, //Bottom-left vertex
                x_min * zoom, y_min * zoom, 0.0f, //top-left vertex
                x_max * zoom, y_min * zoom, 0.0f, //top-right vertex
                
            };
            float[] vertices2 =
            {
                x_min * zoom, y_max * zoom, 0.0f, //Bottom-left vertex
                x_max * zoom, y_min * zoom, 0.0f, //top-right vertex
                x_max * zoom, y_max * zoom, 0.0f, //Bottom-right vertex
            };
            /*float[] vertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
            };*/
            float[] colors =
            {
                1.0f,1.0f,1.0f,1.0f,
                1.0f,1.0f,1.0f,1.0f,
                1.0f,1.0f,1.0f,1.0f,
                1.0f,1.0f,1.0f,1.0f
            };



            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            int VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            int VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            int ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            
            int vertex_location = GL.GetAttribLocation(shader_program.Handle, "aPosition");

            //Console.WriteLine(vertex_location);

            GL.VertexAttribPointer(vertex_location, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //int texCoordLocation = GL.GetAttribLocation(shader_program.Handle,"aTexCoord");
            //GL.EnableVertexAttribArray(texCoordLocation);
            //GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(vertex_location);
            shader_program.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteVertexArray(ElementBufferObject);


            return true;
        }

        public bool ContainsPoint(Vector2 point)
        {

            if (!(point.X < BottomRight.X))
            {
                return false;
            }
            if (!(point.X > TopLeft.X))
            {
                return false;
            }
            if (!(point.Y > BottomRight.Y))
            {
                return false;
            }
            if (!(point.Y < TopLeft.Y))
            {
                return false;
            }
            return true;
        }

        //TODO: I have no clue if this works.
        public bool Intercects(AABB other)
        {
            if (other.TopLeft.X < BottomRight.X &&
                other.BottomRight.X > TopLeft.X &&
                other.TopLeft.Y < BottomRight.Y &&
                other.BottomRight.Y > TopLeft.Y)
            {
                return true;
            }
            return false;
        }
    }
}
