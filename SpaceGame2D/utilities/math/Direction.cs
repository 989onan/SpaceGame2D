using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{

    public enum Direction
    {
        None,
        Up,
        Left,
        Right,
        Down
    }
    public class DirectionMethods
    {

        
        public static Point Direct(Direction direction)
        {

            if (direction == Direction.Left)
            {
                return new Point(-1, 0);
            }
            else if (direction == Direction.Right)
            {
                return new Point(1, 0);
            }
            else if (direction == Direction.Up)
            {
                return new Point(0, 1);
            }
            else
            {
                return new Point(0, -1);
            }
        }
    }
    

}
