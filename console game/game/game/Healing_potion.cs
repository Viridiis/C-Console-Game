using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Healing_potion : Inventory_item
    {
        public string name { get; init; }
        public int minHeal;
        public int maxHeal;
        public int tempHp;

        public Healing_potion(string name, int minHeal, int maxHeal)
        {
            this.name = name;
            this.minHeal = minHeal;
            this.maxHeal = maxHeal;
        }

        public int potion_used()
        {
            Random random = new Random();
            int healing = random.Next(minHeal, maxHeal + 1);
            return healing;
        }

        public void use_item(Game_context context)
        {
            int healed_health_amount = potion_used();
            tempHp = context.user.currentHp;
            context.user.currentHp += healed_health_amount;
            if (context.user.currentHp > context.user.maxHp)
            {
                Console.WriteLine(context.user.name + " healed for " + (context.user.maxHp - tempHp));
                context.user.currentHp = context.user.maxHp;
            }
            else
            {
                Console.WriteLine(context.user.name + " healed for " + healed_health_amount);
            }
        }
    }
}
