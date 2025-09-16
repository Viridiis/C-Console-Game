using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Unit(string name, int maxHp, int currentHp, int strength, int dexterity, int inteligence, int constitution, int wits, Weapon weapon, int iniciative = 0, int crit_chance = 0)
    {
        public string name = name;
        public int maxHp = maxHp;
        public int currentHp = currentHp;
        public int strength = strength;
        public int dexterity = dexterity;
        public int inteligence = inteligence;
        public int consitution = constitution;
        public int wits = wits;
        public int iniciative = iniciative + wits;
        public int crit_chance = crit_chance + wits;
        public Weapon weapon = weapon;
        public Inventory inventory = new Inventory(5);
        public bool is_defending;
        public bool is_waiting;

        public int scale_damage(int damage)
        {
            damage = (int)(damage + ((strength - 10) * (0.5 * weapon.strength_scaling)));
            damage = (int)(damage + ((dexterity - 10)* (0.5 * weapon.dexterity_scaling)));
            damage = (int)(damage + ((inteligence - 10) * (0.5 * weapon.inteligence_scaling)));
            return damage;
        }

        public void display_base_info()
        {
            Console.WriteLine(name
                + " || Hp: " + currentHp + "/" + maxHp
                + " || Dmg: " + scale_damage(weapon.minDmg) + "-" + scale_damage(weapon.maxDmg)
                + " || Ini: " + iniciative
                /*+ " || Critical chance: " + crit_chance + "%"
                /+ " || Iniciative: " + iniciative*/);
        }

        public void display_stats()
        {
            Console.WriteLine("\n" + name
                + "\nStrength: " + strength 
                + " || Dexterity: " + dexterity 
                + " || Inteligence: " + inteligence 
                + " || Constitution: " + consitution
                + " || Wits: " + wits + "\n");
        }

       

        public int take_damage(int damage_taken)
        {
            int originalHp = currentHp;
            currentHp = int.Clamp(currentHp - damage_taken, 0, maxHp);
            return originalHp - currentHp;
        }

        public void attack(Unit attacked, int attacked_tile, int attacker_tile)
        {
            Unit attacker = this;
            Random random = new Random();
            int damage_rolled = random.Next(scale_damage(weapon.minDmg) , scale_damage(weapon.maxDmg)+1);
            if (attacked.is_defending) damage_rolled = (int)(damage_rolled * 0.5);
            attacked.take_damage(damage_rolled);
            Console.WriteLine($"[{attacker_tile}] {attacker.name} attacks [{attacked_tile}] {attacked.name} for {damage_rolled}");
        }

        public void mage_attack(int attacker_tile)
        {
            Team opposite_team = Game.board.get_unit_team(this) == Team.ally_team ? Team.enemy_team : Team.ally_team;
            foreach (Unit attacked in Game.board.get_all_units_in_team(opposite_team))
            {
                if (attacked.is_alive())
                {
                    this.attack(attacked, Game.board.get_unit_tile(attacked), attacker_tile);
                }
            }
        }

        public void heal(Unit healed_unit, int healed_unit_tile, int healer_tile)
        {
            Unit healer = this;
            Random random = new Random();
            int damage_healed = random.Next(scale_damage(weapon.minDmg), scale_damage(weapon.maxDmg) + 1);
            damage_healed = int.Abs(healed_unit.take_damage(-damage_healed));
            Console.WriteLine($"[{healer_tile}] {healer.name} heals [{healed_unit_tile}] {healed_unit.name} for {damage_healed}");
        }

        public void aoe_heal(int healer_tile)
        {
            Team current_team = Game.board.get_unit_team(this);
            foreach (Unit healed_unit in Game.board.get_all_units_in_team(current_team))
            {
                if (healed_unit.is_alive())
                {
                    this.heal(healed_unit, Game.board.get_unit_tile(healed_unit), healer_tile);
                }
            }
        }

        public void defend()
        {
            is_defending = true;
        }

        public void stop_defend()
        {
            is_defending = false;
        }

        public bool is_alive()
        {
            if(currentHp <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void show_inventory()
        {
            int i = 1;

            if(inventory.is_empty())
            {
                Console.WriteLine("Inventory is empty");
            }

            foreach (inventory_slot slot in inventory)
            {
                Console.WriteLine($"{i} - {slot.item.name} [{slot.item_count}]");
                i++;
            }
        }
        
        public bool give_item(Inventory_item item, int amount = 1)
        {
            if (item == null)
            {
                return false;
            }

            if (!inventory.can_add_item(item, amount))
            {
                Console.WriteLine("Inventory is already full");
                return false;
            }

            inventory.add_item(item, amount);
            return true;
        }




    }
}

