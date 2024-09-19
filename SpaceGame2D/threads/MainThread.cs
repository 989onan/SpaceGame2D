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

namespace SpaceGame2D.threads
{
    public class MainThread
    {

        public Thread main_thread;
        public Main_PhysicsThread physics_thread;
        public Main_GraphicsThread graphics_thread;


        private DateTime last_time;

        public readonly DateTime gamestart;

        private bool is_running;

        public Player player;
        //public Player player2;


        public MainThread()
        {
            this.gamestart = DateTime.Now;
            
            this.graphics_thread = new Main_GraphicsThread(this);
            this.graphics_thread.Window.Unload += Stop;
            player = new Player(Vector2.Zero, Human.instance);
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
                this.is_running = true;
                if (!this.graphics_thread.is_running)
                {
                    Thread.Sleep(1);
                }
                
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
                this.graphics_thread.Zoom = (float)(totaltime * .01);
            }

        }
    }
}
