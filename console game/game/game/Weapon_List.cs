using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    internal class Weapon_List
    {
        public static List<Weapon> weapons = new List<Weapon>()
        {
            new Weapon("rusty sword", 5, 10, 1.5, 1.2, 0, weapon_type.melee),
            new Weapon("dagger", 4, 6, 0, 1.7, 0, weapon_type.melee),
            new Weapon("wand", 1, 2, 0, 1.2, 0, Spell_List.spells[0], weapon_type.magic),
            new Weapon("claw", 3, 5, 1, 1, 1, weapon_type.melee),
            new Weapon("shotgun", 100, 1500, 20, 1, 1, weapon_type.ranged),
            new Weapon("single_target_healing_weapon", 10, 20, 1, 1, 1, weapon_type.single_target_heal),
            new Weapon("aoe_healing_weapon", 4, 9, 1, 1, 1, weapon_type.aoe_heal)
        };
    }
}
