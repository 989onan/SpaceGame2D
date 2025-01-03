﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public class ModifiableTuple2<T>: IEnumerable<T>
    {
        public T Item1;
        public T Item2;

        public ModifiableTuple2(T point1, T point2)
        {
            this.Item1 = point1;
            this.Item2 = point2;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new T[] { Item1, Item2 }.GetEnumerator() as IEnumerator<T>;
        }

        public override string ToString()
        {
            return "Item1: " + Item1.ToString() + " Item2: " + Item2.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new T[] { Item1, Item2 }.GetEnumerator();
        }
    }
}
