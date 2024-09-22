using OpenTK.Core;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpaceGame2D.graphics.texturemanager.packer
{
    public class Atlas
    {
        private static int _width;
        private static int _height;

        public static int width => _width;
        public static int height => _height;

        public static ReadOnlyDictionary<string, TextureTile> Tiles => _Tiles.AsReadOnly();

        private static Dictionary<string, TextureTile> _Tiles = new Dictionary<string, TextureTile>();


        private static int Handle;
        public static byte[] texture => _internal_texture;

        private static byte[] _internal_texture = new byte[0];

        public static bool isLoaded { get; private set; }

        public Atlas()
        {



        }

        public static void LoadToGPU()
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _internal_texture);
        }

        public static void UseImage()
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public static TextureTile AddToAtlas(string path) {
            isLoaded = false;
            if (_Tiles.ContainsKey(path))
            {
                return _Tiles.GetValueOrDefault(path);
            }
            else
            {
                _Tiles.TryAdd(path, new TextureTile(path));
                return _Tiles.GetValueOrDefault(path);
            }

            
        }

        public static TextureTile getTexture(string path)
        {
            if (Atlas.Tiles.ContainsKey(path))
            {
                return Atlas.Tiles.GetValueOrDefault(path);
            }
            else
            {
                return Atlas.Tiles.GetValueOrDefault("null.png");
            }
        }

        public static void RegenerateAtlas()
        {
            //Console.WriteLine("regenerating atlas!");
            foreach (string filepath in Directory.GetFiles(Path.Join(BootStrapper.path, "graphics/textures/"), "*.*", SearchOption.AllDirectories))
            {
                string filepath_real = filepath.Split("graphics/textures/")[1];
                if (!(filepath_real.Replace("\\", "/").EndsWith(".png") || filepath_real.Replace("\\", "/").EndsWith(".bmp") || filepath_real.Replace("\\", "/").EndsWith(".tga") || filepath_real.Replace("\\", "/").EndsWith(".tiff")))
                {
                    continue;
                }
                TextureTile texture = Atlas.AddToAtlas(filepath_real.Replace("\\","/"));
            }
            foreach (TextureTile tile in _Tiles.Values)
            {
                tile.Deload();
            }
            
            BinPacker regenerator = new BinPacker();


            

            regenerator.fit();

            _width = regenerator.root.w;
            _height = regenerator.root.h;

            _internal_texture = new byte[_width * _height * 4];

            Console.WriteLine(_internal_texture.Length);

            foreach (TextureTile tile in _Tiles.Values)
            {
                Console.WriteLine("writing "+tile.path);
                tile.WriteImageToAtlas(_internal_texture);
            }

            Console.WriteLine("atlas regenerated!");
            isLoaded = true;
        }


        //class taken from Avatar Toolkit.
        private class BinPacker
        {
            public Rectangle_Obj root;
            private List<TextureTile> bin = new List<TextureTile>();
            public BinPacker() {
                this.root = null;
                this.bin = Atlas._Tiles.Values.ToList();

                this.bin.Sort();
                this.bin.Reverse();
                bin.ForEach((TextureTile texture) =>
                {
                    Console.WriteLine("texture size: " + (texture.image.Width*texture.image.Height).ToString());
                });
                
            }

            public void fit()
            {


                List<TextureTile> structure = this.bin;
                int structure_len = this.bin.Count;
                int w = 0;
                int h = 0;
                if (structure_len > 0)
                {
                    w = structure[0].image.Width;
                    h = structure[0].image.Height;
                }

                this.root = new Rectangle_Obj(0, 0, w, h);
                foreach (TextureTile img in structure)
                {
                    w = img.image.Width;
                    h = img.image.Height;

                    Rectangle_Obj node = this.root.find(w, h);
                    if (node != null)
                    {
                        Rectangle_Obj fit = node.split(w, h);


                        img.SetPositionOnNewAtlas(fit.x, fit.y);
                    }
                    else
                    {
                        Rectangle_Obj fit = this.grow_node(w, h);

                        img.SetPositionOnNewAtlas(fit.x, fit.y);
                    }

                }
            }

            public Rectangle_Obj grow_node(int w, int h){
                bool can_grow_right = (h <= this.root.h);
                bool can_grow_down = (w <= this.root.w);

                bool should_grow_right = can_grow_right && (this.root.h >= (this.root.w + w));
                bool should_grow_down = can_grow_down && (this.root.w >= (this.root.h + h));

                if (should_grow_right)
                {
                    return this.grow_right(w, h);
                }
                else if (should_grow_down)
                {
                    return this.grow_down(w, h);
                }
                else if(can_grow_right){
                    return this.grow_right(w, h);
                }
                else if (can_grow_down)
                {
                    return this.grow_down(w, h);
                }

                return null; //if this fires, the shit wasn't in order of size in the que. - @989onan
            }

            public Rectangle_Obj grow_right(int w, int h){
                this.root = new Rectangle_Obj(
                        0,
                        0,
                        this.root.w + w,
                        this.root.h,
                        this.root,
                        true,
                        new Rectangle_Obj(this.root.w, 0, w, this.root.h));
                Rectangle_Obj node = this.root.find(w, h);
                if(node != null)
                {
                    return node.split(w, h);
                }
                return null;
            }


            public Rectangle_Obj grow_down(int w, int h)
            {
                this.root = new Rectangle_Obj(

                    0,
                    0,
                    this.root.w,
                    this.root.h + h,
                    new Rectangle_Obj(0, this.root.h, this.root.w, h),
                    true,
                    this.root
                );
                Rectangle_Obj node = this.root.find(w, h);
                if(node != null){
                    return node.split(w, h);
                }
                return null;
            }
        }
                

    }
}
