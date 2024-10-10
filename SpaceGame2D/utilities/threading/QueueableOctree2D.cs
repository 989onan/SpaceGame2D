using SpaceGame2D.enviroment.physics;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpaceGame2D.utilities.threading
{
    public class QueueableOctree2D<T> : Octree2D<T> where T : ICollideable
    {
        private ConcurrentQueue<T> AddQue = new ConcurrentQueue<T>();
        private ConcurrentQueue<T> RemoveQue = new ConcurrentQueue<T>();
        public new void Add(T item)
        {
            AddQue.Enqueue(item);
        }

        public QueueableOctree2D(AABB bounds): base(bounds)
        {
            this.bounds = bounds;
        }

        public new bool Remove(T item)
        {
            if (RemoveQue.Contains(item)) return false;
            RemoveQue.Enqueue(item);
            return true;
        }

        public void FlushQueue()
        {
            while (AddQue.TryDequeue(out T item))
            {
                base.Add(item);
            }
            while (RemoveQue.TryDequeue(out T item))
            {
                base.Remove(item);
            }
            //Console.WriteLine("flushed queue!");
        }
    }
}
