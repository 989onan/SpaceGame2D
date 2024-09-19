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
        public Vector2 BottomRight;
        public Vector2 TopLeft;



        //origin is top left of screen.
        public float x_max { get => BottomRight.X; set => BottomRight.X = value; }

        public float y_max  {get => BottomRight.Y; set => BottomRight.Y = value; }

        public float x_min {get => TopLeft.X; set => TopLeft.X = value; }

        public float y_min { get => TopLeft.Y; set => TopLeft.Y = value; }

        public Vector2 size => new Vector2(x_max - x_min, y_max - y_min);

        //get center of box.
        public Vector2 Center { get => new Vector2(((x_max - x_min) / 2) + x_min, ((y_max - y_min) / 2) + y_min); set => setByCenter(value, this.size); }


        public Vector2 bottom_middle { get => new Vector2(((x_max - x_min) / 2), y_max); set => setByBottomMiddle(value); }

        private void setByCenter(Vector2 center, Vector2 size)
        {

            this.BottomRight = new Vector2(center.X + (size.X / 2), center.Y + (size.Y / 2));
            this.TopLeft = new Vector2(center.X - (size.X / 2), center.Y - (size.Y / 2));
        }

        public AABB(Vector2 BottomRight, Vector2 TopLeft)
        {
            this.BottomRight = BottomRight;
            this.TopLeft = TopLeft;



        }


        public AABB(AABB original)
        {
            this.TopLeft = original.TopLeft;
            this.BottomRight = original.BottomRight;
        }

        private void setByBottomMiddle(Vector2 bottomMiddle)
        {

            
            float middle_y = (((y_max - y_min) / 2)+bottomMiddle.Y);
            this.Center = new Vector2(middle_y, bottomMiddle.X);
            
        }

        public static AABB Size_To_AABB(Vector2 center, Vector2 size)
        {
            Vector2 BottomRight = new Vector2(center.X + (size.X / 2), center.Y + (size.Y / 2));
            Vector2 TopLeft = new Vector2(center.X - (size.X / 2), center.Y - (size.Y / 2));
            return new AABB(BottomRight, TopLeft);
        }


        public bool ContainsPoint(Vector2 point)
        {

            if (!(point.X < BottomRight.X))
            {
                return false;
            }
            if (!(point.X > TopLeft.X))
            {
                return false;
            }
            if (!(point.Y > BottomRight.Y))
            {
                return false;
            }
            if (!(point.Y < TopLeft.Y))
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

        //TODO: I have no clue if this works.
        public bool Intercects(AABB other)
        {
            if (other.TopLeft.X < BottomRight.X &&
                other.BottomRight.X > TopLeft.X &&
                other.TopLeft.Y < BottomRight.Y &&
                other.BottomRight.Y > TopLeft.Y)
            {
                return true;
            }
            return false;
        }
    }
}
