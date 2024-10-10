using SpaceGame2D.enviroment.physics;
using SpaceGame2D.utilities.threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public class AABBMath<T> where T: ICollideable
    {

        public static IEnumerable<T> CollectCollideableIntercecting(IEnumerable<T> colliders, AABB box)
        {

            QueueableConcurrentList<T> result = new QueueableConcurrentList<T>();


            //chunk our collections to x so we can run multiple lines with x in each.
            
            foreach (T collider in colliders) //so we're doing less calculations.
            {

                if (collider.Collider.Intercects(box))
                {

                    result.Add(collider);
                }
                //Console.WriteLine("we have a static object.");


            }
            result.FlushQueue();

            return result;

        }
        public static IEnumerable<T> SweptAABBScene(IEnumerable<T> colliders, ICollideable original_obj, Vector2 Velocity_dir)
        {
            List<OrderedPlace<T>> orderedPhysics = new List<OrderedPlace<T>>();
            //float remainingtime = 0;
            //Console.WriteLine("Doing swept AABB on "+colliders.Count().ToString()+" colliders. ");
            //chunk our collections to x so we can run multiple lines with x in each.
            
            foreach (T staticPhysicsObject in colliders.Where(o => o != null))
            {
                

                Tuple<float, Vector2> result = SweptAABB(original_obj.Collider, staticPhysicsObject, Velocity_dir);
                //Console.WriteLine("has potential");

                //if (remainingtime < .01f) remainingtime = 1f;
                orderedPhysics.Add(new OrderedPlace<T>(result.Item1,staticPhysicsObject));

            }
           

            orderedPhysics.Sort();
            return orderedPhysics.Select(o => o.obj);

        }

        public static Tuple<float, Vector2> SweptAABB(AABB original_obj, T collider, Vector2 Velocity_dir)
        {
            Vector2 InvEntry = new Vector2(0, 0);
            Vector2 InvExit = new Vector2(0, 0);

            // find the distance between the objects on the near and far sides for both x and y 
            if (Velocity_dir.X > 0.0f)
            {
                InvEntry.X = collider.Collider.x_min - original_obj.x_max;
                InvExit.X = collider.Collider.x_max - original_obj.x_min;
            }
            else
            {
                InvEntry.X = collider.Collider.x_max - original_obj.x_min;
                InvExit.X = collider.Collider.x_min - original_obj.x_max;
            }

            if (Velocity_dir.Y > 0.0f)
            {
                InvEntry.Y = collider.Collider.y_min - original_obj.y_max;
                InvExit.Y = collider.Collider.y_max - original_obj.y_min;
            }
            else
            {
                InvEntry.Y = collider.Collider.y_max - original_obj.y_min;
                InvExit.Y = collider.Collider.y_min - original_obj.y_max;
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



            if (entryTime > exitTime) return new Tuple<float, Vector2>(1, normal); // This check was correct.
            if (Entry.X < 0.0f && Entry.Y < 0.0f) return new Tuple<float, Vector2>(1, normal);
            if (Entry.X < 0.0f)
            {
                if (original_obj.x_max <= collider.Collider.x_min || original_obj.x_min >= collider.Collider.x_max) return new Tuple<float, Vector2>(1, normal);
            }
            if (Entry.Y < 0.0f)
            {
                // Check that the bounding box started overlapped or not.
                //Console.WriteLine("no collision here!");
                if (original_obj.y_max <= collider.Collider.y_min || original_obj.y_min >= collider.Collider.y_max) return new Tuple<float, Vector2>(1, normal);
            }
            if (Entry.X > 1.0f || Entry.Y > 1.0f)
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

    }
}
