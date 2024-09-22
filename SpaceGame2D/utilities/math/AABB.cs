using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.texturemanager;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.utilities.math
{
    public class AABB
    {
        public Vector2 Minimum;
        public Vector2 Maximum;



        //origin is top left of screen.
        public float x_max { get => Maximum.X; set => Maximum.X = value; }

        public float y_max { get => Maximum.Y; set => Maximum.Y = value; }

        public float x_min { get => Minimum.X; set => Minimum.X = value;  }

        public float y_min { get => Minimum.Y; set => Minimum.Y = value;}

        public Vector2 size => new Vector2(x_max - x_min, y_max - y_min);

        //get center of box.
        public Vector2 Center { get => new Vector2(((x_max - x_min) / 2) + x_min, ((y_max - y_min) / 2) + y_min); set => setByCenter(value, this.size); }

        public Vector2 bottom_middle { get => new Vector2(Center.X, y_min); set => setByBottomMiddle(value); }

        private void setByCenter(Vector2 center, Vector2 size)
        {

            this.Minimum = new Vector2(center.X - (size.X / 2), center.Y - (size.Y / 2));
            this.Maximum = new Vector2(center.X + (size.X / 2), center.Y + (size.Y / 2));
        }

        public AABB(Vector2 Minimum, Vector2 Maximum)
        {
            this.Minimum = Minimum;
            this.Maximum = Maximum;



        }


        public AABB(AABB original)
        {
            this.Maximum = original.Maximum;
            this.Minimum = original.Minimum;
        }

        private void setByBottomMiddle(Vector2 bottomMiddle)
        {

            
            float middle_y = (((y_max - y_min) / 2)+bottomMiddle.Y);
            this.Center = new Vector2(middle_y, bottomMiddle.X);
            
        }

        public static AABB Size_To_AABB(Vector2 center, Vector2 size)
        {
            Vector2 Minimum = new Vector2(center.X - (size.X / 2), center.Y - (size.Y / 2));
            Vector2 Maximum = new Vector2(center.X + (size.X / 2), center.Y + (size.Y / 2));
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


        public bool EncapsulateBounds(AABB other)
        {
            if (other == null)
            {
                return false;
            }

            if(other.x_max > this.x_max){
                this.x_max = other.x_max;
            }

            if (other.y_max > this.y_max)
            {
                this.y_max = other.y_max;
            }

            if (other.x_min < this.x_min)
            {
                this.x_min = other.x_min;
            }

            if (other.y_min < this.y_min)
            {
                this.y_min = other.y_min;
            }

            return true;
        }

        public bool ExtendByVector(Vector2 other)
        {

            if (other.X > 0)
            {
                this.x_max += other.X;
            }

            if (other.Y > 0)
            {
                this.y_max += other.Y;
            }

            if (other.Y < 0)
            {
                this.y_min += other.Y;
            }

            if (other.X < 0)
            {
                this.x_min += other.X;
            }

            return true;
        }


        public Vector2 PushOutOfMe(AABB other)
        {
            if (!Intercects(other))
            {
                return new Vector2(0, 0);
            }
            //direction we should push for shortest amount;
            float left = other.x_max - this.x_min;
            float top = this.y_max - other.y_min;
            float bottom = this.y_max - other.y_min;
            float right = this.x_max - other.x_min;

            int axies_infinity = 0;

            if(left > this.size.X / 2)
            {
                left = float.PositiveInfinity;
                axies_infinity++;
            }
            if (top > this.size.Y / 2)
            {
                top = float.PositiveInfinity;
                axies_infinity++;
            }
            if (bottom > this.size.Y / 2)
            {
                bottom = float.PositiveInfinity;
                axies_infinity++;
            }
            if (right > this.size.X / 2)
            {
                right = float.PositiveInfinity;
                axies_infinity++;
            }
            //Console.WriteLine(right.ToString()+","+left.ToString()+","+top.ToString()+","+bottom.ToString());  

            float choice = Math.Min(Math.Min(left, right), Math.Min(top, bottom));

            Vector2 push_dir = new Vector2(0, 0);

            if(axies_infinity == 4)
            {
                return new Vector2(0, this.y_max - other.y_min);
            }

            if (top == choice)
            {
                push_dir += new Vector2(0, top);
            }
            else if (left == choice) {
                push_dir -= new Vector2(left, 0);
            }
            else if (right == choice) {
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
            if (other.Maximum.X > Minimum.X &&
                other.Minimum.X < Maximum.X &&
                other.Maximum.Y > Minimum.Y &&
                other.Minimum.Y < Maximum.Y)
            {
                return true;
            }
            return false;
        }



        public void RestrictAABBWithinOurselves(AABB other)
        {

            if(Minimum.X > other.Minimum.X)
            {
                other.Center = new Vector2(Minimum.X+(other.size.X/2), other.Center.Y);
            }
            if (Minimum.Y > other.Minimum.Y)
            {
                other.Center = new Vector2(other.Center.X, Minimum.Y + (other.size.Y / 2));
            }
            if (Maximum.X < other.Maximum.X)
            {
                other.Center = new Vector2(Maximum.X - (other.size.X / 2), other.Center.Y);
            }
            if (Maximum.Y < other.Maximum.Y)
            {
                other.Center = new Vector2(other.Center.X, Maximum.Y - (other.size.Y / 2));
            }


        }
    }
}
