using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager.packer
{
    //class taken from Avatar Toolkit.
    class Rectangle_Obj
    {
        public int x = 0;
        public int y = 0;
        public int w = 0;
        public int h = 0;
        Rectangle_Obj down = null;
        bool used = false;
        //TextureTile internal_image;
        Rectangle_Obj right = null;

        public Rectangle_Obj(int x, int y, int w, int h, Rectangle_Obj down = null, bool used = false, Rectangle_Obj right = null) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.down = down;
            this.used = used;
            this.right = right;
        }

        public Rectangle_Obj split(int w, int h) {


            this.used = true;
            
            
            this.down = new Rectangle_Obj(this.x, this.y + h, this.w, this.h - h);
            this.right = new Rectangle_Obj(this.x + w, this.y, this.w - w, h);
            return this;
        }

        public Rectangle_Obj find(int w, int h){
            if(this.used){
                Rectangle_Obj right = this.right.find(w, h);
                if (right != null){ return right; }
                else { return this.down.find(w, h); }
            }
                
            else if((w <= this.w) && (h <= this.h)){
                return this;
            }
                
            return null;
        }
    }
    
}
