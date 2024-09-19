using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.utilities.math;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;
using System.Resources;
using System.Reflection.Metadata;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.texturemanager.packer;

namespace SpaceGame2D.threads.GraphicsThread
{
    public class Main_GraphicsThread
    {
        public GameWindow Window { get; set; }

        private bool is_running;

        private DateTime last_time;

        public float Zoom { get; set; }

        private int seconds_recognized;

        public readonly MainThread source_thread;

        public Shader default_shader;
        //this needs to be run last when making threads.
        public Main_GraphicsThread(MainThread source_thread)
        {
            this.Window = new GameWindow(new GameWindowSettings(), new NativeWindowSettings());
            
            this.is_running = true;
            this.source_thread = source_thread;


            //Console.WriteLine(GL.GetProgramInfoLog(default_shader));

            this.Zoom = 1f;
            this.Window.RenderFrame += Render;
            this.Window.FramebufferResize += OnFramebufferResize;
            this.Window.Load += OnLoad;
            this.Window.Unload += OnDispose;


            //register shaders
            ShaderManager.register("SpaceGame2D:default", new Shader("graphics/shaders/default.vert", "graphics/shaders/default.frag"));
        }

        private void OnDispose()
        {
            
            
        }


        private void OnLoad()
        {
            ShaderManager.LoadAll();
            Texture null_texture = new Texture("null.png");
            Atlas.AddToQue(null_texture);
            Atlas.RegenerateAtlas();
            Atlas.LoadToGPU();
            Atlas.UseImage();
            GraphicsRegistry.LoadAll();
        }

        public void Stop()
        {
            this.is_running = false;
            this.Window.Close();
            
        }

        private void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
        }


        private void Render(FrameEventArgs e)
        {
            
            DateTime now = DateTime.Now;
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if ((now - this.source_thread.gamestart).TotalSeconds > seconds_recognized)
            {
                seconds_recognized++;
                Console.WriteLine("second passed on graphics thread. seconds:" + seconds_recognized.ToString());
            }

            foreach (IRenderableGraphic obj in GraphicsRegistry.getAll())
            {
                obj.DrawImage(Zoom, new System.Numerics.Vector2(0, 0));
            }






            this.Window.SwapBuffers();
            this.last_time = DateTime.Now;
        }
    }
}
