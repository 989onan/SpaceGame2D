using SpaceGame2D.enviroment;
using SpaceGame2D.enviroment.Entities;
using SpaceGame2D.enviroment.Entities.Species;
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

namespace SpaceGame2D.threads
{
    public class MainThread
    {
        public List<Universe> universes = new List<Universe>();

        public List<LivingEntity> players = new List<LivingEntity>();

        public Thread main_thread;
        public Main_PhysicsThread physics_thread;
        public Main_GraphicsThread graphics_thread;

        public IWorld cur_world;

        private DateTime last_time;

        public readonly DateTime gamestart;

        private bool is_running;


        public MainThread()
        {
            this.gamestart = DateTime.Now;
            universes.Add(new Universe());

            //TODO: Remove these!
            IWorld spawn_planet = new Planet(universes[0], new Size(200, 200), new Vector2(0, -9.8f), "Avalon_Beta");
            universes[0].worlds.Add(spawn_planet);
            Player player = new Player(Human.Instance, spawn_planet);
            players.Add(player);
            cur_world = spawn_planet;


            main_thread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                while (this.is_running)
                {
                    iterate();
                    last_time = DateTime.Now;
                }
            }));



            this.physics_thread = new Main_PhysicsThread(this);

            Main_PhysicsThread.activePhysicsObjects.Add(player);



            this.is_running = true;
            main_thread.Start();
            Console.WriteLine("boot initalized!");


            this.graphics_thread = new Main_GraphicsThread(this);
            this.graphics_thread.Window.Unload += Stop;
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
