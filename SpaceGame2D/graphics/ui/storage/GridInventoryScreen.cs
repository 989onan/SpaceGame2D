using SpaceGame2D.enviroment.storage;
using SpaceGame2D.graphics.compiledshaders;
using SpaceGame2D.graphics.renderables;
using SpaceGame2D.graphics.texturemanager;
using SpaceGame2D.graphics.ui;
using SpaceGame2D.threads.GraphicsThread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.graphics.ui.storage
{
    public class GridInventoryScreen : IStorageScreen
    {
        public bool IsVisible { get; set; }
        public int order { get; set; }

        public Vector2 position;

        public Vector2 GuiScreenSize;
        public int width { get; }
        public int height { get; }

        public IShader shader { get; set; }

        public ItemSlot[] slots { get; set; }
        public GridInventoryScreen(string shader, IStorageObject sourceObj, int width)
        {
            this.shader = ShaderManager.getAll().GetValueOrDefault(shader);
            slots = sourceObj.inventory.getSlots();
            height = (int)(slots.Length / (float)width);
            this.width = width;
            position = new Vector2(.05f, .05f);
            GuiScreenSize = new Vector2(.3f, .3f);
            //Console.WriteLine("created an inventory screen with "+slots.Length.ToString()+" length.");
            Main_GraphicsThread._renderableObjects.Add(this);
        }




        public void OpenUI()
        {
            this.IsVisible = true;
            
        }

        public bool DrawImage(float animationtime, Vector2 game_window_size)
        {
            
            if(!this.IsVisible) return false;
            //Console.WriteLine("rendering screen");
            float slot_size;
            if (GuiScreenSize.X > GuiScreenSize.Y)
            {
                slot_size = GuiScreenSize.Y / ((float)height);
            }
            else
            {
                slot_size = GuiScreenSize.X / ((float)width);

            }


            int iteration = 0;

            foreach (ItemSlot item in slots)
            {
                //Console.WriteLine(height.ToString());
                if(!item.DrawImage(position+new Vector2((iteration % width) * slot_size, ((int)(iteration / width)) * slot_size), new Vector2(slot_size, slot_size), game_window_size, animationtime))
                {
                    return false;
                }
                iteration++;
            }
            return true;
        }

        public void destruct()
        {
            Main_GraphicsThread._renderableObjects.Remove(this);
            slots = null;
        }

        public void CloseGui()
        {
            this.IsVisible = false;

        }
    }
}
