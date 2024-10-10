using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public static class BufferMod
    {
        public static T Get_Value<T>(this T[] bools, int width, int x, int y)
        {
            return bools[x + (y * width)];
        }

        public static void Set_Value<T>(this T[] bools, int width, int x, int y, T val)
        {
            bools[x + (y * width)] = val;
        }

        public static Tuple<int, int> OneDimensional_To_TwoDimensional(int width, int pos)
        {
            return new Tuple<int, int>(pos % width, (int)((float)pos / (float)width));
        }

    }
}
