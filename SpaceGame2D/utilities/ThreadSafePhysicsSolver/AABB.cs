using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceGame2D.enviroment.blocks;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.texturemanager;
using StbImageSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace SpaceGame2D.utilities.ThreadSafePhysicsSolver

{
    public class AABB : IEquatable<AABB>
    {
        public Vector2 Minimum;
        public Vector2 Maximum;

        public override string ToString()
        {
            return "Min: " + Minimum.ToString() + " Max: " + Maximum.ToString();
        }

        //origin is top left of screen.
        public float x_max { get => Maximum.X; set => Maximum.X = value; }

        public float y_max { get => Maximum.Y; set => Maximum.Y = value; }

        public float x_min { get => Minimum.X; set => Minimum.X = value; }

        public float y_min { get => Minimum.Y; set => Minimum.Y = value; }

        public Vector2 size => new Vector2(x_max - x_min, y_max - y_min);

        //get center of box.
        public Vector2 Center { get => new Vector2((x_max - x_min) / 2 + x_min, (y_max - y_min) / 2 + y_min); set => setByCenter(value, size); }

        public Vector2 bottom_middle { get => new Vector2(Center.X, y_min); set => setByBottomMiddle(value); }

        private void setByCenter(Vector2 center, Vector2 size)
        {

            Minimum = new Vector2(center.X - size.X / 2, center.Y - size.Y / 2);
            Maximum = new Vector2(center.X + size.X / 2, center.Y + size.Y / 2);
        }

        public AABB(Vector2 Minimum, Vector2 Maximum)
        {
            this.Minimum = Minimum;
            this.Maximum = Maximum;



        }

        public AABB()
        {
            Maximum = Vector2.Zero;
            Minimum = Vector2.Zero;
        }


        public AABB(float xmin, float ymin, float xmax, float ymax)
        {
            Minimum = new Vector2(xmin, ymin);
            Maximum = new Vector2(xmax, ymax);
        }


        public AABB(AABB original)
        {
            Maximum = original.Maximum;
            Minimum = original.Minimum;
        }

        private void setByBottomMiddle(Vector2 bottomMiddle)
        {


            float middle_y = (y_max - y_min) / 2 + bottomMiddle.Y;
            Center = new Vector2(middle_y, bottomMiddle.X);

        }

        public static AABB Size_To_AABB(Vector2 center, Vector2 size)
        {
            Vector2 Minimum = new Vector2(center.X - size.X / 2, center.Y - size.Y / 2);
            Vector2 Maximum = new Vector2(center.X + size.X / 2, center.Y + size.Y / 2);
            return new AABB(Minimum, Maximum);
        }


        public bool ContainsPoint(Vector2 point)
        {

            if (!(point.X < Minimum.X))
            {
                return false;
            }
            if (!(point.X > Maximum.X))
            {
                return false;
            }
            if (!(point.Y > Minimum.Y))
            {
                return false;
            }
            if (!(point.Y < Maximum.Y))
            {
                return false;
            }
            return true;
        }

        public bool ContainsFully(AABB other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.x_max > x_max)
            {
                return false;
            }

            if (other.y_max > y_max)
            {
                return false;
            }

            if (other.x_min < x_min)
            {
                return false;
            }

            if (other.y_min < y_min)
            {
                return false;
            }

            return true;
        }
        public bool EncapsulateBounds(AABB other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.x_max > x_max)
            {
                x_max = other.x_max;
            }

            if (other.y_max > y_max)
            {
                y_max = other.y_max;
            }

            if (other.x_min < x_min)
            {
                x_min = other.x_min;
            }

            if (other.y_min < y_min)
            {
                y_min = other.y_min;
            }

            return true;
        }

        public AABB ExtendByVector(Vector2 other)
        {

            if (other.X > 0)
            {
                x_max += other.X;
            }

            if (other.Y > 0)
            {
                y_max += other.Y;
            }

            if (other.Y < 0)
            {
                y_min += other.Y;
            }

            if (other.X < 0)
            {
                x_min += other.X;
            }

            return this;
        }


        public bool canCreateRectangleFrom(AABB other)
        {
            //first if statements are if two sides touch
            if (other.Minimum.X == Maximum.X)
            {
                if (other.Minimum.Y == Minimum.Y && other.Maximum.Y == Maximum.Y)
                {
                    return true;
                }
            }
            else if (other.Minimum.Y == Maximum.Y)
            {
                if (other.Minimum.X == Minimum.X && other.Maximum.X == Maximum.X)
                {
                    return true;
                }
            }
            else if (Minimum.Y == other.Maximum.Y)
            {
                if (other.Minimum.X == Minimum.X && other.Maximum.X == Maximum.X)
                {
                    return true;
                }
            }
            else if (Minimum.X == other.Maximum.X)
            {
                if (other.Minimum.Y == Minimum.Y && other.Maximum.Y == Maximum.Y)
                {
                    return true;
                }
            }
            return false;

        }


        public Vector2 PushOutOfMe(AABB other)
        {
            if (!Intercects(other))
            {
                return new Vector2(0, 0);
            }
            //direction we should push for shortest amount;
            float left = other.x_max - x_min;
            float top = y_max - other.y_min;
            float bottom = other.y_max - y_min;
            float right = x_max - other.x_min;

            int axies_infinity = 0;

            if (left > size.X / 2)
            {
                left = float.PositiveInfinity;
                axies_infinity++;
            }
            if (top > size.Y / 2)
            {
                top = float.PositiveInfinity;
                axies_infinity++;
            }
            if (bottom > size.Y / 2)
            {
                bottom = float.PositiveInfinity;
                axies_infinity++;
            }
            if (right > size.X / 2)
            {
                right = float.PositiveInfinity;
                axies_infinity++;
            }
            //Console.WriteLine(right.ToString()+","+left.ToString()+","+top.ToString()+","+bottom.ToString());  

            float choice = Math.Min(Math.Min(left, right), Math.Min(top, bottom));

            Vector2 push_dir = new Vector2(0, 0);

            if (axies_infinity == 4)
            {
                return new Vector2(0, 0);//this.y_max - other.y_min);
            }

            if (top == choice)
            {
                push_dir += new Vector2(0, top);
            }
            else if (left == choice)
            {
                push_dir -= new Vector2(left, 0);
            }
            else if (right == choice)
            {
                push_dir += new Vector2(right, 0);
            }
            else if (bottom == choice)
            {
                push_dir -= new Vector2(0, bottom);
            }

            return push_dir;
        }

        public bool Intercects(AABB other)
        {

            if (other.Maximum.X > Minimum.X)
            {
                if (other.Minimum.X < Maximum.X)
                {
                    if (other.Maximum.Y > Minimum.Y)
                    {
                        if (other.Minimum.Y < Maximum.Y)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool Touches(AABB other)
        {
            //hopefully nesting this way allows for us to check only a few numbers. idk if this is faster or not.
            if (other.Maximum.X >= Minimum.X)
            {
                if (other.Minimum.X <= Maximum.X)
                {
                    if (other.Maximum.Y >= Minimum.Y)
                    {
                        if (other.Minimum.Y <= Maximum.Y)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        /*
        Func<ICollideable, bool> blocker
            List<Tuple<ICollideable, Vector2>> result = new List<Tuple<ICollideable, Vector2>>();

            resultbefore.Sort();

            List<Tuple<ICollideable, Vector2>> resultbefore2 = new List<Tuple<ICollideable, Vector2>>(resultbefore.Select(o => o.obj));


            //filter based on collide points.
            foreach (Tuple<ICollideable, Vector2> resultitem in resultbefore2)
            {
                if (!blocker(resultitem.Item1))
                {
                    break;
                }
                result.Add(resultitem);
            }

            return result;
         */



        public void RestrictAABBWithinOurselves(AABB other)
        {

            if (Minimum.X > other.Minimum.X)
            {
                other.Center = new Vector2(Minimum.X + other.size.X / 2, other.Center.Y);
            }
            if (Minimum.Y > other.Minimum.Y)
            {
                other.Center = new Vector2(other.Center.X, Minimum.Y + other.size.Y / 2);
            }
            if (Maximum.X < other.Maximum.X)
            {
                other.Center = new Vector2(Maximum.X - other.size.X / 2, other.Center.Y);
            }
            if (Maximum.Y < other.Maximum.Y)
            {
                other.Center = new Vector2(other.Center.X, Maximum.Y - other.size.Y / 2);
            }


        }


        public bool Equals(AABB other)
        {
            return other.Minimum == Minimum && other.Maximum == Maximum;
        }
    }
}
