using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public enum weapon_type : uint
    {
        melee,
        ranged,
        magic,
        single_target_heal,
        aoe_heal
    }

    public static class Weapon_type_extensions
    {
        public static bool is_healing_type(this weapon_type weapon_type)
        {
            return weapon_type == weapon_type.single_target_heal || weapon_type == weapon_type.aoe_heal;
        }
    }

    public class Weapon
    {
        public string name;
        public int minDmg;
        public int maxDmg;
        public double strength_scaling;
        public double dexterity_scaling;
        public double inteligence_scaling;
        public Spell spell;
        public weapon_type type;



        public Weapon(string name, int minDmg, int maxDmg, double strength_scaling, double dexterity_scaling, double inteligence_scaling, weapon_type type)
        {
            this.name = name;
            this.minDmg = minDmg;
            this.maxDmg = maxDmg;
            this.strength_scaling = strength_scaling;
            this.dexterity_scaling = dexterity_scaling;
            this.inteligence_scaling = inteligence_scaling;
            this.type = type;
        }

        public Weapon(string name, int minDmg, int maxDmg, double strength_scaling, double dexterity_scaling, double inteligence_scaling, Spell spell, weapon_type type)
        {
            this.name = name;
            this.minDmg = minDmg;
            this.maxDmg = maxDmg;
            this.strength_scaling = strength_scaling;
            this.dexterity_scaling = dexterity_scaling;
            this.inteligence_scaling = inteligence_scaling;
            this.spell = spell;
            this.type = type;
        }

        public static Weapon no_weapon = new Weapon("no weapon", 0, 0, 0, 0, 0, weapon_type.melee);
    }
}
