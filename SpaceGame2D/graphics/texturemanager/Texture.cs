using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class Texture: IComparable<Texture>
    {

        public ImageResult image;

        public string path;





        public Texture(string texture_path)
        {

            //StbImage.stbi_set_flip_vertically_on_load(1);
            image = ImageResult.FromStream(File.OpenRead(Path.Join(BootStrapper.path, "graphics/textures/" + texture_path)), ColorComponents.RedGreenBlueAlpha);
            this.path = texture_path;
        }

        public int CompareTo(Texture other)
        {
            return (this.image.Height*this.image.Width).CompareTo(other.image.Width*other.image.Height);
        }
    }
}
