using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.threading
{
    
    //so we can add items and stuff but not write items while it's being written by another thread.

    public class ConcurrentList<T>: IEnumerable<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();


        public void Add(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                _list.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _list.AsReadOnly().AsEnumerable<T>().GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IEnumerable<T> ReadList()
        {
            _lock.EnterReadLock();
            try
            {
                return _list.AsReadOnly();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _list.AsReadOnly().AsEnumerable().GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}
