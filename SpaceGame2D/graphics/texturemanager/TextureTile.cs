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
    public class TextureTile: IComparable<TextureTile>
    {
        public float x => (((float)start_x)/ ((float)Atlas.width));
        public float y => (((float)start_y) / ((float)Atlas.height));

        public float width => (((float)image.Width) / ((float)Atlas.width));
        public float height => (((float)image.Height) / ((float)Atlas.height));


        public int start_x { get ; private set; }
        public int start_y { get ; private set; }

        private int new_start_y;

        private int new_start_x;

        public string path { get; }

        public readonly ImageResult image;

        public bool IsLoaded { get; private set; }

        public TextureTile(string texture_path)
        {

            //StbImage.stbi_set_flip_vertically_on_load(1);
            this.image = ImageResult.FromStream(File.OpenRead(Path.Join(BootStrapper.path, "graphics/textures/" + texture_path)), ColorComponents.RedGreenBlueAlpha);
            this.path = texture_path;
            this.IsLoaded = false;

        }




        public void SetPositionOnNewAtlas(int x, int y)
        {
            this.new_start_x = x;
            this.new_start_y = y;
        }

        public byte[] WriteImageToAtlas(byte[] cur_atlas)
        {
            int x = this.new_start_x;
            int y = this.new_start_y;
            this.start_x = x;
            this.start_y = y;

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



            this.IsLoaded = true;
            return cur_atlas;
        }
        public int CompareTo(TextureTile other)
        {
            return (this.image.Height + this.image.Width).CompareTo(other.image.Width + other.image.Height);
        }

        public void Deload()
        {
            this.IsLoaded = false;
        }
    }
}
