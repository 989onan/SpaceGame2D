using SpaceGame2D.enviroment;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using SpaceGame2D.threads.PhysicsThread;
using SpaceGame2D.threads.GraphicsThread;
using SpaceGame2D.enviroment.species;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.enviroment.world;

namespace SpaceGame2D.threads
{
    public class MainThread
    {

        public Thread main_thread;
        public Main_PhysicsThread physics_thread;
        public Main_GraphicsThread graphics_thread;


        private DateTime last_time;

        public readonly DateTime gamestart;

        public bool is_running { get; private set; }

        public Player player;
        public Player player2;

        public IWorld cur_world;

        public MainThread()
        {
            this.gamestart = DateTime.Now;

            SpeciesRegistry.GenerateList();
            
            this.graphics_thread = new Main_GraphicsThread(this);
            this.graphics_thread.Window.Unload += Stop;
            //player = new Player(new Vector2(-1f, 1.1f), SpeciesRegistry.getSpecies("SpaceGame2D:Human"));
            player = new Player(new Vector2(0,2), SpeciesRegistry.getSpecies("SpaceGame2D:Avali"));

            cur_world = new Planet("avalon", new Point(200, 200), new Vector2(0, -9.8f));
            new WorldGenerator(99, cur_world.BlockGrid);

            cur_world.BlockGrid.RenderOffset = new Vector2(-1, -1f);
            //player2 = new Player(new Vector2(1f,0f), Avali.instance);


            //TODO: Remove these!
            /*Human instance = Human.Instance;
            IWorld spawn_planet = new Planet(universes[0], new Size(1, 1), new Vector2(0, -9.8f), "Avalon_Beta");


            universes[0].worlds.Add(spawn_planet);
            spawn_planet.enviroment.setTile(new Point(0,0), new EarthGrass(new Point(0, 0), spawn_planet.enviroment));
            spawn_planet.enviroment.setTile(new Point(0,-1), new EarthGrass(new Point(0, -1), spawn_planet.enviroment));
            Player player = new Player(instance, spawn_planet);
            players.Add(player);
            cur_world = spawn_planet;*/




            main_thread = new Thread(new ThreadStart(() =>
            {
                
                if (!this.graphics_thread.is_running)
                {
                    Thread.Sleep(1);
                }
                this.is_running = true;
                while (this.is_running)
                {
                    iterate();
                    last_time = DateTime.Now;
                }
            }));



            this.physics_thread = new Main_PhysicsThread(this);




            
            main_thread.Start();
            Console.WriteLine("boot initalized!");


            
            this.graphics_thread.Window.Run();//run last.
        }

        public void KeyPressed(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.D)
            {
                player.velocity = new Vector2(1f * player.species.walk_speed, player.velocity.Y);
            }
            if (e.Key == Keys.A)
            {
                player.velocity = new Vector2(-1f * player.species.walk_speed, player.velocity.Y);
            }
            if(e.Key == Keys.Space)
            {
                if (player.OnGround)
                {
                    player.velocity = new Vector2(player.velocity.X, player.species.jump_velocity);
                }
                else
                {
                    Console.WriteLine("failed ground check");
                }
            }
            //Console.WriteLine("pressed key.");
            
        }

        public void KeyReleased(KeyboardKeyEventArgs e)
        {
            if (e.Key == Keys.D)
            {
                player.velocity = new Vector2(0f, player.velocity.Y);
            }
            if (e.Key == Keys.A)
            {
                player.velocity = new Vector2(0f, player.velocity.Y);
            }
            //Console.WriteLine("pressed key.");

        }

        public void Stop()
        {
            Console.WriteLine("stopping!");
            this.physics_thread.Stop();
            try
            {
                this.graphics_thread.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
            
            this.is_running = false;
        }

        private void iterate()
        {
            double totaltime = (DateTime.Now - this.gamestart).TotalSeconds;
            if (totaltime > 3000)
            {
                this.Stop();
            }
            else
            {
                //this.graphics_thread.Zoom = (float)(totaltime * .01);
            }

        }
    }
}
