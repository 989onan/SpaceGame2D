using OpenTK.Windowing.Common.Input;
using SpaceGame2D.graphics.texturemanager.packer;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class TextureTile
    {
        public float x => (((float)start_x)/ ((float)Atlas.width));
        public float y => (((float)start_y) / ((float)Atlas.height));

        public float width => (((float)image.Width) / ((float)Atlas.width));
        public float height => (((float)image.Height) / ((float)Atlas.height));


        public readonly int start_x;
        public readonly int start_y;

        public string path { get; }

        public readonly ImageResult image;

        public TextureTile(int x, int y, Texture source_texture)
        {

            this.start_x = x;
            this.start_y = y;

            this.image = source_texture.image;
            this.path = source_texture.path;
        }

        public byte[] WriteImageToAtlas(byte[] cur_atlas)
        {
            int x = start_x;
            int y = start_y;
            int w = image.Width;
            int h = image.Height;

            //image_var: Image = eval("mat." + type)


            byte[] image_pixels = image.Data;


            Console.WriteLine("writing image \"" + path + "\" to Atlas.");
            Console.WriteLine(cur_atlas.Length);
            Console.WriteLine("x: \"" + x.ToString() + "\" " + "y: \"" + y.ToString() + "\" " + "w: \"" + w.ToString() + "\" " + "h: \"" + h.ToString() + "\" ");
            //taken from Avatar Toolkit.
            for (int k = 0; k < h; k++) {
                for (int i = 0; i < w; i++)
                {
                    for(int channel = 0; channel < 3; channel++) {
                        byte imagepixel = image_pixels[
                                        (int)((
                                            (k * w)
                                            + i) * 4)
                                        + (int)(channel)];

                        cur_atlas[
                            (int)((((k + y) * Atlas.width)
                            +
                            (i + x)) * 4)
                            + (int)(channel)] = imagepixel;
                    }
                        
                }
            }
                    



            return cur_atlas;
        }

        
    }
}
