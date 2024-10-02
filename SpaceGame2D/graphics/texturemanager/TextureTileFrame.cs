using SpaceGame2D.graphics.texturemanager.packer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class TextureTileFrame
    {
        private int start_x;
        private int start_y;

        public float x => (((float)start_x) / ((float)Atlas.width))+(width*frame);
        public float y => (((float)start_y) / ((float)Atlas.height));

        private int frame;

        public float width => (((float)Width) / ((float)Atlas.width))/(float)frame_total;
        public float height => (((float)Height) / ((float)Atlas.height));

        private int frame_total;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool flip_x { get; set; }

        public TextureTileFrame(int width, int height, int start_x, int start_y, int frame, int frame_total)
        {
            Width = width;
            Height = height;
            this.start_x = start_x;
            this.start_y = start_y;
            this.frame = frame % frame_total;
            this.frame_total = frame_total;
        }
    }
}
