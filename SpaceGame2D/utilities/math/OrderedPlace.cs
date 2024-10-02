using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public class OrderedPlace<T> : IComparable<OrderedPlace<T>>
    {
        public T obj;
        public float collide_time;
        public OrderedPlace(float collide_time, T obj)
        {
            this.obj = obj;
            this.collide_time = collide_time;

        }
        public int CompareTo(OrderedPlace<T> other)
        {
            return collide_time.CompareTo(other.collide_time);
        }
    }
}
