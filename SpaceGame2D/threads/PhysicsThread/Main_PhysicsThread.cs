using SpaceGame2D.enviroment.Entities;
using SpaceGame2D.utilities.math;
using SpaceGame2D.utilities.threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceGame2D.threads.PhysicsThread
{
    public class Main_PhysicsThread
    {

        public static ConcurrentList<IMoveableEntity> activePhysicsObjects = new ConcurrentList<IMoveableEntity>();

        public static ConcurrentList<IStaticPhysicsObject> StaticPhysicsObjects = new ConcurrentList<IStaticPhysicsObject>();

        public Thread main_thread;

        public readonly MainThread source_thread;

        private bool is_running;

        private DateTime last_time;

        public Main_PhysicsThread(MainThread source_thread)
        {

            this.main_thread = new Thread(new ThreadStart( () =>
            {
                while (this.is_running)
                {
                    iterate();
                    last_time = DateTime.Now;
                }
            }
            ));
            this.is_running = true;

            this.main_thread.Start();
            this.source_thread = source_thread;
        }


        public void Stop()
        {
            this.is_running = false;
        }


        private void iterate()
        {
            DateTime now = DateTime.Now;

            foreach (IMoveableEntity entity in activePhysicsObjects)
            {
                        

                entity.bounding_box.Center = entity.bounding_box.Center + (entity.velocity * (float)(now - last_time).TotalSeconds);

                        
                    

                foreach (IStaticPhysicsObject obj in StaticPhysicsObjects.Where((obj) => obj.World == entity.World))
                {
                    


                    Console.WriteLine("object collided with other object!");
                }
            }
            








        }
    }
}
