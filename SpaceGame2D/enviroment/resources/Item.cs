﻿using OpenTK.Graphics.OpenGL;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;
using SpaceGame2D.utilities.ThreadSafePhysicsSolver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.resources
{
    public class Item: IPositionedRenderable
    {
        public string Name => LangTranslate.translate(this.UniqueIdentifier);

        string ImageName { get; set; }

        public string UniqueIdentifier { get; }

        public IShader shader { get; }

        public int order => 11;

        public Item(string ImageName, string ItemID, IShader shader)
        {
            this.ImageName = ImageName;
            this.UniqueIdentifier = ItemID;
            this.shader = shader;   

        }

        public TextureTileFrame ItemGraphic(float animation_time)
        {
            return Atlas.getTextureAnimated(ImageName, animation_time);
        }

        public void destruct()
        {
            throw new NotImplementedException();
        }

        public bool DrawImage(Vector2 position, Vector2 size, Vector2 game_window_size, float animationtime)
        {
            float WindowRatio;
            if (game_window_size.X > game_window_size.Y)
            {
                WindowRatio = game_window_size.X / game_window_size.Y;
            }
            else
            {
                WindowRatio = game_window_size.Y / game_window_size.X;
            }
            AABB position_aabb = AABB.Size_To_AABB(position, size);


            float depth = -.5f + order / 10;
            TextureTileFrame image = ItemGraphic(animationtime);
            Vector2 start = new Vector2(image.x, image.y);
            Vector2 end = new Vector2(image.x + image.width, image.y + image.height);
            AABB screen = new AABB(new Vector2(-1.2f, -1.2f), new Vector2(1.2f, 1.2f));
            float[] vertices;
            if (game_window_size.Y > game_window_size.X)
            {
                AABB graphic_on_screen = new AABB(position_aabb);

                graphic_on_screen.y_max = (position_aabb.y_max);
                graphic_on_screen.y_min = (position_aabb.y_min);
                graphic_on_screen.x_min = (position_aabb.x_min * WindowRatio);
                graphic_on_screen.x_max = (position_aabb.x_max * WindowRatio);


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
                AABB graphic_on_screen = new AABB(position_aabb);
                graphic_on_screen.y_max = (position_aabb.y_max * WindowRatio);
                graphic_on_screen.y_min = (position_aabb.y_min * WindowRatio);
                graphic_on_screen.x_min = (position_aabb.x_min);
                graphic_on_screen.x_max = (position_aabb.x_max);

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



            GL.DrawElements(PrimitiveType.Triangles, GraphicsRegistry.indices_Quad.Length, DrawElementsType.UnsignedByte, 0);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            return true;
        }

        public bool DrawImage(float animationtime, Vector2 game_window_size)
        {
            Console.WriteLine("trying to draw world graphic as an IRenderable! DON'T DO THIS EVER!!");
            return DrawImage(new Vector2(0, 0), new Vector2(0, 0), game_window_size, animationtime);
        }

        


    }
}
