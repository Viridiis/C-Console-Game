using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game
{
    public class Spell
    {
        public string name;
        public int minDmg;
        public int maxDmg;

        public Spell(string name, int minDmg, int maxDmg)
        {
            this.name = name;
            this.minDmg = minDmg;
            this.maxDmg = maxDmg;
        }

    }
}
