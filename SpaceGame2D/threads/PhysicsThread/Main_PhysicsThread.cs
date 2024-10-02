
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceGame2D.enviroment;
using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.enviroment.physics;
using SpaceGame2D.enviroment.species;
using SpaceGame2D.enviroment.world;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.utilities.math;
using SpaceGame2D.utilities.threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceGame2D.threads.PhysicsThread
{

    
    public class Main_PhysicsThread
    {

        public Task main_thread;

        private bool is_running;

        public static ConcurrentList<IActivePhysicsObject> active_physics_objects = new ConcurrentList<IActivePhysicsObject>();
        public static ConcurrentList<IStaticPhysicsObject> static_physics_objects = new ConcurrentList<IStaticPhysicsObject>();

        private DateTime last_time;

        public Main_PhysicsThread()
        {

            int millisecond_clock = 10;




            this.main_thread = new Task(async () =>
            {
                this.is_running = true;
                if (!MainThread.Instance.is_running)
                {
                    Thread.Sleep(1);
                }
                last_time = DateTime.Now;
                while (this.is_running)
                {
                    DateTime now = DateTime.Now;
                    await iterate();
                    last_time = DateTime.Now;

                    //I wish I could have atomic physics, but because of floating point error and how fast
                    //an infinitely fast clock would be, I have to restrict it to running at maxiumum 100 times a second to prevent issues.
                    float delta_times_secs = (float)((now - last_time).TotalMilliseconds);
                    //Console.WriteLine("Delta time:" + ((int)float.Round(millisecond_clock - delta_times_secs)).ToString());

                    await Task.Delay((int)float.Round(millisecond_clock - delta_times_secs));
                    
                    
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


        public DateTime last_break { get; private set; }
        public void ControlPlayer(float delta_times_secs, DateTime now)
        {
            GameWindow window = MainThread.Instance.graphics_thread.Window;
            Vector2 vel = MainThread.Instance.player.velocity;
            ISpecies species = MainThread.Instance.player.species;
            if (window.IsKeyDown(Keys.D))
            {
                vel = new Vector2(species.walk_speed, vel.Y);
            }
            if (window.IsKeyDown(Keys.A))
            {
                vel = new Vector2(-species.walk_speed, vel.Y);
            }
            if (!(window.IsKeyDown(Keys.D) || window.IsKeyDown(Keys.A)))
            {
                vel = new Vector2(vel.X / MainThread.Instance.cur_world.enviro.air_resistance_factor, vel.Y);
            }
            if (window.IsKeyDown(Keys.Space))
            {
                if (MainThread.Instance.player.ground != null && MainThread.Instance.player.ground.HasCollision == true)
                {
                    vel = new Vector2(vel.X, species.jump_velocity);
                }
                else
                {
                    //Console.WriteLine("failed ground check");
                }
            }
            if (window.IsKeyDown(Keys.E))
            {
                MainThread.Instance.player.storageScreen.IsVisible = true;
            }
            else
            {
                MainThread.Instance.player.storageScreen.CloseGui();

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
                //WindowRatio = window.Size.Y / window.Size.X;
            }

            if (window.Size.X > window.Size.Y)
            {


            }
            else
            {

            }
            //TODO: Mouse needs to be normalized based on it's position from center of screen, when coords represent the top left instead.
            //I think it's the top left at least - @989onan

            List<IStaticPhysicsObject> iblocks = static_physics_objects.FindAll(o => o is IBlock);

            float meters_reach_to_screen = species.reach_meters;


            Vector2 MouseClamped = new Vector2(Math.Clamp((MouseCenter / MainThread.Instance.graphics_thread.Zoom).X, -meters_reach_to_screen, meters_reach_to_screen), Math.Clamp((MouseCenter / MainThread.Instance.graphics_thread.Zoom).Y, -meters_reach_to_screen, meters_reach_to_screen));

            MainThread.Instance.graphics_thread.PhysicalMousePosition = MouseClamped + MainThread.Instance.player.position_physics;

            List<IStaticPhysicsObject> collection = new AABB(MainThread.Instance.player.Collider).ExtendByVector(MouseClamped)
                        .CollectAABBIntercectingMe(iblocks);
            //Console.WriteLine(MainThread.Instance.graphics_thread.RealMousePosition.X.ToString() + "," + MainThread.Instance.graphics_thread.RealMousePosition.Y.ToString());
            List<OrderedPlace<Tuple<IStaticPhysicsObject, Vector2>>> ray_interection = AABB.GetRayIntercectionAndNormal(new Vector4(
                MainThread.Instance.player.position_physics.X,
                MainThread.Instance.player.position_physics.Y,
                MainThread.Instance.graphics_thread.PhysicalMousePosition.X,
                MainThread.Instance.graphics_thread.PhysicalMousePosition.Y),
                    collection);
            //Console.WriteLine(collection.Count());
            ray_interection.Sort();
            //Console.WriteLine(ray_interection.Count());
            if (ray_interection.Count() > 0)
            {

                MainThread.Instance.selectedCube.target_block = (ray_interection.First().obj.Item1 as IBlock);
                //Console.WriteLine(MainThread.Instance.selectedCube.target_block.block_position.ToString());
            }
            else
            {
                MainThread.Instance.selectedCube.target_block = null;
                MainThread.Instance.selectedCube.alt_position = MainThread.Instance.graphics_thread.PhysicalMousePosition;
            }


            MainThread.Instance.player.velocity = vel;


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
                        //last_break = now;
                    }
                }
                
            }
            
        }

        private int seconds_recognized;




        private async Task iterate()
        {
            DateTime now = DateTime.Now;

            float delta_times_secs = (float)((now-last_time).TotalSeconds);

            //MainThread.Instance.cur_grid.RenderOffset = new Vector2(MainThread.Instance.cur_grid.RenderOffset.X, (MainThread.Instance.cur_grid.RenderOffset.Y)+ (delta_times_secs* .1f));
            ControlPlayer(delta_times_secs, now);
            //meat and butter of everything. does all object collision detection and speed reductions.
            List<IStaticPhysicsObject> staticPhysicsObjects = active_physics_objects.Where(o => !o.IsActive).Select(o => o as IStaticPhysicsObject).ToList();
            staticPhysicsObjects.AddRange(static_physics_objects);
            //Console.WriteLine(staticPhysicsObjects.Count().ToString());

            List<Task> physics_obj_threads = new List<Task>();

            active_physics_objects.ForEach(obj => physics_obj_threads.Add(
                Task.Run(async () =>
                {

                    //if (!obj.IsActive)
                    //{
                    //    obj.velocity = Vector2.Zero;
                    //    return;
                    //}
                    AABB velocity_aabb = new AABB(obj.Collider);

                    Vector2 new_positon = obj.position_physics;
                    Vector2 physicsSpeedReducedVelocity = (obj.velocity) * delta_times_secs;

                    List<IStaticPhysicsObject> static_physics_potential = velocity_aabb.ExtendByVector(physicsSpeedReducedVelocity).CollectAABBIntercectingMe(staticPhysicsObjects);

                    List<OrderedPlace<IStaticPhysicsObject>> orderedPhysics = new List<OrderedPlace<IStaticPhysicsObject>>();



                    List<Task> Static_calcs = new List<Task>();
                    //float remainingtime = 0;
                    foreach (IStaticPhysicsObject staticPhysicsObject in static_physics_potential.Where(o=>o != null))
                    {
                        Static_calcs.Add(Task.Run(() =>
                        {
                            Tuple<float, Vector2> result = AABB.SweptAABB(obj.Collider, staticPhysicsObject.Collider, physicsSpeedReducedVelocity);
                            //Console.WriteLine("has potential");

                            //if (remainingtime < .01f) remainingtime = 1f;
                            orderedPhysics.Add(new OrderedPlace<IStaticPhysicsObject>(result.Item1, staticPhysicsObject));
                        }));
                    }
                    await Task.WhenAll(Static_calcs.ToArray());
                    orderedPhysics.Sort();
                    obj.ground = null;
                    foreach (OrderedPlace<IStaticPhysicsObject> sorted_item in orderedPhysics)
                    {
                        IStaticPhysicsObject staticColliderClosest = sorted_item.obj;

                        AABB active_collider = new AABB(obj.Collider);
                        active_collider.Center = new_positon;

                        Tuple<float, Vector2> result = AABB.SweptAABB(active_collider, staticColliderClosest.Collider, physicsSpeedReducedVelocity);
                        float collisiontime = result.Item1;
                        Vector2 normal = result.Item2;
                        if (obj.ground == null)
                        {
                            if (normal.Y == -1f)
                            {
                                obj.ground = sorted_item.obj;
                            }

                        }
                        if (staticColliderClosest.Collider.Intercects(obj.Collider))
                        {
                            Vector2 diff = staticColliderClosest.Collider.PushOutOfMe(obj.Collider);
                            new_positon += diff;
                        }
                        if (result.Item1 >= 1.0f)
                        {
                            continue;
                        }

                        float remainingtime = 1.0f - collisiontime;

                        new_positon += physicsSpeedReducedVelocity * collisiontime;


                        float dotprod = (physicsSpeedReducedVelocity.X * normal.Y + physicsSpeedReducedVelocity.Y * normal.X) * remainingtime;

                        physicsSpeedReducedVelocity = new Vector2((float)(dotprod * normal.Y), (float)(dotprod * normal.X));
                        new_positon += physicsSpeedReducedVelocity;


                        if (staticColliderClosest.Collider.Intercects(obj.Collider))
                        {
                            Vector2 diff = staticColliderClosest.Collider.PushOutOfMe(obj.Collider);
                            new_positon += diff;
                        }
                        staticColliderClosest.TriggerCollideEvent(obj, normal);
                    }

                    if (orderedPhysics.Count() < 1)
                    {
                        new_positon += physicsSpeedReducedVelocity;
                    }

                    obj.velocity = physicsSpeedReducedVelocity / delta_times_secs;
                    obj.velocity += delta_times_secs * MainThread.Instance.cur_world.enviro.gravity;

                    obj.position_physics = new_positon;
                }))
            );


            //Console.WriteLine("awaiting");
            await Task.WhenAll(physics_obj_threads.ToArray());
            //Console.WriteLine("awaiting finished");



            if ((now - MainThread.Instance.gamestart).TotalSeconds > seconds_recognized)
            {
                seconds_recognized = (int)(now - MainThread.Instance.gamestart).TotalSeconds + 1;
                //Console.WriteLine("second passed on physics thread. seconds:" + seconds_recognized.ToString());
                //Console.WriteLine("FPS physics:" + (1 / delta_times_secs).ToString());

            }




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
