using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public struct inventory_slot
    {
        public Inventory_item item;
        public int item_count;
        public inventory_slot(Inventory_item item, int item_count)
        {
            this.item = item;
            this.item_count = item_count;
        }
    }
    public class Inventory : IEnumerable<inventory_slot>
    {
        
        List<inventory_slot> slots;
        int inventory_size;

        public Inventory(int inventory_size) {
            this.inventory_size = inventory_size;
            slots = new List<inventory_slot>(inventory_size);
        }

        public inventory_slot this[int index]
        {
            get
            {
                return slots[index];
            }
        }

        public void add_item(Inventory_item item, int amount = 1)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int item_index = find_item_index(item);

            if (item_index == -1)
            {
                if(slots.Count >= inventory_size)
                {
                    throw new Exception("inventory is full af");
                }
                slots.Add(new inventory_slot(item, amount)); 
            }
            else
            {
                //slots[item_index].item_count -= amount; code below is the same
                inventory_slot slot = slots[item_index];
                slot.item_count = slot.item_count + amount;
                slots[item_index] = slot;
            }
        }

        public bool can_add_item(Inventory_item item, int amount = 1)
        {
            if (item is null) return false;
            if (amount <= 0) return false;
            int item_index = find_item_index(item);

            if (item_index == -1)
            {
                if (slots.Count >= inventory_size)
                {
                    return false;
                }
            }
            return true;

        }


        public void remove_item(Inventory_item item, int amount = 1)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int item_index = find_item_index(item);
            if(item_index == -1)
            {
                throw new ArgumentException("somehow you removed non existing item from your inventory g");
            }

            if (slots[item_index].item_count < amount)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (slots[item_index].item_count == amount)
            {
                slots.RemoveAt(item_index);
            }
            else
            {
                //slots[item_index].item_count -= amount; code below is the same
                inventory_slot slot = slots[item_index];
                slot.item_count = slot.item_count - amount;
                slots[item_index] = slot;
            }
        }

        public bool is_empty()
        {
            if (inventory_size <= 0) return true;
            return slots.Count == 0;
        }

        public int find_item_index(Inventory_item item)
        {
            int index = 0;
            foreach (inventory_slot slot in slots)
            {
                if(slot.item == item)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public IEnumerator<inventory_slot> GetEnumerator()
        {
            return ((IEnumerable<inventory_slot>)slots).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)slots).GetEnumerator();
        }
    }
}
