using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    internal class Spell_List
    {
        public static List<Spell> spells = new List<Spell>()
        {
            new Spell("Fire Bolt", 1, 10),
            new Spell("Voltaic Bolt", 2, 20),
        };
    }
}
