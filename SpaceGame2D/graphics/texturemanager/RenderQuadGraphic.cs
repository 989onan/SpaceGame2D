using OpenTK.Graphics.OpenGL;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class RenderQuadGraphic : IRenderableGraphic
    {
        private IShader shader;

        public TextureTile image => this.graphicObject.currentTexture;

        private int VertexArrayObject;
        private int ElementBufferObject;
        private int VertexBufferObject;
        private uint[] indices = {0,1,2, // first triangle (bottom left - top left - top right)
                     0,2,3};

        public IRenderableObject graphicObject {get; private set;}
        public RenderQuadGraphic(IRenderableObject graphicObject, string shader_program)
        {
            this.graphicObject = graphicObject;
            shader = ShaderManager.getAll().GetValueOrDefault(shader_program);
            GraphicsRegistry.registerRenderGraphic(this);
        }


        public void LoadGraphic()
        {
            

            ElementBufferObject = GL.GenBuffer();
            




            //Console.WriteLine(image.Handle); 



            VertexArrayObject = GL.GenVertexArray();
           
            VertexBufferObject = GL.GenBuffer();
            

        }

        public void DisposeGraphic()
        {
            Console.WriteLine("disposing graphic");
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteVertexArray(ElementBufferObject);
        }

        public bool DrawImage(float zoom, Vector2 offset)
        {

            AABB position = AABB.Size_To_AABB(this.graphicObject.position, this.graphicObject.size);


            //get position of object, and also find graphic position on atlas.
            float[] vertices = {
                (position.x_min + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, this.graphicObject.currentTexture.x, this.graphicObject.currentTexture.y,  // lower-left corner  
                (position.x_min + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, this.graphicObject.currentTexture.x, this.graphicObject.currentTexture.height + this.graphicObject.currentTexture.y,  // top-left corner
                (position.x_max + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, this.graphicObject.currentTexture.x + this.graphicObject.currentTexture.width, this.graphicObject.currentTexture.height + this.graphicObject.currentTexture.y,   // top-right corner
                (position.x_max + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, this.graphicObject.currentTexture.x + this.graphicObject.currentTexture.width, this.graphicObject.currentTexture.y  // lower-right corner
                
            };

            /*vertices = new float[]{
                (position.x_min + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, 0f, 0f,  // lower-left corner  
                (position.x_min + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, 0f, 1.0f,  // top-left corner
                (position.x_max + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, 1.0f, 1.0f,   // top-right corner
                (position.x_max + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, 1.0f, 0f  // lower-right corner
                
            };*/
            this.shader.Use();
            Atlas.UseImage();

            int vertex_location = GL.GetAttribLocation(shader.Handle, "aPosition");

            //Console.WriteLine(vertex_location);
            GL.EnableVertexAttribArray(vertex_location);
            GL.VertexAttribPointer(vertex_location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            
            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            //Console.WriteLine(texCoordLocation);
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            
            GL.BindVertexArray(VertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            
            


            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            
            return true;
        }

    }
}
