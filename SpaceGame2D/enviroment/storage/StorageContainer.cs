using SpaceGame2D.graphics.ui;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame2D.enviroment.storage
{
    public class StorageContainer //not threading safe!
    {
        public int length => slots.Length;

        private ItemSlot[] slots;


        public StorageContainer(int amount)
        {

            slots = new ItemSlot[amount];
            for (int i = 0; i < amount; i++)
            {
                slots[i] = new ItemSlot(this);
            }
        }

        public ItemSlot getSlot(int pos)
        {
            if(pos > slots.Length)
            {
                throw new IndexOutOfRangeException("Cannot go outside container slot bounds!!");
            }
            else
            {
                return slots[pos];
            }
        }

        public ItemSlot[] getSlots()
        {
            return slots.ToArray();
        }

        //slot count will have how much was not added
        public void AddToAny(ItemSlot slot, Direction inserting_from = Direction.Up) 
        {

            foreach (ItemSlot slot_in_ourselves in slots)
            {
                slot_in_ourselves.addItems(slot, inserting_from);
                if (!(slot.count > 0)) return;
            }

            //add the rest to new slots
            foreach (ItemSlot slot_in_ourselves in slots)
            {
                slot_in_ourselves.addItems(slot, inserting_from);
                if (!(slot.count > 0)) return;
            }

            //if we get here, the item can't fit in this inventory.
        }


        //use a slot for an intemediary.
        //if it's a certain amount per second for transfer (EX: A factory machine), then set max to that amount and only call this function per second, then add the stuff to our own inventory
        //don't set max higher than what we could put into our own inventory for a factory.
        public void RemoveFromAny(ItemSlot slot, Direction inserting_from = Direction.Up)
        {

            foreach (ItemSlot slot_in_ourselves in slots)
            {
                slot_in_ourselves.grabIntoStack(slot, inserting_from);
                if (!(slot.count == slot.max)) return;
            }

            //if we get here, the item wasn't in this inventory.
        }

    }
}
