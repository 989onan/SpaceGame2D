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
using SpaceGame2D.graphics.renderables;

namespace SpaceGame2D.threads.GraphicsThread
{
    public class Main_GraphicsThread
    {
        public GameWindow Window { get; set; }

        public bool is_running { get; private set; }

        private DateTime last_time;

        public Vector2 PhysicalMousePosition { get; set; }
        public float Zoom { get; set; }

        private int seconds_recognized;

        public Vector2 window_size { get; private set; }


        public Shader default_shader;

        public delegate void RegisterImage();

        public static event RegisterImage RegisterImages;
        //this needs to be run last when making threads.
        public Main_GraphicsThread()
        {
            this.Window = new GameWindow(new GameWindowSettings(), new NativeWindowSettings());
            
            


            //Console.WriteLine(GL.GetProgramInfoLog(default_shader));

            this.Zoom = .08f;
            this.Window.RenderFrame += Render;
            
            this.Window.FramebufferResize += OnFramebufferResize;
            this.Window.Load += OnLoad;
            this.Window.Unload += OnDispose;
            this.Window.Size = new OpenTK.Mathematics.Vector2i(800, 800);

            //register shaders
            ShaderManager.register("SpaceGame2D:default", new Shader("graphics/shaders/default.vert", "graphics/shaders/default.frag"));
        }

        private void OnDispose()
        {
            GraphicsRegistry.DisposeBuffers();
            
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
            GL.MatrixMode(MatrixMode.Modelview);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            //GL.Enable(EnableCap.Texture2D);
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            
            Atlas.RegenerateAtlas();
            Atlas.LoadToGPU();
            Atlas.UseImage();
            GraphicsRegistry.LoadBuffers();
            this.is_running = true;
            window_size = new Vector2(Window.Size.X, Window.Size.Y);
            GL.Viewport(0, 0, Window.Size.X, Window.Size.Y);
            
        }

        public void Stop()
        {
            this.is_running = false;
            this.Window.Close();
            
        }

        private void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            GL.Viewport(0, 0, Window.Size.X, Window.Size.Y);

            window_size = new Vector2(Window.Size.X, Window.Size.Y);
        }


        private void Render(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(142f/225f, 202f/255f,1f,1f);
            //GL.Disable(EnableCap.DepthTest);
            DateTime now = DateTime.Now;
            
            //GL.Clear(ClearBufferMask.ColorBufferBit);


            double deltatime = (now - this.last_time).TotalSeconds;



            if ((now - MainThread.Instance.gamestart).TotalSeconds > seconds_recognized)
            {
                seconds_recognized = (int)(now - MainThread.Instance.gamestart).TotalSeconds + 1;
                //Console.WriteLine("second passed on graphics thread. seconds:" + seconds_recognized.ToString());
                //Console.WriteLine("FPS:" + (1/deltatime).ToString());
            }

            //Console.WriteLine("tick");
            //GL.C

            Vector2 physics_pos = MainThread.Instance.player.position_physics;

            float animation_time = (float)(now-MainThread.Instance.gamestart).TotalSeconds*100;

            foreach (IRenderableWorldGraphic obj in GraphicsRegistry.getAllWorldGraphics())
            {
                //iterate through all blocks that need to be rendered.
                if(obj == null) continue;
                obj.DrawImage(Zoom, -physics_pos, this.window_size, animation_time);
                
            }

            MainThread.Instance.selectedCube.DrawImage(Zoom, -physics_pos, this.window_size, animation_time);
            foreach (IRenderable obj in GraphicsRegistry.getAllRenderGraphics())
            {
                //iterate through all blocks that need to be rendered.
                obj.Draw(animation_time, this.window_size);
            }


            this.Window.SwapBuffers();
            this.last_time = DateTime.Now;
        }
    }
}
