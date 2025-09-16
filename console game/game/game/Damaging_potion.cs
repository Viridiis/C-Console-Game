using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Damaging_potion : Inventory_item
    {
        public string name { get; init; }
        public int minDmg;
        public int maxDmg;

        public Damaging_potion(string name, int minDmg, int maxDmg)
        {
            this.name = name;
            this.minDmg = minDmg;
            this.maxDmg = maxDmg;
        }

        public int potion_used()
        {
            Random random = new Random();
            int damage_dealt = random.Next(minDmg, maxDmg + 1);
            return damage_dealt;
        }

        public void use_item(Game_context context)
        {
            int damage_dealt = potion_used();
            context.enemy.currentHp -= damage_dealt;
            Console.WriteLine(context.user.name + " used " + name + " on " + context.enemy.name + " and dealt " + damage_dealt + " damage.");
        }
    }
}