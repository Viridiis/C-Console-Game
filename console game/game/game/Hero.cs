using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Hero : Unit
    {
        public Hero(string name) : base(name, 50, 50, 10, 10, 10, 10, 10, Weapon.no_weapon)
        {

        }

        public void display_actions()
        {
            Console.WriteLine("1. Attack");
            Console.WriteLine("2. Defend");
            Console.WriteLine("3. Open spellbook");
            Console.WriteLine("4. Open inventory");
            Console.WriteLine("5. Wait");
            Console.WriteLine("6. Show your stats");
            Console.WriteLine("7. Show enemy's stats\n");
        }

        public void reset_stats(Stat_Snapshot snapshot)
        {
            strength = snapshot.strength;
            dexterity = snapshot.dexterity;
            inteligence = snapshot.inteligence;
            consitution = snapshot.constitution;

        }

        public Stat_Snapshot take_stats_snapshot()
        {
            return new Stat_Snapshot(strength, dexterity, inteligence, consitution, wits);
        }

    }
}
