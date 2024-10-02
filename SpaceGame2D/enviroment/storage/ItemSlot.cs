using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.storage
{
    public class ItemSlot
    {

        private int max_priv = 128; //NEVER USE DIRECTLY. USE BELOW.
        public int max { get => max_priv; set => max_priv = Math.Abs(value); }

        private int count_priv = 0; //NEVER USE DIRECTLY. USE BELOW.
        public int count { get => count_priv; set => count_priv = Math.Abs(value); }
        public StorageContainer parent {  get;}
        public Direction AccessableFrom { get; set;}

        public string Name { get; set;}

        public ItemSlot(StorageContainer root, Direction AccessableFrom = Direction.Up | Direction.Down | Direction.Left | Direction.Right, string Name = "")
        {
            parent = root;
            max = 128;
            this.AccessableFrom = AccessableFrom;
            this.Name = Name;
        }

        public IItemSource stored { get; set;  }



        public void addItems(ItemSlot itemStack, Direction AccessableFrom = Direction.Up)
        {
            if (!this.AccessableFrom.HasFlag(AccessableFrom))
            {
                //Console.WriteLine("Tried to insert from wrong direction on slot. This is fine!");
                return;
            }
            if (stored != null)
            {
                if (!stored.UniqueIdentifier.Equals(itemStack.stored.UniqueIdentifier))
                {
                    //Console.WriteLine("\"" + stored.UniqueIdentifier + "\" != \"" + itemStack.stored.UniqueIdentifier + "\"");
                    return;
                }
                //Console.WriteLine("before: \"" + stored.UniqueIdentifier + "\" == \"" + itemStack.stored.UniqueIdentifier + "\"");
            }
            else
            {
                //Console.WriteLine("before: \"null\" == \"" + itemStack.stored.UniqueIdentifier + "\"");
            }
            
            int remaining = 0;
            if ((itemStack.count + this.count) - max > 0) {
                remaining = (itemStack.count + this.count) - max;
            }


            //Console.WriteLine("before: count = " + count.ToString() + " remaining = " + remaining.ToString());
            this.count = Math.Clamp((itemStack.count + this.count), 0, max);
            stored = itemStack.stored;
            //Console.WriteLine("after: count = " + count.ToString() + " remaining = " + remaining.ToString());
            //Console.WriteLine("after: \"" + stored.UniqueIdentifier + "\" == \"" + itemStack.stored.UniqueIdentifier + "\"");
            itemStack.count = remaining;
        }

        public void Clear()
        {
            this.stored = null;
            this.count = 0;
        }


        public void grabIntoStack(ItemSlot itemStack, Direction AccessableFrom = Direction.Up) //automatically modifies given stack to have the items from ourselves.
        {
            if (!this.AccessableFrom.HasFlag(AccessableFrom))
            {
                //Console.WriteLine("Tried to insert from wrong direction on slot. This is fine!");
                return;
            }
            if (stored != null)
            {
                if (!stored.UniqueIdentifier.Equals(itemStack.stored.UniqueIdentifier))
                {
                    //Console.WriteLine("\"" + stored.UniqueIdentifier + "\" != \"" + itemStack.stored.UniqueIdentifier+"\"");
                    return;
                }


                int remainingroom = itemStack.max - itemStack.count;
                if (this.count <= remainingroom)
                {
                    
                    itemStack.count += this.count;
                    this.count = 0;
                    this.stored = null;
                }
                else
                {
                    itemStack.count = itemStack.max;
                    this.count -= remainingroom;
                }

            }
            else
            {
                return;
            }

        }

    }
}
