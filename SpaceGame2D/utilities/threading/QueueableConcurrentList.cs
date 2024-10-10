using SpaceGame2D.graphics.renderables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace SpaceGame2D.utilities.threading
{
    public class QueueableConcurrentList<T>: List<T>
    {
        private ConcurrentQueue<T> AddQue = new ConcurrentQueue<T>();
        private ConcurrentQueue<T> RemoveQue = new ConcurrentQueue<T>();
        public new void Add(T item)
        {
            AddQue.Enqueue(item);
        }

        public new bool Remove(T item)
        {
            if(RemoveQue.Contains(item)) return false;
            RemoveQue.Enqueue(item);
            return true;
        }

        public void FlushQueue()
        {
            if (!(AddQue.Count() > 0) && !(RemoveQue.Count() > 0)) return;
            while(AddQue.TryDequeue(out T item))
            {
                base.Add(item);
            }
            while(RemoveQue.TryDequeue( out T item))
            {
                base.Remove(item);
            }
            //Console.WriteLine("flushed queue!");
        }
    }
}
