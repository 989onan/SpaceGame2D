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
using SpaceGame2D.enviroment.world.actors;
using SpaceGame2D.enviroment;
using SpaceGame2D.threads.Factory_Threads;

namespace SpaceGame2D.threads
{
    public class MainThread
    {

        public Task main_thread;
        public Main_PhysicsThread physics_thread;
        public Main_GraphicsThread graphics_thread;

        public static List<FactorySubThread> factories = new List<FactorySubThread>();

        public static MainThread Instance { get; private set; }

        private DateTime last_time;

        public readonly DateTime gamestart;

        public bool is_running { get; private set; }

        public Player player;

        public SelectionRedicule selectedCube;
        //public Player player2;

        public IWorld cur_world;

        public MainThread()
        {
            if(MainThread.Instance != null)
            {
                return; //don't start game if it already started.
            }
            MainThread.Instance = this;
            this.gamestart = DateTime.Now;

            SpeciesRegistry.GenerateList();
            


            this.graphics_thread = new Main_GraphicsThread();
            this.graphics_thread.Window.Unload += Stop;
            //player = new Player(new Vector2(-1f, 1.1f), SpeciesRegistry.getSpecies("SpaceGame2D:Human"));
            //TODO: Remove these!
            selectedCube = new SelectionRedicule("SpaceGame2D:default");
            player = new Player(new Vector2(0,20), SpeciesRegistry.getSpecies("SpaceGame2D:Avali"));

            

            cur_world = new Planet("Earth", new Point(200, 200), new WorldEnviromentProperties(new Vector2(0, -9.8f),2));
            
            Random random = new Random();
            new WorldGenerator(random.Next(), cur_world.BlockGrid).generate();


            cur_world.BlockGrid.RenderOffset = new Vector2(0, 0f);
            cur_world.BlockGrid.RecalculateVoxelSimplification();
            //player2 = new Player(new Vector2(1f,0f), Avali.instance);



            /*Human instance = Human.Instance;
            IWorld spawn_planet = new Planet(universes[0], new Size(1, 1), new Vector2(0, -9.8f), "Avalon_Beta");


            universes[0].worlds.Add(spawn_planet);
            spawn_planet.enviroment.setTile(new Point(0,0), new EarthGrass(new Point(0, 0), spawn_planet.enviroment));
            spawn_planet.enviroment.setTile(new Point(0,-1), new EarthGrass(new Point(0, -1), spawn_planet.enviroment));
            Player player = new Player(instance, spawn_planet);
            players.Add(player);
            cur_world = spawn_planet;*/




            main_thread = new Task(async () =>
            {


                if (!this.graphics_thread.is_running)
                {
                    await Task.Delay(1);
                }
                this.is_running = true;
                while (this.is_running)
                {
                    DateTime now = DateTime.Now;
                    await iterate(now);
                    last_time = DateTime.Now;
                }
            });



            this.physics_thread = new Main_PhysicsThread();




            
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

        private async Task iterate(DateTime now)
        {
            double totaltime = (DateTime.Now - this.gamestart).TotalSeconds;
            if (totaltime > 3000)
            {
                //this.Stop();
            }
            else
            {
                
            }

        }
    }
}
