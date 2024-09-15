using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class Texture
    {
        public int Handle;

        public ImageResult image;

        public Texture(string texture_path)
        {

            StbImage.stbi_set_flip_vertically_on_load(1);
            image = ImageResult.FromStream(File.OpenRead(Path.Join(BootStrapper.path, "graphics/textures/" + texture_path)), ColorComponents.RedGreenBlueAlpha);

        }

        public void Load()
        {
            
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }


        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);


        }

    }
}
