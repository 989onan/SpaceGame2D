
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

        public static QueueableConcurrentList<IActivePhysicsObject> active_physics_objects = new QueueableConcurrentList<IActivePhysicsObject>();
        public static QueueableOctree2D<IStaticPhysicsObject> static_physics_objects = new QueueableOctree2D<IStaticPhysicsObject>(new AABB(-10,-10,BlockGrid.size_grid*210, BlockGrid.size_grid * 210));

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

                        Console.WriteLine("Physics engine has been saturated at a capacity of: " + active_physics_objects.Count().ToString() + " skipping!");
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
                vel = new Vector2(vel.X / MainThread.Instance.cur_world.enviro.air_resistance_factor, vel.Y);
            }
            if (player.JetPackOn)
            {
                float jetpackvel = (Player.JetPackForce / species.weight_kg);
                if (window.IsKeyDown(Keys.D))
                {
                    vel = new Vector2(jetpackvel, vel.Y);
                }
                if (window.IsKeyDown(Keys.A))
                {
                    vel = new Vector2(-jetpackvel, vel.Y);
                }
                if (window.IsKeyDown(Keys.W))
                {
                    // + vel.Y
                    vel = new Vector2(vel.X, jetpackvel);
                }
                if (window.IsKeyDown(Keys.Space))
                {
                    // + vel.Y
                    Console.WriteLine(player.position_physics.ToString());
                    player.position_physics = Vector2.Zero;
                    vel = Vector2.Zero;
                    Console.WriteLine("resetting player position.");
                }
                if (window.IsKeyDown(Keys.S))
                {
                    vel = new Vector2(vel.X, -jetpackvel);
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
            IEnumerable<IBlock> collection = static_physics_objects.FindCollisions(new AABB(MainThread.Instance.player.Collider).ExtendByVector(MouseClamped)).OfType<IBlock>();
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
                        

                        //last_break = now;
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

        private int seconds_recognized;




        private void iterate(DateTime now)
        {
            
            active_physics_objects.FlushQueue();
            static_physics_objects.FlushQueue();
            float delta_times_secs = (float)((now - last_time).TotalSeconds);

            //MainThread.Instance.cur_grid.RenderOffset = new Vector2(MainThread.Instance.cur_grid.RenderOffset.X, (MainThread.Instance.cur_grid.RenderOffset.Y)+ (delta_times_secs* .1f));
            ControlPlayer(delta_times_secs, now);
            //meat and butter of everything. does all object collision detection and speed reductions.
            //List<IStaticPhysicsObject> staticPhysicsObjects = active_physics_objects.Where(o => !o.IsActive).Select(o => o as IStaticPhysicsObject).ToList();
            
            //Console.WriteLine(staticPhysicsObjects.Count().ToString());


            //chunk our active physics objects into tasks that are 40 items big.
            //Console.WriteLine("iterate phys!");
            foreach (IActivePhysicsObject obj in active_physics_objects)
            {
                if (!obj.IsActive)
                {
                    obj.velocity = Vector2.Zero;
                    continue;
                }
                obj.velocity = new Vector2(float.IsNaN(obj.velocity.X) ? 0f : obj.velocity.X, float.IsNaN(obj.velocity.Y) ? 0f : obj.velocity.Y);
                
                AABB PhysCollider = new AABB(obj.Collider);
                AABB BroadPhase = new AABB(obj.Collider);
                Vector2 new_positon = obj.position_physics;
                
                Vector2 physicsSpeedReducedVelocity = (obj.velocity) * delta_times_secs;
                //Console.WriteLine(obj.velocity);
                
                //Console.WriteLine(delta_times_secs);
                //Vector2 velocity = obj.velocity;
                Vector2 sign = new Vector2(Math.Sign(physicsSpeedReducedVelocity.X), Math.Sign(physicsSpeedReducedVelocity.Y));
                //DateTime start_phys_coll = DateTime.Now;

                //+ (sign*0.5f));

                BroadPhase.ExtendByVector(sign * (Math.Abs(physicsSpeedReducedVelocity.X) + Math.Abs(physicsSpeedReducedVelocity.Y)));

                //TODO: This will become excruciatingly slow if there is a lot of resting active physics objects possibly. fix this and reimplement maybe?
                //List<IStaticPhysicsObject> static_physics_potential = new List<IStaticPhysicsObject>(await AABBMath<IStaticPhysicsObject>.CollectCollideableIntercecting(staticPhysicsObjects, BroadPhase));

                List<IStaticPhysicsObject> static_physics_potential = static_physics_objects.FindCollisions(BroadPhase).ToList();

                

                IEnumerable<IStaticPhysicsObject> orderedPhysics = AABBMath<IStaticPhysicsObject>.SweptAABBScene(static_physics_potential, obj, physicsSpeedReducedVelocity);
                //Console.WriteLine("doiing aabb on "+ orderedPhysics.Count().ToString());
                obj.ground = null;
                //DateTime start_phys = DateTime.Now;
                //int i = 0;
                //Console.WriteLine("start physics.");
                foreach (IStaticPhysicsObject sorted_item in orderedPhysics)
                {
                    //BroadPhase = new AABB(PhysCollider);
                    //BroadPhase.ExtendByVector(physicsSpeedReducedVelocity);
                    Tuple<float, Vector2> result = AABBMath<IStaticPhysicsObject>.SweptAABB(PhysCollider, sorted_item, physicsSpeedReducedVelocity);
                    float collisiontime = result.Item1;
                    Vector2 normal = result.Item2;
                    //i++;
                    if (obj.ground == null)
                    {
                        if (normal.Y == -1f)
                        {
                            //Console.WriteLine("setting ground.");
                            obj.ground = sorted_item;
                        }

                    }
                    
                    if (result.Item1 >= 1.0f)
                    {
                        continue;
                    }

                    float remainingtime = 1.0f - collisiontime;

                    //Console.WriteLine("collision time: " + collisiontime.ToString());

                    PhysCollider.Center += physicsSpeedReducedVelocity * collisiontime;
                    

                    float dotprod = (physicsSpeedReducedVelocity.X * normal.Y + physicsSpeedReducedVelocity.Y * normal.X) * remainingtime;

                    physicsSpeedReducedVelocity = new Vector2((float)(dotprod * normal.Y), (float)(dotprod * normal.X));



                    //Console.WriteLine("physics speed at end: " + physicsSpeedReducedVelocity.ToString());
                }
                //Console.WriteLine("end physics.");

                //await Task.WhenAll(calculations);
                //Console.WriteLine("physics took longer than gather?: " + ((start_phys - DateTime.Now) > start_phys_coll - start_phys).ToString());

                PhysCollider.Center += physicsSpeedReducedVelocity;

                //await Task.Delay(100);

                obj.velocity = physicsSpeedReducedVelocity / delta_times_secs;
                obj.velocity += delta_times_secs * MainThread.Instance.cur_world.enviro.gravity;
                obj.position_physics = PhysCollider.Center;

            }
            
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
