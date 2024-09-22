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

        public TextureTile image => this.graphicObject.UpdateCurrentImage();

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

        public bool DrawImage(float zoom, Vector2 offset, float window_height, float window_width)
        {
            float WindowRatio;
            if (window_width > window_height)
            {
                WindowRatio = window_width / window_height;
            }
            else
            {
                WindowRatio = window_height / window_width;
            }
            AABB position = AABB.Size_To_AABB(this.graphicObject.GraphicCenterPosition, this.graphicObject.graphic_size);





            //offset *= zoom;
            AABB screen = new AABB(new Vector2(-1.2f, -1.2f), new Vector2(1.2f, 1.2f));
            //Console.WriteLine(this.graphicObject.UpdateCurrentImage().width);
            float[] vertices;
            //get position of object, and also find graphic position on atlas.
            if (window_height > window_width)
            {
                AABB graphic_on_screen = new AABB(position);
                
                graphic_on_screen.y_max = ((position.y_max + offset.Y) * zoom);
                graphic_on_screen.y_min = ((position.y_min + offset.Y) * zoom);
                graphic_on_screen.x_min = ((position.x_min + offset.X) * WindowRatio) * zoom;
                graphic_on_screen.x_max = ((position.x_max + offset.X) * WindowRatio) * zoom;


                if (!graphic_on_screen.Intercects(screen))
                {
                    return false;
                }

                offset.Y *= WindowRatio;
                //Console.WriteLine("fire!");
                vertices = new float[]{
                    graphic_on_screen.x_min, graphic_on_screen.y_max, 0.0f, this.graphicObject.UpdateCurrentImage().x, this.graphicObject.UpdateCurrentImage().y,  // lower-left corner  
                    graphic_on_screen.x_min, graphic_on_screen.y_min, 0.0f, this.graphicObject.UpdateCurrentImage().x, this.graphicObject.UpdateCurrentImage().height + this.graphicObject.UpdateCurrentImage().y,  // top-left corner
                    graphic_on_screen.x_max, graphic_on_screen.y_min, 0.0f, this.graphicObject.UpdateCurrentImage().x + this.graphicObject.UpdateCurrentImage().width, this.graphicObject.UpdateCurrentImage().height + this.graphicObject.UpdateCurrentImage().y,   // top-right corner
                    graphic_on_screen.x_max, graphic_on_screen.y_max, 0.0f, this.graphicObject.UpdateCurrentImage().x + this.graphicObject.UpdateCurrentImage().width, this.graphicObject.UpdateCurrentImage().y  // lower-right corner
                };
            }
            else
            {
                AABB graphic_on_screen = new AABB(position);
                graphic_on_screen.y_max = ((position.y_max + offset.Y) * WindowRatio) * zoom;
                graphic_on_screen.y_min = ((position.y_min + offset.Y) * WindowRatio) * zoom;
                graphic_on_screen.x_min = (position.x_min + offset.X) * zoom;
                graphic_on_screen.x_max = (position.x_max + offset.X) * zoom;

                if (!graphic_on_screen.Intercects(screen))
                {
                    return false;
                }
                offset.X *= WindowRatio;
                //
                vertices = new float[]{
                    graphic_on_screen.x_min, graphic_on_screen.y_max, 0.0f, this.graphicObject.UpdateCurrentImage().x, this.graphicObject.UpdateCurrentImage().y,  // lower-left corner  
                    graphic_on_screen.x_min, graphic_on_screen.y_min, 0.0f, this.graphicObject.UpdateCurrentImage().x, this.graphicObject.UpdateCurrentImage().height + this.graphicObject.UpdateCurrentImage().y,  // top-left corner
                    graphic_on_screen.x_max, graphic_on_screen.y_min, 0.0f, this.graphicObject.UpdateCurrentImage().x + this.graphicObject.UpdateCurrentImage().width, this.graphicObject.UpdateCurrentImage().height + this.graphicObject.UpdateCurrentImage().y,   // top-right corner
                    graphic_on_screen.x_max, graphic_on_screen.y_max, 0.0f, this.graphicObject.UpdateCurrentImage().x + this.graphicObject.UpdateCurrentImage().width, this.graphicObject.UpdateCurrentImage().y  // lower-right corner
                };
            }
            

            /*float[] vertices = new float[]{
                (position.x_min + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, 0f, 0f,  // lower-left corner  
                (position.x_min + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, 0f, 1.0f,  // top-left corner
                (position.x_max + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, 1.0f, 1.0f,   // top-right corner
                (position.x_max + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, 1.0f, 0f  // lower-right corner
                
            };*/

            if (!Atlas.isLoaded) return false;
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
