using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.threads;
using SpaceGame2D.utilities.threading;
using System.Numerics;

namespace SpaceGame2D.utilities.ThreadSafePhysicsSolver 
{
    public class PhysicsSolver<A,B> where B: IActivePhysicsObject where A : ICollideable
    {
        private QueueableConcurrentList<B> active_physics_objects = new QueueableConcurrentList<B>();
        private QueueableOctree2D<A> static_physics_objects;
        public PhysicsSolver(AABB worldstaticSize)
        {
            static_physics_objects = new QueueableOctree2D<A>(worldstaticSize);


        }

        public IEnumerable<A> FindStaticPhysics(AABB area)
        {
            return static_physics_objects.FindCollisions(area);
        }

        public void QueueAddStatic(A a)
        {
            static_physics_objects.Add(a);
        }

        public bool QueueRemoveStatic(A a)
        {
            return static_physics_objects.Remove(a);
        }

        public void QueueAddActive(B a)
        {
            active_physics_objects.Add(a);
        }

        public bool QueueRemoveActive(B a)
        {
            return active_physics_objects.Remove(a);
        }

        public int ActiveCount()
        {
            return active_physics_objects.Count();
        }


        public void iterate(TimeSpan deltaTime)
        {
            this.active_physics_objects.FlushQueue();
            this.static_physics_objects.FlushQueue();
            float delta_times_secs = ((float)deltaTime.TotalSeconds);
            foreach (B obj in active_physics_objects)
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
                Vector2 sign = new Vector2(Math.Sign(physicsSpeedReducedVelocity.X), Math.Sign(physicsSpeedReducedVelocity.Y));

                BroadPhase.ExtendByVector(sign * (Math.Abs(physicsSpeedReducedVelocity.X) + Math.Abs(physicsSpeedReducedVelocity.Y)));

                //TODO: This will become excruciatingly slow if there is a lot of resting active physics objects possibly. fix this and reimplement maybe?
                //List<IStaticPhysicsObject> static_physics_potential = new List<IStaticPhysicsObject>(await AABBMath<IStaticPhysicsObject>.CollectCollideableIntercecting(staticPhysicsObjects, BroadPhase));

                List<A> static_physics_potential = this.FindStaticPhysics(BroadPhase).ToList();



                IEnumerable<A> orderedPhysics = AABBMath<A>.SweptAABBScene(static_physics_potential, obj, physicsSpeedReducedVelocity);
                obj.ground = null;
                foreach (A sorted_item in orderedPhysics)
                {
                    Tuple<float, Vector2> result = AABBMath<A>.SweptAABB(PhysCollider, sorted_item, physicsSpeedReducedVelocity);
                    float collisiontime = result.Item1;
                    Vector2 normal = result.Item2;
                    //i++;
                    if (obj.ground == null)
                    {
                        if (normal.Y == -1f)
                        {
                            obj.ground = sorted_item;
                        }
                    }

                    if (result.Item1 >= 1.0f)
                    {
                        continue;
                    }

                    float remainingtime = 1.0f - collisiontime;

                    PhysCollider.Center += physicsSpeedReducedVelocity * collisiontime;


                    float dotprod = (physicsSpeedReducedVelocity.X * normal.Y + physicsSpeedReducedVelocity.Y * normal.X) * remainingtime;

                    physicsSpeedReducedVelocity = new Vector2((float)(dotprod * normal.Y), (float)(dotprod * normal.X));
                }

                PhysCollider.Center += physicsSpeedReducedVelocity;


                obj.velocity = physicsSpeedReducedVelocity / delta_times_secs;
                obj.velocity += delta_times_secs * MainThread.Instance.cur_world.enviro.gravity;
                obj.position_physics = PhysCollider.Center;

            }
        }


        
    }
}
