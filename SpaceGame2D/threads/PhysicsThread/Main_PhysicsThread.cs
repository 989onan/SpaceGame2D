
using SpaceGame2D.enviroment.physics;
using SpaceGame2D.graphics.texturemanager;
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

        public Vector2 gravity = new Vector2(0f, -9.8f);

        public static ConcurrentList<IActivePhysicsObject> active_physics_objects = new ConcurrentList<IActivePhysicsObject>();
        public static ConcurrentList<IStaticPhysicsObject> static_physics_objects = new ConcurrentList<IStaticPhysicsObject>();

        private DateTime last_time;

        public Main_PhysicsThread(MainThread source_thread)
        {

            int millisecond_clock = 5;

            this.main_thread = new Thread(new ThreadStart( () =>
            {
                this.is_running = true;
                if (!this.source_thread.is_running)
                {
                    Thread.Sleep(1);
                }
                last_time = DateTime.Now;
                while (this.is_running)
                {
                    DateTime now = DateTime.Now;
                    iterate();
                    last_time = DateTime.Now;

                    //I wish I could have atomic physics, but because of floating point error and how fast
                    //an infinitely fast clock would be, I have to restrict it to running at maxiumum 100 times a second to prevent issues.
                    float delta_times_secs = (float)((now - last_time).TotalSeconds);
                    //Console.WriteLine("Delta time:" + ((int)float.Round(millisecond_clock - delta_times_secs)).ToString());
                    Thread.Sleep((int)float.Round(millisecond_clock - delta_times_secs));
                    
                    
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

        private int seconds_recognized;


        private void iterate()
        {
            DateTime now = DateTime.Now;

            float delta_times_secs = (float)((now-last_time).TotalSeconds);

            //this.source_thread.cur_grid.RenderOffset = new Vector2(this.source_thread.cur_grid.RenderOffset.X, (this.source_thread.cur_grid.RenderOffset.Y)+ (delta_times_secs* .1f));

            //meat and butter of everything. does all object collision detection and speed reductions.
            foreach (IActivePhysicsObject activePhysicsObject in active_physics_objects)
            {
                AABB velocity_aabb = new AABB(activePhysicsObject.Collider);

                Vector2 new_positon = activePhysicsObject.position_physics;

                //activePhysicsObject.velocity += activePhysicsObject.newVelocityImpulse;
                //activePhysicsObject.newVelocityImpulse = Vector2.Zero;
                Vector2 physicsSpeedReducedVelocity = (activePhysicsObject.velocity) * delta_times_secs;
                //Vector2 retain_velocity = new Vector2(1, 1);

                velocity_aabb.ExtendByVector(physicsSpeedReducedVelocity);

                IStaticPhysicsObject[] static_physics_potential = new IStaticPhysicsObject[static_physics_objects.Count()];
                int position = 0;

                foreach(IStaticPhysicsObject staticPhysicsObject in static_physics_objects) //so we're doing less calculations.
                {
                    //Console.WriteLine("we have a static object.");
                    if (staticPhysicsObject.Collider.Intercects(velocity_aabb))
                    {
                        
                        static_physics_potential[position] = staticPhysicsObject;
                        position++;
                    }
                }

                

                List<OrderedPhysicsPlace> orderedPhysics = new List<OrderedPhysicsPlace>();
                

                activePhysicsObject.OnGround = false;
                //float remainingtime = 0;
                foreach (IStaticPhysicsObject staticPhysicsObject in static_physics_potential)
                {
                    if(staticPhysicsObject == null)
                    {
                        break;
                    }
                    Tuple<float, Vector2> result = SweptAABB(activePhysicsObject.Collider, staticPhysicsObject.Collider, physicsSpeedReducedVelocity);
                    //Console.WriteLine("has potential");

                    //if (remainingtime < .01f) remainingtime = 1f;
                    orderedPhysics.Add(new OrderedPhysicsPlace(result.Item1, staticPhysicsObject));

                }
                Vector2 normal = new Vector2(0, 0);
                Vector2 prevnormal = new Vector2(0, 0);
                orderedPhysics.Sort();
                //orderedPhysics.Reverse();
                int iterations = 0;
                foreach(OrderedPhysicsPlace sorted_item in orderedPhysics)
                {
                    IStaticPhysicsObject staticColliderClosest = sorted_item.obj;

                    AABB active_collider = new AABB(activePhysicsObject.Collider);
                    active_collider.Center = new_positon;

                    Tuple<float, Vector2> result = SweptAABB(active_collider, staticColliderClosest.Collider, physicsSpeedReducedVelocity);
                    float collisiontime = result.Item1;
                    normal = result.Item2;
                    if (!activePhysicsObject.OnGround)
                    {
                        activePhysicsObject.OnGround = normal.Y == -1f;
                    }
                    if (staticColliderClosest.Collider.Intercects(activePhysicsObject.Collider))
                    {
                        Vector2 diff = staticColliderClosest.Collider.PushOutOfMe(activePhysicsObject.Collider);
                        new_positon += diff;
                    }
                    if (collisiontime == 1f)
                    {
                        continue;
                    }
                    if (iterations > 1)
                    {
                        if(normal == prevnormal) continue;
                    }
                    //if (remainingtime < .01f) remainingtime = 1f;
                    
                    float remainingtime = 1.0f - collisiontime;
                    //double magnitude = Math.Sqrt((physicsSpeedReducedVelocity.X * physicsSpeedReducedVelocity.X + physicsSpeedReducedVelocity.Y * physicsSpeedReducedVelocity.Y)) * remainingtime;

                    new_positon += physicsSpeedReducedVelocity * collisiontime;

                    //float magnitude = (float)Math.Sqrt((physicsSpeedReducedVelocity.X * physicsSpeedReducedVelocity.X + physicsSpeedReducedVelocity.Y * physicsSpeedReducedVelocity.Y)) * remainingtime;
                    //float dotprod = physicsSpeedReducedVelocity.X * normal.Y + physicsSpeedReducedVelocity.Y * normal.X;

                    

                    //physicsSpeedReducedVelocity.X = dotprod * normal.Y * magnitude;
                    //physicsSpeedReducedVelocity.Y = dotprod * normal.X * magnitude;

                    float dotprod = (physicsSpeedReducedVelocity.X * normal.Y + physicsSpeedReducedVelocity.Y * normal.X) * remainingtime;
                    //if (dotprod > 0.0f) dotprod = 1.0f;
                    //else if (dotprod < 0.0f) dotprod = -1.0f;

                    physicsSpeedReducedVelocity = new Vector2((float)(dotprod * normal.Y), (float)(dotprod * normal.X));
                    new_positon += physicsSpeedReducedVelocity;

                    


                    iterations++;//last
                    if(iterations < 1)
                    {
                        prevnormal = normal;
                    }
                }

                if (iterations < 1)
                {
                    new_positon += physicsSpeedReducedVelocity;
                }
                
                activePhysicsObject.velocity = physicsSpeedReducedVelocity / delta_times_secs;
                activePhysicsObject.velocity += delta_times_secs * gravity;

                activePhysicsObject.position_physics = new_positon;

                // physicsSpeedReducedVelocity;

                //physicsSpeedReducedVelocity / delta_times_secs;

                //Console.WriteLine(physicsSpeedReducedVelocity.ToString());


            }


            if ((now - this.source_thread.gamestart).TotalSeconds > seconds_recognized)
            {
                seconds_recognized = (int)(now - this.source_thread.gamestart).TotalSeconds + 1;
                Console.WriteLine("second passed on physics thread. seconds:" + seconds_recognized.ToString());
                Console.WriteLine("FPS physics:" + (1 / delta_times_secs).ToString());

            }




        }

        private class OrderedPhysicsPlace : IComparable<OrderedPhysicsPlace>
        {
            public IStaticPhysicsObject obj;
            public float collide_time;
            public OrderedPhysicsPlace(float collide_time, IStaticPhysicsObject obj)
            {
                this.obj = obj;
                this.collide_time = collide_time;

            }
            public int CompareTo(OrderedPhysicsPlace other)
            {
                return collide_time.CompareTo(other.collide_time);
            }
        }

        public Tuple<float, Vector2> SweptAABB(AABB original_obj, AABB collider, Vector2 Velocity_dir)
        {
            Vector2 InvEntry = new Vector2(0, 0);
            Vector2 InvExit = new Vector2(0, 0);

            // find the distance between the objects on the near and far sides for both x and y 
            if (Velocity_dir.X > 0.0f)
            {
                InvEntry.X = collider.x_min - original_obj.x_max;
                InvExit.X = collider.x_max - original_obj.x_min;
            }
            else
            {
                InvEntry.X = collider.x_max - original_obj.x_min;
                InvExit.X = collider.x_min - original_obj.x_max;
            }

            if (Velocity_dir.Y > 0.0f)
            {
                InvEntry.Y = collider.y_min - original_obj.y_max;
                InvExit.Y = collider.y_max - original_obj.y_min;
            }
            else
            {
                InvEntry.Y = collider.y_max - original_obj.y_min;
                InvExit.Y = collider.y_min - original_obj.y_max;
            }


            Vector2 Entry;
            Vector2 Exit;

            if (Velocity_dir.X == 0.0f)
            {
                Entry.X = float.NegativeInfinity;
                Exit.X = float.PositiveInfinity;
            }
            else
            {
                Entry.X = InvEntry.X / Velocity_dir.X;
                Exit.X = InvExit.X / Velocity_dir.X;
            }

            if (Velocity_dir.Y == 0.0f)
            {
                Entry.Y = float.NegativeInfinity;
                Exit.Y = float.PositiveInfinity;
            }
            else
            {
                Entry.Y = InvEntry.Y / Velocity_dir.Y;
                Exit.Y = InvExit.Y / Velocity_dir.Y;
            }

            float entryTime = Math.Max(Entry.X, Entry.Y);
            float exitTime = Math.Min(Exit.X, Exit.Y);

            Vector2 normal = new Vector2(0, 0);



            /*if (entryTime > exitTime) return new Tuple<float, Vector2>(1, normal); // This check was correct.
            if (Entry.X < 0.0f && Entry.Y < 0.0f) return new Tuple<float, Vector2>(1, normal);
            if (Entry.X < 0.0f)
            {
                if (original_obj.x_max < collider.x_min || original_obj.x_min > collider.x_max) return new Tuple<float, Vector2>(1, normal);
            }
            if (Entry.Y < 0.0f)
            {
                // Check that the bounding box started overlapped or not.
                if (original_obj.y_max < collider.y_min || original_obj.y_min > collider.y_max) return new Tuple<float, Vector2>(1, normal);
            }*/
            if (entryTime > exitTime || Entry.X < 0.0f && Entry.Y < 0.0f || Entry.X > 1.0f || Entry.Y > 1.0f)
            {
                return new Tuple<float, Vector2>(1, normal);
            }
            else // if there was a collision 
            {
                // calculate normal of collided surface
                if (Entry.X > Entry.Y)
                {
                    if (InvEntry.X < 0.0f)
                    {
                        normal.X = 1.0f;
                        normal.Y = 0.0f;
                    }
                    else
                    {
                        normal.X = -1.0f;
                        normal.Y = 0.0f;
                    }
                }
                else
                {
                    if (InvEntry.Y < 0.0f)
                    {
                        normal.X = 0.0f;
                        normal.Y = 1.0f;
                    }
                    else
                    {
                        normal.X = 0.0f;
                        normal.Y = -1.0f;
                    }
                } // return the time of collisionreturn entryTime; 
            }

            return new Tuple<float, Vector2>(entryTime, normal);
        }


        //My poor attempts at collision code.
        /*public Tuple<AABB, Vector2, Vector2> VelocityShrinkCollide(AABB original_obj, AABB Velocity, Vector2 Velocity_dir, AABB collider)
        {
            AABB velocity_Encapsulate = original_obj;


            Vector2 retain_velocity = new Vector2(1, 1);




            //Vector2 overlap = new Vector2(0, 0);

            //Vector2


            

            if (collider.y_max > Velocity.y_min)
            {
                float reduceratio = 0;
                AABB potentialvelocitylimit = new AABB(original_obj);
                AABB potentialNewObjPosition = new AABB(original_obj);

                reduceratio = (collider.y_max - original_obj.y_min) / (Velocity.y_min - collider.y_max);
                //Console.WriteLine(reduceratio.ToString()+", ("+ collider.y_max.ToString() +"-"+original_obj.y_min.ToString() + ") / ("+ Velocity.y_min.ToString() +"-"+ collider.y_max.ToString()+")");
                Vector2 new_direction_limit = reduceratio * Velocity_dir;
                potentialvelocitylimit.ExtendByVector(new_direction_limit);

                potentialvelocitylimit.RestrictAABBWithinOurselves(potentialNewObjPosition);

                //if we are to either side of the block at this coordinate restriction, then don't actually stop because our path will not hit it inbetween.
                //this checks if the cube if it were to stop at "collider" were to actually be touching it.
                //this made my head hurt - @989onan
                if (potentialNewObjPosition.Touches(collider))
                {
                    //Console.WriteLine("passed x_vel test 1");
                    //Console.WriteLine(potentialvelocitylimit.y_min.ToString() + "," + potentialvelocitylimit.y_max.ToString() + "," + collider.x_min.ToString() + "," + collider.x_max.ToString());
                    retain_velocity.Y = 0;


                    Velocity.y_min  = collider.y_max;
                    if (Velocity.size.Y < original_obj.size.Y)
                    {
                        Velocity.y_max = collider.y_max + original_obj.size.Y;
                    }




                     retain_velocity.Y = 0;

                }

            }

            if (collider.y_min < Velocity.y_max)
            {
                float reduceratio = 0;

                //Vector2 normalized = Vector2.Normalize(original_obj.Center - collider.Center);

                AABB potentialvelocitylimit = new AABB(original_obj);
                AABB potentialNewObjPosition = new AABB(original_obj);

                reduceratio = (collider.y_min - original_obj.y_max) / (Velocity.y_max - collider.y_min);
                //Console.WriteLine(reduceratio.ToString() + ", (" + collider.y_max.ToString() + "-" + original_obj.y_min.ToString() + ") / (" + Velocity.y_min.ToString() + "-" + collider.y_max.ToString() + ")");
                Vector2 new_direction_limit = reduceratio * Velocity_dir;
                potentialvelocitylimit.ExtendByVector(new_direction_limit);

                potentialvelocitylimit.RestrictAABBWithinOurselves(potentialNewObjPosition);


                //if we are to either side of the block at this coordinate restriction, then don't actually stop because our path will not hit it inbetween.
                //this checks if the cube if it were to stop at "collider" were to actually be touching it.
                //this made my head hurt - @989onan
                if (potentialNewObjPosition.Touches(collider) )
                {
                    //Console.WriteLine("passed x_vel test 1");
                    //Console.WriteLine(potentialvelocitylimit.y_min.ToString() + "," + potentialvelocitylimit.y_max.ToString() + "," + collider.x_min.ToString() + "," + collider.x_max.ToString());
                    retain_velocity.Y = 0;


                    Velocity.y_max = collider.y_min;
                    if (Velocity.size.Y < original_obj.size.Y)
                    {
                        //Velocity.y_min = collider.y_min - original_obj.size.Y;
                    }




                     retain_velocity.Y = 0;

                }

            }



            return new Tuple<AABB, Vector2, Vector2>(Velocity, Velocity_dir, retain_velocity);
        }*/


    }
}
