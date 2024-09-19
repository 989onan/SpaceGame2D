
using SpaceGame2D.enviroment.physics;
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

        public Thread main_thread;

        public readonly MainThread source_thread;

        private bool is_running;

        public ConcurrentList<IActivePhysicsObject> active_physics_objects = new ConcurrentList<IActivePhysicsObject>();
        public ConcurrentList<IStaticPhysicsObject> static_physics_objects = new ConcurrentList<IStaticPhysicsObject>();

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




            //meat and butter of everything. does all object collision detection and speed reductions.
            foreach (IActivePhysicsObject activePhysicsObject in active_physics_objects)
            {
                AABB velocity_aabb = new AABB(activePhysicsObject.Collider);

                velocity_aabb.EncapsulateBounds(AABB.Size_To_AABB(activePhysicsObject.Collider.Center + activePhysicsObject.velocity, activePhysicsObject.Collider.size));

                List<IStaticPhysicsObject> static_physics_potential = new List<IStaticPhysicsObject>();


                foreach(IStaticPhysicsObject staticPhysicsObject in static_physics_objects) //so we're doing less calculations.
                {
                    if (staticPhysicsObject.Collider.Intercects(velocity_aabb))
                    {
                        static_physics_potential.Add(staticPhysicsObject);
                    }
                }
                foreach (IStaticPhysicsObject staticPhysicsObject in static_physics_potential)
                {
                    VelocityShrinkCollide(activePhysicsObject.Collider, velocity_aabb, activePhysicsObject.velocity, staticPhysicsObject.Collider);
                }



                    
            }







        }



        //hope and pray to the powers that be that this works - @989onan
        public void VelocityShrinkCollide(AABB original_obj, AABB Velocity, Vector2 Velocity_dir, AABB collider)
        {
            AABB velocity_Encapsulate = original_obj;

            
            if (Velocity.x_max > collider.x_min) //check if we could potentially hit a block based on our velocity
            {
                //if we are above or below the block at this coordinate restriction, then don't actually stop because our path will not hit it inbetween.
                //this checks if the cube if it were to stop at "collider" were to actually be touching it.
                //if it is, then actually shrink our velocity box by so.
                //reminder, maximum is the bottom and minimum is the top of the AABB. Top left is origin. - @989onan
                //this made my head hurt - @989onan
                if (((Velocity.y_max - (original_obj.y_max- original_obj.y_min)) < collider.y_max) || ((Velocity.y_min + (original_obj.y_max - original_obj.y_min)) > collider.y_min))
                {
                    Velocity.x_max = collider.x_min;
                    Velocity_dir.X = 0;
                }

            }

            if (Velocity.y_max > collider.y_min) //check if we could potentially hit a block based on our velocity
            {
                //if we are to either side of the block at this coordinate restriction, then don't actually stop because our path will not hit it inbetween.
                //this checks if the cube if it were to stop at "collider" were to actually be touching it.
                //if it is, then actually shrink our velocity box by so.
                //reminder, maximum is the bottom and minimum is the top of the AABB. Top left is origin. - @989onan
                //this made my head hurt - @989onan
                if (((Velocity.x_max - (original_obj.x_max - original_obj.x_min)) < collider.x_max) || ((Velocity.x_min + (original_obj.x_max - original_obj.x_min)) > collider.x_min))
                {
                    Velocity.y_max = collider.y_min;
                    Velocity_dir.Y = 0;
                }

            }



            if (Velocity.x_min > collider.x_max) //check if we could potentially hit a block based on our velocity
            {
                //if we are above or below the block at this coordinate restriction, then don't actually stop because our path will not hit it inbetween.
                //this checks if the cube if it were to stop at "collider" were to actually be touching it.
                //if it is, then actually shrink our velocity box by so.
                //reminder, maximum is the bottom and minimum is the top of the AABB. Top left is origin. - @989onan
                //this made my head hurt - @989onan
                if (((Velocity.y_max - (original_obj.y_max - original_obj.y_min)) < collider.y_max) || ((Velocity.y_min + (original_obj.y_max - original_obj.y_min)) > collider.y_min))
                {
                    Velocity.x_min = collider.x_max;
                    Velocity_dir.X = 0;
                }

            }

            if (Velocity.y_min > collider.y_max) //check if we could potentially hit a block based on our velocity
            {
                //if we are to either side of the block at this coordinate restriction, then don't actually stop because our path will not hit it inbetween.
                //this checks if the cube if it were to stop at "collider" were to actually be touching it.
                //if it is, then actually shrink our velocity box by so.
                //reminder, maximum is the bottom and minimum is the top of the AABB. Top left is origin. - @989onan
                //this made my head hurt - @989onan
                if (((Velocity.x_max - (original_obj.x_max - original_obj.x_min)) < collider.x_max) || ((Velocity.x_min + (original_obj.x_max - original_obj.x_min)) > collider.x_min))
                {
                    Velocity.y_min = collider.y_max;
                    Velocity_dir.Y = 0;
                }

            }


        }



    }
}
