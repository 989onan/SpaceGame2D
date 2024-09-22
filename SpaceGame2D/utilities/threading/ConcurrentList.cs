using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.threading
{
    
    //so we can add items and stuff but not write items while it's being written by another thread.

    public class ConcurrentList<T>: List<T>
    {


        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


        public void AddThreadSafe(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                this.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        public void RemoveThreadSafe(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                this.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
