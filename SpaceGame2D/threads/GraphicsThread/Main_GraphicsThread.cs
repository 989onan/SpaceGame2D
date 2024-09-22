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
using SpaceGame2D.enviroment.species;
using SpaceGame2D.enviroment.blocks;
using System.Numerics;

namespace SpaceGame2D.threads.GraphicsThread
{
    public class Main_GraphicsThread
    {
        public GameWindow Window { get; set; }

        public bool is_running { get; private set; }

        private DateTime last_time;

        public float Zoom { get; set; }

        private int seconds_recognized;

        public float window_width { get; private set; }
        public float window_height { get; private set; }

        public readonly MainThread source_thread;

        public Shader default_shader;

        public delegate void RegisterImage();

        public static event RegisterImage RegisterImages;
        //this needs to be run last when making threads.
        public Main_GraphicsThread(MainThread source_thread)
        {
            this.Window = new GameWindow(new GameWindowSettings(), new NativeWindowSettings());
            
            
            this.source_thread = source_thread;


            //Console.WriteLine(GL.GetProgramInfoLog(default_shader));

            this.Zoom = .1f;
            this.Window.RenderFrame += Render;
            this.Window.KeyDown += source_thread.KeyPressed;
            this.Window.KeyUp += source_thread.KeyReleased;
            this.Window.FramebufferResize += OnFramebufferResize;
            this.Window.Load += OnLoad;
            this.Window.Unload += OnDispose;
            this.Window.Size += new OpenTK.Mathematics.Vector2i(800, 800);

            //register shaders
            ShaderManager.register("SpaceGame2D:default", new Shader("graphics/shaders/default.vert", "graphics/shaders/default.frag"));
        }

        private void OnDispose()
        {
            
            
        }

        protected virtual void RaiseLoadImageEvent()
        {
            // Raise the event in a thread-safe manner using the ?. operator.
            RegisterImages?.Invoke();
        }


        private void OnLoad()
        {
            ShaderManager.LoadAll();
            RaiseLoadImageEvent();
            

            Atlas.RegenerateAtlas();
            Atlas.LoadToGPU();
            Atlas.UseImage();
            GraphicsRegistry.LoadAll();
            this.is_running = true;
            this.window_height = Window.Size.Y;
            this.window_width = Window.Size.X;
            //GL.MatrixMode(MatrixMode.Modelview);
            
        }

        public void Stop()
        {
            this.is_running = false;
            this.Window.Close();
            
        }

        private void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);

            this.window_height = e.Height;
            this.window_width = e.Width;
        }


        private void Render(FrameEventArgs e)
        {
            
            DateTime now = DateTime.Now;
            
            GL.Clear(ClearBufferMask.ColorBufferBit);


            double deltatime = (now - this.last_time).TotalSeconds;



            if ((now - this.source_thread.gamestart).TotalSeconds > seconds_recognized)
            {
                seconds_recognized = (int)(now - this.source_thread.gamestart).TotalSeconds + 1;
                Console.WriteLine("second passed on graphics thread. seconds:" + seconds_recognized.ToString());
                Console.WriteLine("FPS:" + (1/deltatime).ToString());
            }

            //Console.WriteLine("tick");
            //GL.C
            Vector2 physics_pos = this.source_thread.player.position_physics;
            foreach (IRenderableGraphic obj in GraphicsRegistry.getAll())
            {
                //iterate through all blocks that need to be rendered.

                obj.DrawImage(Zoom, -physics_pos, this.window_height, this.window_width);
            }



            this.Window.SwapBuffers();
            this.last_time = DateTime.Now;
        }
    }
}
