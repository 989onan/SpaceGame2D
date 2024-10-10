
using OpenTK.Graphics.ES11;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceGame2D.enviroment;
using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.species;
using SpaceGame2D.enviroment.world;
using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.utilities.math;
using SpaceGame2D.utilities.physicsSolver;
using SpaceGame2D.utilities.threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace SpaceGame2D.threads.PhysicsThread
{

    
    public class Main_PhysicsThread
    {

        public Task main_thread;

        private bool is_running;

        public bool baked = false;

        public static PhysicsSolver<IStaticPhysicsObject, IActivePhysicsObject> solver = new PhysicsSolver<IStaticPhysicsObject, IActivePhysicsObject>(new AABB(-10, -10, BlockGrid.size_grid * 210, BlockGrid.size_grid * 210));

        private DateTime last_time;

        public Main_PhysicsThread()
        {

            //I wish I could have atomic physics, but because of floating point error and how fast
            //an infinitely fast clock would be, I have to restrict it to running at miniumum 100 times a second to prevent issues.
            TimeSpan millisecond_clock = TimeSpan.FromMilliseconds(10);




            this.main_thread = new Task(async () =>
            {
                //this.is_running = true;

                while (!MainThread.Instance.is_running)
                {
                    await Task.Delay(1);
                }

                last_time = DateTime.Now;
                await Task.Delay(millisecond_clock);
                while (this.is_running)
                {
                    DateTime now = DateTime.Now;
                    iterate(now);
                    last_time = DateTime.Now;
                    


                    TimeSpan delta_times_secs = (last_time - now);
                    if ((millisecond_clock - delta_times_secs).TotalSeconds < 0f && baked)
                    {

                        Console.WriteLine("Physics engine has been saturated at a capacity of: " + solver.active_physics_objects.Count().ToString() + " skipping!");
                        last_time = now + millisecond_clock;
                        //return;
                    }
                    else
                    {
                        //Console.WriteLine("Physics engine baked, fully starting.");
                    }
                    if (!baked)
                    {
                        baked = true;
                    }
                    //Console.WriteLine("iterating physics.");

                    if ((millisecond_clock - delta_times_secs) > TimeSpan.Zero) //to prevent from delaying none.
                    {
                        await Task.Delay(millisecond_clock - delta_times_secs); //a-hah! Now we can have very accurate physics time clock.
                    }

                    
                }
            }
            );
            this.is_running = true;

            this.main_thread.Start();
        }


        public void Stop()
        {
            this.is_running = false;
        }

        public static void QueueKeyUp(KeyboardKeyEventArgs key)
        {
            pressed_keys.Enqueue(new Tuple<KeyboardKeyEventArgs, bool>(key, false));
        }

        public static void QueueKeyDown(KeyboardKeyEventArgs key)
        {
            pressed_keys.Enqueue(new Tuple<KeyboardKeyEventArgs, bool>(key, true));
        }

        //bool is: iskeydown?
        public static ConcurrentQueue<Tuple<KeyboardKeyEventArgs, bool>> pressed_keys = new ConcurrentQueue<Tuple<KeyboardKeyEventArgs, bool>>();

        public DateTime last_break { get; private set; }
        public void ControlPlayer(float delta_times_secs, DateTime now)
        {
            GameWindow window = MainThread.Instance.graphics_thread.Window;
            Vector2 vel = MainThread.Instance.player.velocity;
            ISpecies species = MainThread.Instance.player.species;
            Player player = MainThread.Instance.player;
            if (!player.JetPackOn) {
                if (window.IsKeyDown(Keys.D))
                {
                    vel = new Vector2(player.ground != null ? species.walk_speed : species.walk_speed/*vel.X*/, vel.Y);
                }
                if (window.IsKeyDown(Keys.A))
                {
                    vel = new Vector2(player.ground != null ? -species.walk_speed : -species.walk_speed/*vel.X*/, vel.Y);
                }
                if (!(window.IsKeyDown(Keys.D) || window.IsKeyDown(Keys.A)))
                {
                    if(MainThread.Instance.cur_world.enviro.air_resistance_factor <= 0)
                    {
                        vel = new Vector2(vel.X, vel.Y);
                    }
                    else
                    {
                        vel = new Vector2(vel.X / MainThread.Instance.cur_world.enviro.air_resistance_factor, vel.Y);
                    }
                }
            }
            else
            {
                float jetpackvel = (Player.JetPackForce / species.weight_kg)*delta_times_secs;
                Console.WriteLine(jetpackvel);
                if (window.IsKeyDown(Keys.D))
                {
                    vel = new Vector2(jetpackvel + vel.X, vel.Y);
                }
                if (window.IsKeyDown(Keys.A))
                {
                    vel = new Vector2(-jetpackvel + vel.X, vel.Y);
                }
                if (window.IsKeyDown(Keys.W))
                {
                    // + vel.Y
                    vel = new Vector2(vel.X, jetpackvel + vel.Y);
                }
                if (window.IsKeyDown(Keys.Space))
                {
                    // + vel.Y
                    Vector2 sign = new Vector2(Math.Sign(vel.X), Math.Sign(vel.Y));
                    vel = ((-sign * jetpackvel) + vel);
                    Vector2 sign2 = new Vector2(Math.Sign(vel.X), Math.Sign(vel.Y));
                    if (sign.X != sign2.X || sign.Y != sign2.Y)
                    {
                        vel = Vector2.Zero;
                    }
                    if (Vector2.Distance(Vector2.Zero, vel) < .1f)
                    {
                        vel = Vector2.Zero;
                    }
                }
                if (window.IsKeyDown(Keys.S))
                {
                    vel = new Vector2(vel.X, -jetpackvel + vel.Y);
                }
            }
            

            

            float WindowRatio;

            Vector2 MouseCenter = new Vector2(((window.MousePosition.X - (window.Size.X / 2)) / (window.Size.X / 2)), -1.2f * ((window.MousePosition.Y - (window.Size.Y / 2)) / (window.Size.Y)));

            if (window.Size.X > window.Size.Y)
            {
                WindowRatio = window.Size.X / window.Size.Y;
                //MouseCenter = new Vector2(((window.MousePosition.X / window.Size.X) - .5f) * 2, ((window.MousePosition.Y / -window.Size.Y) + .5f) * 2);
            }
            else
            {
                WindowRatio = window.Size.Y / window.Size.X;
            }


            float meters_reach_to_screen = species.reach_meters;


            Vector2 MouseClamped = new Vector2(Math.Clamp((MouseCenter / MainThread.Instance.graphics_thread.Zoom).X, -meters_reach_to_screen, meters_reach_to_screen), Math.Clamp((MouseCenter / MainThread.Instance.graphics_thread.Zoom).Y, -meters_reach_to_screen, meters_reach_to_screen));

            MainThread.Instance.graphics_thread.PhysicalMousePosition = MouseClamped + MainThread.Instance.player.position_physics;

            //get voxels that are actually blocks.
            IEnumerable<IBlock> collection = solver.static_physics_objects.FindCollisions(new AABB(MainThread.Instance.player.Collider).ExtendByVector(MouseClamped)).OfType<IBlock>();
            IEnumerable<IBlock> ray_intercection = AABBMath<IBlock>.SweptAABBScene(collection, MainThread.Instance.player, new Vector2(
                MouseClamped.X,

                MouseClamped.Y

                ));
            //Console.WriteLine(collection.Count())
            //Console.WriteLine(ray_interection.Count());
            if (ray_intercection.Count() > 0)
            {


                MainThread.Instance.selectedCube.target_block = ray_intercection.First();
                //Console.WriteLine(MainThread.Instance.selectedCube.target_block.block_position.ToString());
            }
            else
            {
                MainThread.Instance.selectedCube.target_block = null;
                MainThread.Instance.selectedCube.alt_position = MainThread.Instance.graphics_thread.PhysicalMousePosition;
            }


            


            if(MainThread.Instance.selectedCube.target_block != null)
            {
                if ((now-last_break).TotalSeconds > .8f)
                {
                    if (window.IsMouseButtonDown(MouseButton.Button1))
                    {
                        IBlock block = MainThread.Instance.selectedCube.target_block;
                        BlockGrid grid = block.grid;
                        block.Mine();
                        grid.deleteTileLocation(block.block_position);
                        

                        last_break = now;
                    }
                }
                
            }


            //buffered keys, for instant actions.
            while (pressed_keys.TryDequeue(out Tuple<KeyboardKeyEventArgs,bool> key))
            {
                if (key.Item1.Key == Keys.X && key.Item2)
                {
                    player.JetPackOn = !player.JetPackOn;
                }
                if (key.Item1.Key == Keys.Space && key.Item2)
                {
                    if (player.ground != null)
                    {
                        vel = new Vector2(vel.X, species.jump_velocity);
                        //Console.WriteLine("jumping!");
                    }
                }

                if (key.Item1.Key == Keys.E && key.Item2)
                {
                    
                    if (!player.storageScreen.IsVisible)
                    {
                        player.storageScreen.OpenUI();
                    }
                    else
                    {
                        player.storageScreen.CloseGui();

                    }
                }
                
            }

            player.velocity = vel;
        }





        private void iterate(DateTime now)
        {

            
            float delta_times_secs = (float)((now - last_time).TotalSeconds);

            ControlPlayer(delta_times_secs, now);

            solver.iterate((now - last_time));

        }

        

        


    }
}
