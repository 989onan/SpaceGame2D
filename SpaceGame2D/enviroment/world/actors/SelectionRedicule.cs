using OpenTK.Graphics.OpenGL;
using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.world.actors
{
    public class SelectionRedicule : IRenderableWorldGraphic
    {

        public IShader shader { get; private set; }

        public Vector2 GraphicCenterPosition => getPosition();
        private Vector2 getPosition()
        {
            if (target_block != null)
            {
                if(target_block.grid != null)
                {
                    return target_block.position_physics;
                }
                
            }
            return alt_position;
        }
        public TextureTileFrame UpdateCurrentImage(float animation_time)
        {
            return Atlas.getTextureAnimated("ui/selection.png", animation_time);
        }
        public IBlock target_block { get; set; }

        public Vector2 alt_position = new Vector2(0, 0);

        public SelectionRedicule(string shader_program)
        {
            shader = ShaderManager.getAll().GetValueOrDefault(shader_program);

        }



        public int order => 10;

        //public TextureTileFrame image => UpdateCurrentImage(0f);

        public bool DrawImage(float zoom, Vector2 offset, Vector2 window_size, float animation_time)
        {
            float WindowRatio;
            if (window_size.X > window_size.Y)
            {
                WindowRatio = window_size.X / window_size.Y;
            }
            else
            {
                WindowRatio = window_size.Y / window_size.X;
            }
            AABB position = AABB.Size_To_AABB(GraphicCenterPosition, new Vector2(.6f, .6f));


            float depth = -.5f + order / 10;



            AABB screen = new AABB(new Vector2(-1.2f, -1.2f), new Vector2(1.2f, 1.2f));
            float[] vertices;

            TextureTileFrame image = UpdateCurrentImage(animation_time);

            Vector2 start = new Vector2(image.x, image.y);
            Vector2 end = new Vector2(image.x + image.width, image.y + image.height);

            if (window_size.Y > window_size.X)
            {
                AABB graphic_on_screen = new AABB(position);

                graphic_on_screen.y_max = (position.y_max + offset.Y) * zoom;
                graphic_on_screen.y_min = (position.y_min + offset.Y) * zoom;
                graphic_on_screen.x_min = (position.x_min + offset.X) * WindowRatio * zoom;
                graphic_on_screen.x_max = (position.x_max + offset.X) * WindowRatio * zoom;


                if (!graphic_on_screen.Intercects(screen))
                {
                    return false;
                }



                if (image.flip_x)
                {
                    //flip on x only
                    Vector2 temp = start;
                    start = new Vector2(end.X, start.Y);
                    end = new Vector2(temp.X, end.Y);
                }

                vertices = new float[]{
                    graphic_on_screen.x_min, graphic_on_screen.y_max, depth, start.X, start.Y,  // lower-left corner  
                    graphic_on_screen.x_min, graphic_on_screen.y_min, depth, start.X, end.Y,  // top-left corner
                    graphic_on_screen.x_max, graphic_on_screen.y_min, depth, end.X, end.Y,   // top-right corner
                    graphic_on_screen.x_max, graphic_on_screen.y_max, depth, end.X, start.Y  // lower-right corner
                };
            }
            else
            {
                AABB graphic_on_screen = new AABB(position);
                graphic_on_screen.y_max = (position.y_max + offset.Y) * WindowRatio * zoom;
                graphic_on_screen.y_min = (position.y_min + offset.Y) * WindowRatio * zoom;
                graphic_on_screen.x_min = (position.x_min + offset.X) * zoom;
                graphic_on_screen.x_max = (position.x_max + offset.X) * zoom;

                if (!graphic_on_screen.Intercects(screen))
                {
                    return false;
                }

                if (image.flip_x)
                {
                    //flip on x only
                    Vector2 temp = start;
                    start = new Vector2(end.X, start.Y);
                    end = new Vector2(temp.X, end.Y);
                }

                vertices = new float[]{
                    graphic_on_screen.x_min, graphic_on_screen.y_max, depth, start.X, start.Y,  // lower-left corner  
                    graphic_on_screen.x_min, graphic_on_screen.y_min, depth, start.X, end.Y,  // top-left corner
                    graphic_on_screen.x_max, graphic_on_screen.y_min, depth, end.X, end.Y,   // top-right corner
                    graphic_on_screen.x_max, graphic_on_screen.y_max, depth, end.X, start.Y  // lower-right corner
                };
            }




            /*float[] vertices = new float[]{
                (position.x_min + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, 0f, 0f,  // lower-left corner  
                (position.x_min + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, 0f, 1.0f,  // top-left corner
                (position.x_max + offset.X) * zoom, (position.y_min + offset.Y) * zoom, 0.0f, 1.0f, 1.0f,   // top-right corner
                (position.x_max + offset.X) * zoom, (position.y_max + offset.Y) * zoom, 0.0f, 1.0f, 0f  // lower-right corner
                
            };*/

            if (!Atlas.isLoaded) return false;


            int vertex_location = GL.GetAttribLocation(shader.Handle, "aPosition");

            GL.EnableVertexAttribArray(vertex_location);
            GL.VertexAttribPointer(vertex_location, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            GL.BindVertexArray(GraphicsRegistry.VertexArrayObject_Quad);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StreamDraw);


            GL.BindBuffer(BufferTarget.ArrayBuffer, GraphicsRegistry.VertexBufferObject_Quad);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, GraphicsRegistry.ElementBufferObject_Quad);
            GL.BufferData(BufferTarget.ElementArrayBuffer, GraphicsRegistry.indices_Quad.Length * sizeof(uint), GraphicsRegistry.indices_Quad, BufferUsageHint.StreamDraw);


            OpenTK.Mathematics.Vector3 position_cam = new OpenTK.Mathematics.Vector3(0, 0, .1f);
            shader.SetMatrix4("view", OpenTK.Mathematics.Matrix4.LookAt(position_cam, position_cam + new OpenTK.Mathematics.Vector3(0, 0, -1), new OpenTK.Mathematics.Vector3(0, 1, 0)));
            Atlas.UseImage();
            shader.Use();



            GL.DrawElements(PrimitiveType.Triangles, GraphicsRegistry.indices_Quad.Length, DrawElementsType.UnsignedInt, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            return true;
        }

        public void Draw(float animationtime, Vector2 game_window_size)
        {
            Console.WriteLine("trying to draw world graphic as a IRenderable! DON'T DO THIS EVER!!");
            DrawImage(1, new Vector2(0, 0), game_window_size, animationtime);
        }


        public void destruct()
        {
            GraphicsRegistry.deregisterWorldRenderGraphic(this);
        }
    }
}
