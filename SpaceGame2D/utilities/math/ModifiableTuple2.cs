using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public class ModifiableTuple2<T, T1>
    {
        public T Item1;
        public T1 Item2;

        public ModifiableTuple2(T point1, T1 point2)
        {
            this.Item1 = point1;
            this.Item2 = point2;
        }
        public override string ToString()
        {
            return "Item1: " + Item1.ToString() + " Item2: " + Item2.ToString();
        }
    }
}
