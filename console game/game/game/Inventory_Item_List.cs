using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Inventory_Item_List
    {
        public static List<Inventory_item> items = new List<Inventory_item>()
        {
            new Healing_potion("small potion of healing", 5, 10),
            new Healing_potion("potion of healing", 10, 20),
            new Damaging_potion("spike grenade", 10, 15),
            new Damaging_potion("granade", 20, 30),
            new Damaging_potion("explosive trap", 30, 50),
        };
    }
}
