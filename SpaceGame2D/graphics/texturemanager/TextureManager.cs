using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.texturemanager
{
    public class TextureManager
    {
        private static Dictionary<string, Texture> _textureCache = new Dictionary<string, Texture>();

        private static Texture null_texture;
        public TextureManager() {
            

        }

        public static void LoadAll()
        {
            null_texture = new Texture("null.png");
            foreach(Texture tex in _textureCache.Values)
            {
                tex.Load();
            }
        }

        public static Texture getOrLoadTexture(string texture_path)
        {
            _textureCache.TryGetValue(texture_path, out Texture texture);
            if (texture == null) {
                Console.WriteLine("I am causing a shit ton of lag.");
                texture = loadTexture(texture_path);
                _textureCache.TryAdd(texture_path, texture);
            }
            return texture;
        }

        public static Texture loadTexture(string texture_path)
        {
            try
            {
                Texture new_tex = new Texture(texture_path);
                _textureCache.TryAdd(texture_path, new_tex);
                return new_tex;
            }
            catch (Exception ex) {

                Console.WriteLine("Failed to load texture "+texture_path);
                Console.WriteLine(ex.Message);
                return null_texture;
            }

            


        }

    }
}
