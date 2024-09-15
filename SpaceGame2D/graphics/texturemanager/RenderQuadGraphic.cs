using OpenTK.Graphics.OpenGL;
using SpaceGame2D.enviroment.Entities;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class RenderQuadGraphic: IRenderableTile
    {
        private IShader shader;
        private IStaticPhysicsObject bounding_object;

        private string shader_identifier;
        private string image_path;

        private Texture image;

        private int VertexArrayObject;
        private int ElementBufferObject;
        private int VertexBufferObject;
        private uint[] indices = {0,1,2, // first triangle (bottom left - top left - top right)
                     0,2,3};
        public RenderQuadGraphic(IStaticPhysicsObject bounding_object, string shader_program, string image_file)
        {
            this.shader_identifier = shader_program;
            this.image_path = image_file;
            this.bounding_object = bounding_object;
            GraphicsRegistry.registerRenderGraphic(this);
        }


        
        public void LoadGraphic()
        {
            shader = ShaderManager.getAll().GetValueOrDefault(shader_identifier);
            image = TextureManager.getOrLoadTexture(this.image_path);

            ElementBufferObject = GL.GenBuffer();
            




            //Console.WriteLine(image.Handle); 



            VertexArrayObject = GL.GenVertexArray();
           
            VertexBufferObject = GL.GenBuffer();
            

        }

        public void DisposeGraphic()
        {
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteVertexArray(ElementBufferObject);
        }

        public bool DrawImage(float zoom, Vector2 offset)
        {
            float[] vertices = {
                (this.bounding_object.bounding_box.x_min * zoom)+offset.X, (this.bounding_object.bounding_box.y_max * zoom)+offset.Y, 0.0f, 0.0f, 0.0f,  // lower-left corner  
                (this.bounding_object.bounding_box.x_min * zoom)+offset.X, (this.bounding_object.bounding_box.y_min * zoom)+offset.Y, 0.0f, 0.0f, 1.0f,  // top-left corner
                (this.bounding_object.bounding_box.x_max * zoom)+offset.X, (this.bounding_object.bounding_box.y_min * zoom)+offset.Y, 0.0f, 1.0f, 1.0f,   // top-right corner
                (this.bounding_object.bounding_box.x_max * zoom)+offset.X, (this.bounding_object.bounding_box.y_max * zoom)+offset.Y, 0.0f, 1.0f, 0.0f  // lower-right corner
                
            };


            


            int vertex_location = GL.GetAttribLocation(shader.Handle, "aPosition");

            //Console.WriteLine(vertex_location);
            GL.EnableVertexAttribArray(vertex_location);
            GL.VertexAttribPointer(vertex_location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            //Console.WriteLine(texCoordLocation);
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            image.Use();

            GL.BindVertexArray(VertexArrayObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            this.shader.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            return true;
        }

    }
}
